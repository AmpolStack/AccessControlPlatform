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