CREATE DATABASE UserManagementDB;
GO

USE UserManagementDB;
GO

CREATE TABLE Users (
    userID                INT             NOT NULL IDENTITY,
    firstName             NVARCHAR(100)   NOT NULL,
    lastName              NVARCHAR(100)   NOT NULL,
    gender                CHAR(1),
    age                   INT,
    email                 NVARCHAR(255)   NOT NULL,
    phone                 NVARCHAR(50),
    github                NVARCHAR(255),
    linkedin              NVARCHAR(255),
    graduationYear        SMALLINT,
    country               NVARCHAR(100),
    address               NVARCHAR(500),
    sexualOrientation     NVARCHAR(100),
    disabilities          BIT             NOT NULL DEFAULT 0,
    parsedCV              NVARCHAR(1000),
    personalityTestResult NVARCHAR(255),
    activeAccount         BIT             NOT NULL DEFAULT 1,
    profilePicture        NVARCHAR(500),
    university            NVARCHAR(255),
    CONSTRAINT PK_users PRIMARY KEY (userID),
    CONSTRAINT UQ_users_email UNIQUE (email)
);
GO


CREATE TABLE Skills (
    skillID     INT             NOT NULL IDENTITY,
    name        NVARCHAR(255)   NOT NULL,
    score       FLOAT,
    userID      INT             NOT NULL,
    CONSTRAINT PK_skills PRIMARY KEY (skillID),
    CONSTRAINT FK_skills_user FOREIGN KEY (userID)
        REFERENCES users (userID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO


CREATE TABLE Documents (
    dID             INT             NOT NULL IDENTITY,
    userID          INT             NOT NULL,
    storedDocument  NVARCHAR(1000),
    nameDocument    NVARCHAR(255),
    CONSTRAINT PK_documents PRIMARY KEY (dID),
    CONSTRAINT FK_documents_user FOREIGN KEY (userID)
        REFERENCES users (userID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

CREATE TABLE Preferences (
    pID             INT             NOT NULL IDENTITY,
    userID          INT             NOT NULL,
    preferanceType  NVARCHAR(255),
    value           NVARCHAR(255),
    CONSTRAINT PK_preferences PRIMARY KEY (pID),
    CONSTRAINT FK_preferences_user FOREIGN KEY (userID)
        REFERENCES users (userID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO