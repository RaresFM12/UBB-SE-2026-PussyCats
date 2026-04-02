-- =============================================
-- PussyCatsApp Database Schema
-- Based on DB diagram: USERS, SKILLS, DOCUMENTS, PREFERENCES
-- =============================================

CREATE DATABASE PussyCatsDB;
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
    disabilities    BIT             NULL DEFAULT 0,
    parsedCV        VARCHAR(MAX)    NULL,
    personalityTestResult VARCHAR(500) NULL,
    activeAccount   BIT             NOT NULL DEFAULT 1,
    profilePicture  VARCHAR(500)    NULL,
    university      VARCHAR(200)    NULL,
    degree          VARCHAR(200)    NULL,
    universityStartYear SMALLINT    NULL,
    city            NVARCHAR(200)   NULL,
    LastUpdated     DATETIME        NULL DEFAULT GETDATE(),
    formDataJson    VARCHAR(MAX)    NULL
);
GO

SELECT * FROM users

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

-- ============================================
-- MATCHES table (FK -> USERS)
-- ============================================

CREATE TABLE MATCHES (
    id INT PRIMARY KEY IDENTITY(1,1),
    userID INT NOT NULL,
    companyName NVARCHAR(255) NOT NULL,
    jobRole NVARCHAR(255) NOT NULL,
    matchDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Matches_Users FOREIGN KEY (userID) REFERENCES USERS(userID) ON DELETE CASCADE
);
GO

-- =============================================
-- Updates
-- =============================================

ALTER TABLE Users
ADD degree VARCHAR(200) NULL;

ALTER TABLE Users
ADD universityStartYear SMALLINT NULL;

ALTER TABLE Users ADD LastUpdated DATETIME NULL;
GO
UPDATE Users SET LastUpdated = GETDATE() WHERE LastUpdated IS NULL;
GO

ALTER TABLE Users 
ADD city VARCHAR(100) NULL;

ALTER TABLE Users
ADD motivation VARCHAR(1000) NULL;

-- First, drop the default constraint on the score column if it exists
-- (You may need to find the constraint name if it was auto-generated)
-- Example for SQL Server:
DECLARE @constraintName NVARCHAR(200);
SELECT @constraintName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON c.default_object_id = dc.object_id
WHERE c.object_id = OBJECT_ID('SKILLS') AND c.name = 'score';

IF @constraintName IS NOT NULL
    EXEC('ALTER TABLE SKILLS DROP CONSTRAINT ' + @constraintName);

-- Alter the column type from FLOAT to INT
ALTER TABLE SKILLS
ALTER COLUMN score INT NULL;

-- Optionally, set a new default if needed
ALTER TABLE SKILLS
ADD CONSTRAINT DF_Skills_Score DEFAULT 0 FOR score;

ALTER TABLE SKILLS
ALTER COLUMN achievedDate DATETIME NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('DOCUMENTS') AND name = 'FilePath'
)
BEGIN
    ALTER TABLE DOCUMENTS ADD FilePath VARCHAR(MAX) NULL;
END
GO
 
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('DOCUMENTS') AND name = 'UploadDate'
)
BEGIN
    ALTER TABLE DOCUMENTS ADD UploadDate DATETIME NULL;
END
GO

ALTER TABLE Users
ADD formDataJson VARCHAR(MAX) NULL;

