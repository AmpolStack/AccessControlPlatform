
-- User Login Sp
CREATE PROCEDURE sp_UserLogin
    @Email NVARCHAR(100),
    @Password NVARCHAR(100),
    @Success BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
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
    WHERE Email = @Email AND Password = @Password;
    
    IF @UserId IS NULL
    BEGIN
        SET @Success = 0;
        SET @Message = 'User not found';
        GOTO FinalSelect;
    END
    
    IF @IsActive = 0
    BEGIN
        SET @Success = 0;
        SET @Message = 'User inactive';
        GOTO FinalSelect;
    END

    IF EXISTS (SELECT 1 FROM AccessRecords WHERE UserId = @UserId AND ExitDateTime IS NULL)
    BEGIN
        SET @Success = 0;
        SET @Message = 'hay una sesión abierta en este perfil';
        GOTO FinalSelect;
    END
    
    IF @StoredHash IS NULL
    BEGIN
        SET @Success = 0;
        SET @Message = 'Invalid credentials';
        GOTO FinalSelect;
    END

    UPDATE Users 
    SET LastLoginDate = GETDATE() 
    WHERE Id = @UserId;
    
    SET @Success = 1;
    SET @Message = 'Login successful';
    
FinalSelect:
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
        E.MaxCapacity
    FROM Users U
    INNER JOIN Establishments E ON U.EstablishmentId = E.Id
    WHERE U.Id = @UserId AND @Success = 1;
END
GO 

-- 2. SP for Register Entry
CREATE PROCEDURE sp_RegisterEntry
    @UserId INT,
    @EstablishmentId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @CurrentCapacity INT;
        DECLARE @MaxCapacity INT;
        DECLARE @EstablishmentOpen BIT;


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
        

        SELECT @CurrentCapacity = COUNT(*)
        FROM AccessRecords 
        WHERE EstablishmentId = @EstablishmentId 
          AND ExitDateTime IS NULL;
        

        SELECT @MaxCapacity = MaxCapacity 
        FROM Establishments 
        WHERE Id = @EstablishmentId;

        IF @MaxCapacity IS NOT NULL AND @CurrentCapacity >= @MaxCapacity
        BEGIN
            SET @Result = 0;
            SET @Message = 'Maximum capacity reached';
            ROLLBACK;
            RETURN;
        END
        

        IF NOT EXISTS (
            SELECT 1 FROM Users 
            WHERE Id = @UserId AND IsActive = 1
        )
        BEGIN
            SET @Result = 0;
            SET @Message = 'User not found or inactive';
            ROLLBACK;
            RETURN;
        END
        

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

-- 3. SP for Register Exit
CREATE PROCEDURE sp_RegisterExit
    @UserId INT,
    @EstablishmentId INT,
    @Result BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @AccessRecordId BIGINT;

        IF NOT EXISTS (
            SELECT 1 FROM Users 
            WHERE Id = @UserId AND IsActive = 1
        )
        BEGIN
            SET @Result = 0;
            SET @Message = 'User not found or inactive';
            ROLLBACK;
            RETURN;
        END


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


-- 4. SP for Get Current Capacity
CREATE PROCEDURE sp_GetCurrentCapacity
    @EstablishmentId INT,
    @Success BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate establishment exists
    IF NOT EXISTS (SELECT 1 FROM Establishments WHERE Id = @EstablishmentId)
    BEGIN
        SET @Success = 0;
        SET @Message = 'Establishment not found';
        RETURN;
    END
    
    SET @Success = 1;
    SET @Message = 'Capacity retrieved successfully';
    
    -- Return capacity data
    SELECT 
        ISNULL(COUNT(*), 0) AS CurrentCapacity,
        E.MaxCapacity,
        E.Name AS EstablishmentName
    FROM Establishments E
    LEFT JOIN AccessRecords AR ON AR.EstablishmentId = E.Id AND AR.ExitDateTime IS NULL
    WHERE E.Id = @EstablishmentId
    GROUP BY E.MaxCapacity, E.Name;
END
GO

-- 5. SP for Open Establishment
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

-- 6. SP for Close Establishment
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

-- 7. SP for Create User
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

-- 8. SP for Get User Access History
CREATE PROCEDURE sp_GetUserAccessHistory
    @EstablishmentId INT,
    @StartDate DATE,
    @EndDate DATE,
    @Success BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate establishment exists
    IF NOT EXISTS (SELECT 1 FROM Establishments WHERE Id = @EstablishmentId)
    BEGIN
        SET @Success = 0;
        SET @Message = 'Establishment not found';
        RETURN;
    END
    
    -- Validate date range
    IF @StartDate > @EndDate
    BEGIN
        SET @Success = 0;
        SET @Message = 'Start date cannot be greater than end date';
        RETURN;
    END
    
    SET @Success = 1;
    SET @Message = 'Access history retrieved successfully';
    
    -- Return access history
    SELECT 
        U.FullName,
        U.IdentityDocument,
        AR.EntryDateTime,
        AR.ExitDateTime,
        DATEDIFF(MINUTE, AR.EntryDateTime, ISNULL(AR.ExitDateTime, GETDATE())) AS DurationMinutes
    FROM AccessRecords AR
    INNER JOIN Users U ON AR.UserId = U.Id
    WHERE AR.EstablishmentId = @EstablishmentId
        AND CAST(AR.EntryDateTime AS DATE) BETWEEN @StartDate AND @EndDate
    ORDER BY AR.EntryDateTime DESC;
END
GO

-- 9. SP for Get Hourly Averages
CREATE PROCEDURE sp_GetHourlyAverages
    @EstablishmentId INT,
    @StartDate DATE,
    @EndDate DATE,
    @Success BIT OUTPUT,
    @Message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate establishment exists
    IF NOT EXISTS (SELECT 1 FROM Establishments WHERE Id = @EstablishmentId)
    BEGIN
        SET @Success = 0;
        SET @Message = 'Establishment not found';
        RETURN;
    END
    
    -- Validate date range
    IF @StartDate > @EndDate
    BEGIN
        SET @Success = 0;
        SET @Message = 'Start date cannot be greater than end date';
        RETURN;
    END
    
    SET @Success = 1;
    SET @Message = 'Hourly averages retrieved successfully';
    
    -- Return hourly data
    WITH HourlyData AS (
        SELECT 
            DATEPART(HOUR, EntryDateTime) AS Hour,
            COUNT(*) AS Entries,
            CAST(COUNT(*) AS FLOAT) / NULLIF(COUNT(DISTINCT CAST(EntryDateTime AS DATE)), 0) AS AvgEntries
        FROM AccessRecords
        WHERE EstablishmentId = @EstablishmentId
            AND CAST(EntryDateTime AS DATE) BETWEEN @StartDate AND @EndDate
        GROUP BY DATEPART(HOUR, EntryDateTime)
    )
    SELECT 
        Hour,
        Entries,
        ROUND(ISNULL(AvgEntries, 0), 2) AS AverageEntriesPerDay
    FROM HourlyData
    ORDER BY Hour;
END
GO
