-- CREATIONS

CREATE DATABASE AccessControl;
GO

USE AccessControl;
GO

CREATE TABLE Establishments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    MaxCapacity INT NULL, -- NULL means no capacity limit
    Address NVARCHAR(200) NULL,
    City NVARCHAR(50) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- hash
    FullName NVARCHAR(100) NOT NULL,
    EstablishmentId INT NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Employee')),
    IsActive BIT DEFAULT 1,
    IdentityDocument NVARCHAR(20) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    MustChangePassword BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    LastLoginDate DATETIME2 NULL,
    
    FOREIGN KEY (EstablishmentId) REFERENCES Establishments(Id)
);

CREATE TABLE AccessRecords (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    EstablishmentId INT NOT NULL,
    EntryDateTime DATETIME2 NOT NULL,
    ExitDateTime DATETIME2 NULL,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (EstablishmentId) REFERENCES Establishments(Id)
);

CREATE TABLE EstablishmentOpenings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EstablishmentId INT NOT NULL,
    UserId INT NOT NULL, 
    OpeningDateTime DATETIME2 NOT NULL,
    ClosingDateTime DATETIME2 NULL,
    Status NVARCHAR(20) DEFAULT 'Open' CHECK (Status IN ('Open', 'Closed')),
    
    FOREIGN KEY (EstablishmentId) REFERENCES Establishments(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- PROCEDURES

-- SP for User Login
CREATE PROCEDURE sp_UserLogin
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT;
    DECLARE @StoredHash NVARCHAR(255);
    DECLARE @IsActive BIT;
    DECLARE @EstablishmentId INT;
    
    SELECT 
        @UserId = Id,
        @StoredHash = Password,
        @IsActive = IsActive,
        @EstablishmentId = EstablishmentId
    FROM Users 
    WHERE Email = @Email;
    
    IF @UserId IS NULL
    BEGIN
        SELECT 
            NULL AS UserData,
            NULL AS Establishment,
            'User not found' AS Message,
            0 AS Success;
        RETURN;
    END
    
    IF @IsActive = 0
    BEGIN
        SELECT 
            NULL AS UserData,
            NULL AS Establishment,
            'User inactive' AS Message,
            0 AS Success;
        RETURN;
    END
    
    -- Password verification will be done in code with BCrypt
    IF @StoredHash IS NOT NULL
    BEGIN
        -- Update last login
        UPDATE Users SET LastLoginDate = GETDATE() WHERE Id = @UserId;
        
        SELECT 
            U.Id,
            U.Email,
            U.FullName,
            U.EstablishmentId,
            U.Role,
            U.IsActive,
            U.IdentityDocument,
            U.PhoneNumber,
            U.MustChangePassword,
            U.CreatedDate,
            U.LastLoginDate,
            E.Name AS EstablishmentName,
            E.MaxCapacity,
            'Login successful' AS Message,
            1 AS Success
        FROM Users U
        INNER JOIN Establishments E ON U.EstablishmentId = E.Id
        WHERE U.Id = @UserId;
    END
    ELSE
    BEGIN
        SELECT 
            NULL AS UserData,
            NULL AS Establishment,
            'Invalid credentials' AS Message,
            0 AS Success;
    END
END
GO

-- SP for Register Entry
CREATE PROCEDURE sp_RegisterEntry
    @IdentityDocument NVARCHAR(20),
    @EstablishmentId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @UserId INT;
        DECLARE @CurrentCapacity INT;
        DECLARE @MaxCapacity INT;
        DECLARE @EstablishmentOpen BIT;
        
        -- Check if establishment is open
        SELECT @EstablishmentOpen = COUNT(*)
        FROM EstablishmentOpenings 
        WHERE EstablishmentId = @EstablishmentId 
            AND Status = 'Open'
            AND CAST(OpeningDateTime AS DATE) = CAST(GETDATE() AS DATE);
        
        IF @EstablishmentOpen = 0
        BEGIN
            SET @Result = 0;
            SET @Message = 'Establishment is not open';
            ROLLBACK;
            RETURN;
        END
        
        -- Get current capacity
        SELECT @CurrentCapacity = COUNT(*)
        FROM AccessRecords 
        WHERE EstablishmentId = @EstablishmentId 
            AND ExitDateTime IS NULL;
        
        -- Get max capacity (NULL means no limit)
        SELECT @MaxCapacity = MaxCapacity 
        FROM Establishments 
        WHERE Id = @EstablishmentId;
        
        -- Check capacity only if MaxCapacity is not NULL
        IF @MaxCapacity IS NOT NULL AND @CurrentCapacity >= @MaxCapacity
        BEGIN
            SET @Result = 0;
            SET @Message = 'Maximum capacity reached';
            ROLLBACK;
            RETURN;
        END
        
        -- Find user by identity document
        SELECT @UserId = Id 
        FROM Users 
        WHERE IdentityDocument = @IdentityDocument AND IsActive = 1;
        
        IF @UserId IS NULL
        BEGIN
            SET @Result = 0;
            SET @Message = 'User not found with this identity document';
            ROLLBACK;
            RETURN;
        END
        
        -- Check if user is already inside
        IF EXISTS (
            SELECT 1 FROM AccessRecords 
            WHERE UserId = @UserId 
                AND EstablishmentId = @EstablishmentId 
                AND ExitDateTime IS NULL
        )
        BEGIN
            SET @Result = 0;
            SET @Message = 'User is already inside the establishment';
            ROLLBACK;
            RETURN;
        END
        
        -- Register entry
        INSERT INTO AccessRecords (UserId, EstablishmentId, EntryDateTime)
        VALUES (@UserId, @EstablishmentId, GETDATE());
        
        SET @Result = 1;
        SET @Message = 'Entry registered successfully';
        
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @Result = 0;
        SET @Message = 'Error registering entry: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- SP for Register Exit
CREATE PROCEDURE sp_RegisterExit
    @IdentityDocument NVARCHAR(20),
    @EstablishmentId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @UserId INT;
        DECLARE @AccessRecordId BIGINT;
        
        -- Find user
        SELECT @UserId = Id 
        FROM Users 
        WHERE IdentityDocument = @IdentityDocument AND IsActive = 1;
        
        IF @UserId IS NULL
        BEGIN
            SET @Result = 0;
            SET @Message = 'User not found';
            ROLLBACK;
            RETURN;
        END
        
        -- Find entry record without exit
        SELECT TOP 1 @AccessRecordId = Id
        FROM AccessRecords 
        WHERE UserId = @UserId 
            AND EstablishmentId = @EstablishmentId 
            AND ExitDateTime IS NULL
        ORDER BY EntryDateTime DESC;
        
        IF @AccessRecordId IS NULL
        BEGIN
            SET @Result = 0;
            SET @Message = 'No entry record found for this user';
            ROLLBACK;
            RETURN;
        END
        
        -- Register exit
        UPDATE AccessRecords 
        SET ExitDateTime = GETDATE()
        WHERE Id = @AccessRecordId;
        
        SET @Result = 1;
        SET @Message = 'Exit registered successfully';
        
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @Result = 0;
        SET @Message = 'Error registering exit: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- SP for Get Current Capacity
CREATE PROCEDURE sp_GetCurrentCapacity
    @EstablishmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS CurrentCapacity,
        E.MaxCapacity,
        E.Name AS EstablishmentName
    FROM AccessRecords AR
    INNER JOIN Establishments E ON AR.EstablishmentId = E.Id
    WHERE AR.EstablishmentId = @EstablishmentId 
        AND AR.ExitDateTime IS NULL
    GROUP BY E.MaxCapacity, E.Name;
END
GO

-- SP for Open Establishment
CREATE PROCEDURE sp_OpenEstablishment
    @EstablishmentId INT,
    @UserId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Check if already open
        IF EXISTS (
            SELECT 1 FROM EstablishmentOpenings 
            WHERE EstablishmentId = @EstablishmentId 
                AND Status = 'Open'
                AND CAST(OpeningDateTime AS DATE) = CAST(GETDATE() AS DATE)
        )
        BEGIN
            SET @Result = 0;
            SET @Message = 'Establishment is already open';
            ROLLBACK;
            RETURN;
        END
        
        -- Register opening
        INSERT INTO EstablishmentOpenings (EstablishmentId, UserId, OpeningDateTime, Status)
        VALUES (@EstablishmentId, @UserId, GETDATE(), 'Open');
        
        SET @Result = 1;
        SET @Message = 'Establishment opened successfully';
        
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @Result = 0;
        SET @Message = 'Error opening establishment: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- SP for Close Establishment
CREATE PROCEDURE sp_CloseEstablishment
    @EstablishmentId INT,
    @UserId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @OpeningId INT;
        
        -- Find active opening
        SELECT TOP 1 @OpeningId = Id
        FROM EstablishmentOpenings 
        WHERE EstablishmentId = @EstablishmentId 
            AND Status = 'Open'
            AND CAST(OpeningDateTime AS DATE) = CAST(GETDATE() AS DATE)
        ORDER BY OpeningDateTime DESC;
        
        IF @OpeningId IS NULL
        BEGIN
            SET @Result = 0;
            SET @Message = 'No active opening found for today';
            ROLLBACK;
            RETURN;
        END
        
        -- Register closing
        UPDATE EstablishmentOpenings 
        SET ClosingDateTime = GETDATE(),
            Status = 'Closed'
        WHERE Id = @OpeningId;
        
        SET @Result = 1;
        SET @Message = 'Establishment closed successfully';
        
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @Result = 0;
        SET @Message = 'Error closing establishment: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- SP for Create User
CREATE PROCEDURE sp_CreateUser
    @Email NVARCHAR(100),
    @Password NVARCHAR(255),
    @FullName NVARCHAR(100),
    @IdentityDocument NVARCHAR(20),
    @PhoneNumber NVARCHAR(20),
    @EstablishmentId INT,
    @Role NVARCHAR(20),
    @CreatedBy INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Check if email already exists
        IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        BEGIN
            SET @Result = 0;
            SET @Message = 'Email already registered';
            ROLLBACK;
            RETURN;
        END
        
        -- Check if identity document already exists
        IF EXISTS (SELECT 1 FROM Users WHERE IdentityDocument = @IdentityDocument AND IsActive = 1)
        BEGIN
            SET @Result = 0;
            SET @Message = 'Identity document already registered';
            ROLLBACK;
            RETURN;
        END
        
        -- Check establishment
        IF NOT EXISTS (SELECT 1 FROM Establishments WHERE Id = @EstablishmentId AND IsActive = 1)
        BEGIN
            SET @Result = 0;
            SET @Message = 'Invalid establishment';
            ROLLBACK;
            RETURN;
        END
        
        -- Insert user
        INSERT INTO Users (Email, Password, FullName, IdentityDocument, PhoneNumber, EstablishmentId, Role, MustChangePassword)
        VALUES (@Email, @Password, @FullName, @IdentityDocument, @PhoneNumber, @EstablishmentId, @Role, 1);
        
        SET @Result = 1;
        SET @Message = 'User created successfully';
        
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @Result = 0;
        SET @Message = 'Error creating user: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- SP for Get User Access History
CREATE PROCEDURE sp_GetUserAccessHistory
    @EstablishmentId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        U.FullName,
        U.IdentityDocument,
        AR.EntryDateTime,
        AR.ExitDateTime,
        DATEDIFF(MINUTE, AR.EntryDateTime, AR.ExitDateTime) AS DurationMinutes
    FROM AccessRecords AR
    INNER JOIN Users U ON AR.UserId = U.Id
    WHERE AR.EstablishmentId = @EstablishmentId
        AND CAST(AR.EntryDateTime AS DATE) BETWEEN @StartDate AND @EndDate
    ORDER BY AR.EntryDateTime DESC;
END
GO

-- SP for Get Hourly Averages
CREATE PROCEDURE sp_GetHourlyAverages
    @EstablishmentId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH HourlyData AS (
        SELECT 
            DATEPART(HOUR, EntryDateTime) AS Hour,
            COUNT(*) AS Entries,
            CAST(COUNT(*) AS FLOAT) / COUNT(DISTINCT CAST(EntryDateTime AS DATE)) AS AvgEntries
        FROM AccessRecords
        WHERE EstablishmentId = @EstablishmentId
            AND CAST(EntryDateTime AS DATE) BETWEEN @StartDate AND @EndDate
        GROUP BY DATEPART(HOUR, EntryDateTime)
    )
    SELECT 
        Hour,
        Entries,
        ROUND(AvgEntries, 2) AS AverageEntriesPerDay
    FROM HourlyData
    ORDER BY Hour;
END
GO

-- TRIGGERS

CREATE TRIGGER tr_UserAudit
ON Users
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT;

    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        -- UPDATE
        SELECT @UserId = Id FROM inserted;
        PRINT 'User updated: ' + CONVERT(NVARCHAR(10), @UserId);
    END
    ELSE
    BEGIN
        -- DELETE  
        SELECT @UserId = Id FROM deleted;
        PRINT 'User deleted: ' + CONVERT(NVARCHAR(10), @UserId);
    END
END
GO


CREATE TRIGGER tr_ValidateCapacity
ON AccessRecords
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @EstablishmentId INT;
    DECLARE @CurrentCapacity INT;
    DECLARE @MaxCapacity INT;
    
    SELECT @EstablishmentId = EstablishmentId FROM inserted;
    
    -- Get current capacity
    SELECT @CurrentCapacity = COUNT(*)
    FROM AccessRecords 
    WHERE EstablishmentId = @EstablishmentId 
        AND ExitDateTime IS NULL;
    
    -- Get max capacity
    SELECT @MaxCapacity = MaxCapacity 
    FROM Establishments 
    WHERE Id = @EstablishmentId;
    
    -- Only validate if MaxCapacity is not NULL
    IF @MaxCapacity IS NOT NULL AND @CurrentCapacity > @MaxCapacity
    BEGIN
        RAISERROR('Maximum capacity exceeded', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE TRIGGER tr_PreventDuplicateIdentityDocument
ON Users
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1 
        FROM Users U
        INNER JOIN inserted I ON U.IdentityDocument = I.IdentityDocument AND U.Id != I.Id
        WHERE U.IsActive = 1 AND I.IsActive = 1
    )
    BEGIN
        RAISERROR('Identity document already exists for another active user', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

PRINT 'All stored procedures and triggers created successfully.';