-- =============================================
-- MATCHES table (FK -> USERS)
-- =============================================
CREATE TABLE MATCHES (
    id              INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    companyName     NVARCHAR(255)   NOT NULL,
    jobRole         NVARCHAR(255)   NOT NULL,
    matchDate       DATETIME        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Matches_Users FOREIGN KEY (userID)
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
CREATE INDEX IX_Matches_UserID       ON MATCHES(userID);
CREATE INDEX IX_Users_Email          ON USERS(email);
GO

-- =============================================
-- Sample data insertion (optional)
-- =============================================
INSERT INTO USERS (
    firstName, 
    lastName, 
    gender, 
    age, 
    email, 
    phone, 
    github, 
    linkedin, 
    graduationYear, 
    country, 
    address, 
    university, 
    parsedCV
)
VALUES (
    'Ioana', 
    'Gavrila', 
    'Female', 
    22, 
    'ioana.gavrila@student.ubbcluj.ro', 
    '+40723456789', 
    'github.com/ioanagavrila', 
    'linkedin.com/in/ioanagavrila', 
    2026, 
    'Romania', 
    'Str. Universitatii, Nr. 1, Cluj-Napoca', 
    'Babes-Bolyai University', 
    '{"city": "Cluj-Napoca", "degree": "Faculty of Computer Science", "universityStartYear": 2023, "motivation": "I am passionate about software development and eager to contribute to innovative projects. My focus is on creating user-friendly applications with clean, maintainable code.", "workExperiences": [{"company": "Tech Solutions SRL", "jobTitle": "Junior Software Developer", "startDate": "2024-06-01", "endDate": null, "currentlyWorking": true, "description": "Developing and maintaining web applications using ASP.NET Core and React. Participating in code reviews and contributing to architectural decisions."}, {"company": "StartUp Hub", "jobTitle": "Software Development Intern", "startDate": "2023-07-01", "endDate": "2023-09-30", "currentlyWorking": false, "description": "Worked on a mobile application using React Native. Implemented features for user authentication and data synchronization."}], "projects": [{"name": "PussyCats User Management System", "description": "A comprehensive user profile management system built with WinUI 3 and C#. Features include CV parsing, skill assessment, and profile completeness tracking.", "technologies": ["C#", "WinUI 3", "SQL Server", "Entity Framework"], "url": "github.com/ioanagavrila/pussycats"}, {"name": "E-Commerce Platform", "description": "Full-stack e-commerce solution with shopping cart, payment integration, and admin dashboard.", "technologies": ["React", "Node.js", "MongoDB", "Stripe API"], "url": "github.com/ioanagavrila/ecommerce"}, {"name": "Task Management Mobile App", "description": "Cross-platform mobile application for task management with real-time synchronization.", "technologies": ["React Native", "Firebase", "Redux"], "url": "github.com/ioanagavrila/taskmanager"}], "extraCurricularActivities": [{"activityName": "Google Developer Student Club", "organization": "Babes-Bolyai University", "role": "Technical Lead", "period": "2024 - Present", "description": "Organizing workshops and hackathons for students interested in technology."}, {"activityName": "Coding Competition", "organization": "ACM ICPC", "role": "Participant", "period": "2023 - 2024", "description": "Competed in regional programming contests, focusing on algorithm optimization."}]}'
);

UPDATE USERS 
SET degree = 'Bachelor of Science in Computer Science', universityStartYear = 2023
WHERE userID = 1;

UPDATE USERS
SET 
    github = 'https://github.com/RaresFM12',
    linkedin = 'https://www.linkedin.com/in/rare%C8%99fodorca/'
WHERE userID = 1;


DECLARE @newUserId INT = SCOPE_IDENTITY();

INSERT INTO SKILLS (name, userID)
VALUES 
    ('C#', @newUserId),
    ('JavaScript', @newUserId),
    ('React', @newUserId),
    ('ASP.NET Core', @newUserId),
    ('SQL Server', @newUserId),
    ('Git', @newUserId),
    ('Docker', @newUserId),
    ('Azure', @newUserId),
    ('WinUI 3', @newUserId),
    ('Entity Framework', @newUserId),
    ('REST API', @newUserId),
    ('Agile/Scrum', @newUserId);
GO

INSERT INTO DOCUMENTS (userID, storedDocument, nameDocument)
VALUES 
    (@newUserId, 'https://example.com/cv/ioana_gavrila_cv.pdf', 'Ioana_Gavrila_CV.pdf');


--set the last modified to check if the update works
UPDATE USERS
SET LastUpdated = DATEADD(day, -5, GETDATE())
WHERE userID = 1;

-- =============================================
-- Insert Sample Data into MATCHES
-- =============================================

-- Using explicit dates (e.g., matching over the last few days)
INSERT INTO MATCHES (userID, companyName, jobRole, matchDate)
VALUES 
    (1, 'Tech Innovators Cluj', 'Junior .NET Developer', DATEADD(day, -5, GETDATE())),
    (1, 'Global Software Corp', 'Full Stack Engineer (React/C#)', DATEADD(day, -2, GETDATE())),
    (1, 'FinTech Solutions SRL', 'Frontend Developer', GETDATE());
GO

-- Relying on the DEFAULT constraint for matchDate (will use exactly GETDATE())
INSERT INTO MATCHES(userID, companyName, jobRole)
VALUES 
    (1, 'NextGen AI Startups', 'Software Integration Engineer'),
    (1, 'E-Commerce Giants Ltd.', 'Backend Developer (ASP.NET)');
GO

-- Verify the insertions
SELECT * FROM MATCHES;
GO

SELECT * FROM USERS;
SELECT * FROM documents;
SELECT  * FROM Matches;
SELECT * FROM SKILLS;
