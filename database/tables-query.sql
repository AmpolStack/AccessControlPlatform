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
    Password NVARCHAR(255) NOT NULL, -- BCrypt hash
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
    FOREIGN KEY (EstablishmentId) REFERENCES Establishments(Id),
    FOREIGN KEY (RegisteredByUserId) REFERENCES Users(Id)
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
