-- =============================================
-- PussyCatsApp Database Schema
-- Based on DB diagram: USERS, SKILLS, DOCUMENTS, PREFERENCES
-- =============================================

CREATE DATABASE IF NOT EXISTS PussyCatsDB;
GO

USE PussyCatsDB;
GO

-- =============================================
-- USERS table
-- =============================================
CREATE TABLE USERS (
    userID          INT             PRIMARY KEY IDENTITY(1,1),
    firstName       VARCHAR(50)     NOT NULL,
    lastName        VARCHAR(60)     NOT NULL,
    gender          CHAR(6)         NOT NULL,           -- Male / Female
    age             INT             NOT NULL CHECK (age >= 16 AND age <= 60),
    email           VARCHAR(254)    NOT NULL UNIQUE,
    phone           VARCHAR(20)     NULL,
    github          VARCHAR(200)    NULL,
    linkedin        VARCHAR(200)    NULL,
    graduationYear  SMALLINT        NULL,
    country         VARCHAR(100)    NULL,
    address         VARCHAR(500)    NULL,
    sexualOrientation VARCHAR(50)   NULL,
    disabilities    BIT             NULL DEFAULT 0,
    parsedCV        VARCHAR(MAX)    NULL,
    personalityTestResult VARCHAR(500) NULL,
    activeAccount   BIT             NOT NULL DEFAULT 1,
    profilePicture  VARCHAR(500)    NULL,
    university      VARCHAR(200)    NULL
);
GO

-- =============================================
-- SKILLS table (FK -> USERS)
-- =============================================
CREATE TABLE SKILLS (
    skillID         INT             PRIMARY KEY IDENTITY(1,1),
    name            VARCHAR(60)     NOT NULL,
    score           FLOAT           NULL DEFAULT 0,
    userID          INT             NOT NULL,
    achievedDate    DATE            NULL,
    CONSTRAINT FK_Skills_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID)
        ON DELETE CASCADE
);
GO

-- =============================================
-- DOCUMENTS table (FK -> USERS)
-- =============================================
CREATE TABLE DOCUMENTS (
    dID             INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    storedDocument  VARCHAR(MAX)    NULL,
    nameDocument    VARCHAR(255)    NOT NULL,
    CONSTRAINT FK_Documents_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID)
        ON DELETE CASCADE
);
GO

-- =============================================
-- PREFERENCES table (FK -> USERS)
-- =============================================
CREATE TABLE PREFERENCES (
    pID             INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    preferanceType  VARCHAR(100)    NOT NULL,
    value           VARCHAR(500)    NOT NULL,
    CONSTRAINT FK_Preferences_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID)
        ON DELETE CASCADE
);
GO

-- =============================================
-- Indexes for common queries
-- =============================================
CREATE INDEX IX_Skills_UserID        ON SKILLS(userID);
CREATE INDEX IX_Documents_UserID     ON DOCUMENTS(userID);
CREATE INDEX IX_Preferences_UserID   ON PREFERENCES(userID);
CREATE INDEX IX_Users_Email          ON USERS(email);
GO
