USE PussyCatsDB;
GO

-- =============================================
-- Drop tables in FK-safe order
-- =============================================
IF OBJECT_ID('MATCHES',     'U') IS NOT NULL DROP TABLE MATCHES;
IF OBJECT_ID('SKILLS',      'U') IS NOT NULL DROP TABLE SKILLS;
IF OBJECT_ID('DOCUMENTS',   'U') IS NOT NULL DROP TABLE DOCUMENTS;
IF OBJECT_ID('PREFERENCES', 'U') IS NOT NULL DROP TABLE PREFERENCES;
IF OBJECT_ID('USERS',       'U') IS NOT NULL DROP TABLE USERS;
GO

-- =============================================
-- USERS table (all columns included upfront)
-- =============================================
CREATE TABLE USERS (
    userID                  INT             PRIMARY KEY IDENTITY(1,1),
    firstName               VARCHAR(50)     NOT NULL,
    lastName                VARCHAR(60)     NOT NULL,
    gender                  CHAR(6)         NOT NULL,
    age                     INT             NOT NULL CHECK (age >= 16 AND age <= 60),
    email                   VARCHAR(254)    NOT NULL UNIQUE,
    phone                   VARCHAR(20)     NULL,
    github                  VARCHAR(200)    NULL,
    linkedin                VARCHAR(200)    NULL,
    graduationYear          SMALLINT        NULL,
    country                 VARCHAR(100)    NULL,
    address                 VARCHAR(500)    NULL,
    disabilities            BIT             NULL DEFAULT 0,
    parsedCV                VARCHAR(MAX)    NULL,
    personalityTestResult   VARCHAR(500)    NULL,
    activeAccount           BIT             NOT NULL DEFAULT 1,
    profilePicture          VARCHAR(500)    NULL,
    university              VARCHAR(200)    NULL,
    degree                  VARCHAR(200)    NULL,
    universityStartYear     SMALLINT        NULL,
    city                    NVARCHAR(200)   NULL,
    LastUpdated             DATETIME        NULL DEFAULT GETDATE(),
    formDataJson            VARCHAR(MAX)    NULL,
    motivation              VARCHAR(1000)   NULL        -- was missing from original CREATE
);
GO

-- =============================================
-- SKILLS table
-- =============================================
CREATE TABLE SKILLS (
    skillID         INT             PRIMARY KEY IDENTITY(1,1),
    name            VARCHAR(60)     NOT NULL,
    score           INT             NULL DEFAULT 0,     -- already INT, no ALTER needed
    userID          INT             NOT NULL,
    achievedDate    DATETIME        NULL,               -- already DATETIME, no ALTER needed
    CONSTRAINT FK_Skills_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID) ON DELETE CASCADE
);
GO

-- =============================================
-- DOCUMENTS table
-- =============================================
CREATE TABLE DOCUMENTS (
    dID             INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    storedDocument  VARCHAR(MAX)    NULL,
    nameDocument    VARCHAR(255)    NOT NULL,
    FilePath        VARCHAR(MAX)    NULL,               -- included directly
    UploadDate      DATETIME        NULL,               -- included directly
    CONSTRAINT FK_Documents_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID) ON DELETE CASCADE
);
GO

-- =============================================
-- PREFERENCES table
-- =============================================
CREATE TABLE PREFERENCES (
    pID             INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    preferanceType  VARCHAR(100)    NOT NULL,
    value           VARCHAR(500)    NOT NULL,
    CONSTRAINT FK_Preferences_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID) ON DELETE CASCADE
);
GO

-- =============================================
-- MATCHES table (defined only once)
-- =============================================
CREATE TABLE MATCHES (
    id              INT             PRIMARY KEY IDENTITY(1,1),
    userID          INT             NOT NULL,
    companyName     NVARCHAR(255)   NOT NULL,
    jobRole         NVARCHAR(255)   NOT NULL,
    matchDate       DATETIME        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Matches_Users FOREIGN KEY (userID)
        REFERENCES USERS(userID) ON DELETE CASCADE
);
GO

-- =============================================
-- Indexes
-- =============================================
CREATE INDEX IX_Skills_UserID       ON SKILLS(userID);
CREATE INDEX IX_Documents_UserID    ON DOCUMENTS(userID);
CREATE INDEX IX_Preferences_UserID  ON PREFERENCES(userID);
CREATE INDEX IX_Matches_UserID      ON MATCHES(userID);
CREATE INDEX IX_Users_Email         ON USERS(email);
GO

-- =============================================
-- Insert User 1 (Ioana Gavrila - userID will be 1)
-- =============================================
INSERT INTO USERS (
    firstName, lastName, gender, age, email, phone,
    github, linkedin, graduationYear, country, address,
    disabilities, parsedCV, personalityTestResult, activeAccount,
    profilePicture, university, degree, universityStartYear,
    LastUpdated, formDataJson, city, motivation
)
VALUES (
    'Ioana', 'Gavrila', 'Female', 22,
    'ioana.gavrila@student.ubbcluj.ro', '+40723456789',
    'github.com/ioanagavrila', 'linkedin.com/in/ioanagavrila',
    2026, 'Romania', 'Str. Universitatii, Nr. 1, Cluj-Napoca',
    0,
    '{"city":"Cluj-Napoca","degree":"Faculty of Computer Science","universityStartYear":2023,"motivation":"I am passionate about software development and eager to contribute to innovative projects.","workExperiences":[{"company":"Tech Solutions SRL","jobTitle":"Junior Software Developer","startDate":"2024-06-01","endDate":null,"currentlyWorking":true,"description":"Developing and maintaining web applications using ASP.NET Core and React."},{"company":"StartUp Hub","jobTitle":"Software Development Intern","startDate":"2023-07-01","endDate":"2023-09-30","currentlyWorking":false,"description":"Worked on a mobile application using React Native."}],"projects":[{"name":"PussyCats User Management System","description":"A comprehensive user profile management system built with WinUI 3 and C#.","technologies":["C#","WinUI 3","SQL Server","Entity Framework"],"url":"github.com/ioanagavrila/pussycats"},{"name":"E-Commerce Platform","description":"Full-stack e-commerce solution with shopping cart, payment integration, and admin dashboard.","technologies":["React","Node.js","MongoDB","Stripe API"],"url":"github.com/ioanagavrila/ecommerce"},{"name":"Task Management Mobile App","description":"Cross-platform mobile application for task management with real-time synchronization.","technologies":["React Native","Firebase","Redux"],"url":"github.com/ioanagavrila/taskmanager"}],"extraCurricularActivities":[{"activityName":"Google Developer Student Club","organization":"Babes-Bolyai University","role":"Technical Lead","period":"2024 - Present"},{"activityName":"Coding Competition","organization":"ACM ICPC","role":"Participant","period":"2023 - 2024"}]}',
    NULL, 1,
    NULL,
    'Babes-Bolyai University',
    'Bachelor of Science in Computer Science', 2023,
    '2026-04-02 14:32:29.793',
    '{"firstName":"Ioana","lastName":"Gavrila","age":22,"gender":"Female","email":"ioana.gavrila@student.ubbcluj.ro","phoneNumber":"+40723456789","gitHub":"github.com/ioanagavrila","linkedIn":"linkedin.com/in/ioanagavrila","country":"Romania","city":"Cluj-Napoca","university":"Babes-Bolyai University","degree":"Bachelor of Science in Computer Science","universityStartYear":2023,"expectedGraduationYear":2026,"address":"Str. Universitatii, Nr. 1, Cluj-Napoca","motivation":"I am passionate about software development and eager to contribute to innovative projects.","hasDisabilities":false,"skills":["HTML","JavaScript","CSS","TypeScript","React","Git","GitHub","Jest","Webpack","Vite","Figma","REST","SQL","PostgreSQL","Docker"],"workExperiences":[{"company":"Tech Solutions SRL","jobTitle":"Junior Software Developer","startDate":"2024-06-01T00:00:00","endDate":null,"description":"Developing and maintaining web applications using ASP.NET Core and React.","currentlyWorking":true},{"company":"StartUp Hub","jobTitle":"Software Development Intern","startDate":"2023-07-01T00:00:00","endDate":"2023-09-30T00:00:00","description":"Worked on a mobile application using React Native.","currentlyWorking":false}],"projects":[{"name":"PussyCats User Management System","description":"A comprehensive user profile management system built with WinUI 3 and C#.","technologies":["C#","WinUI 3","SQL Server","Entity Framework"],"url":"github.com/ioanagavrila/pussycats"},{"name":"E-Commerce Platform","description":"Full-stack e-commerce solution with shopping cart, payment integration, and admin dashboard.","technologies":["React","Node.js","MongoDB","Stripe API"],"url":"github.com/ioanagavrila/ecommerce"},{"name":"Task Management Mobile App","description":"Cross-platform mobile application for task management with real-time synchronization.","technologies":["React Native","Firebase","Redux"],"url":"github.com/ioanagavrila/taskmanager"}],"extraCurricularActivities":[{"activityName":"Google Developer Student Club","organization":"Babes-Bolyai University","role":"Technical Lead","period":"2024 - Present"},{"activityName":"Coding Competition","organization":"ACM ICPC","role":"Participant","period":"2023 - 2024"}]}',
    'Cluj-Napoca',
    'I am passionate about software development and eager to contribute to innovative projects. My focus is on creating user-friendly applications with clean, maintainable code.'
);
GO

-- =============================================
-- Insert User 2 (second Ioana row — userID will be 2)
-- Needed so the userID=5 skills inserts can work;
-- insert placeholder users 2-5 or reassign skills to userID=1.
-- Here we reassign the skills block to userID=1 since no real user 5 exists.
-- =============================================

-- =============================================
-- Documents for User 1
-- (use literal 1, not SCOPE_IDENTITY across a GO boundary)
-- =============================================
INSERT INTO DOCUMENTS (userID, storedDocument, nameDocument)
VALUES (1, 'https://example.com/cv/ioana_gavrila_cv.pdf', 'Ioana_Gavrila_CV.pdf');
GO

-- =============================================
-- Matches for User 1
-- =============================================
INSERT INTO MATCHES (userID, companyName, jobRole, matchDate)
VALUES
    (1, 'NextGen AI Startups',   'Backend Developer',  '2025-05-12'),
    (1, 'E-Commerce Giants Ltd.','Backend Developer',  '2025-06-28'),
    (1, 'Google',                'Frontend Developer', '2025-08-15'),
    (1, 'Amazon',                'AI/ML Engineer',     '2025-10-03'),
    (1, 'Microsoft',             'Frontend Developer', '2025-12-21'),
    (1, 'Cresta',                'Data Analyst',       '2026-01-30'),
    (1, 'UiPath',                'Backend Developer',  '2026-02-18'),
    (1, 'LSEG',                  'DevOps Engineer',    '2026-03-25');
GO

-- =============================================
-- Skills for User 1
-- =============================================
INSERT INTO SKILLS (name, score, userID, achievedDate) VALUES
    ('Tailwind',          88, 1, '2022-03-15'),
    ('SCSS',              84, 1, '2021-11-10'),
    ('Cypress',           79, 1, '2023-01-20'),
    ('Selenium',          70, 1, '2022-09-05'),
    ('Parcel',            73, 1, '2021-07-18'),
    ('Adobe XD',          87, 1, '2026-03-31'),
    ('Zeplin',            17, 1, '2026-03-31'),
    ('Storybook',         70, 1, '2026-03-31'),
    ('Google Analytics',  62, 1, '2026-03-31'),
    ('Hotjar',            45, 1, '2026-03-31'),
    ('Scrum',             80, 1, '2022-10-09'),
    ('Jira',              85, 1, '2021-12-03');
GO

-- =============================================
-- Skills originally for userID=5 —
-- no user 5 exists, so inserted under userID=1.
-- If a real second user is needed, insert them above and change to userID=2.
-- =============================================
INSERT INTO SKILLS (name, score, userID, achievedDate) VALUES
    ('PyTorch',              86, 1, '2023-02-17'),
    ('Keras',                81, 1, '2022-06-08'),
    ('NLTK',                 74, 1, '2022-11-21'),
    ('spaCy',                77, 1, '2023-03-09'),
    ('OpenCV',               69, 1, '2021-10-27'),
    ('FastAPI',              83, 1, '2023-05-16'),
    ('Hugging Face',         78, 1, '2024-01-12'),
    ('BigQuery',             65, 1, '2022-07-19'),
    ('Tableau',              72, 1, '2021-09-14'),
    ('Looker',               67, 1, '2023-08-22'),
    ('Hypothesis Testing',   80, 1, '2022-04-02'),
    ('Regression',           84, 1, '2021-12-29');
GO

-- =============================================
-- Verification
-- =============================================
SELECT * FROM USERS;
SELECT * FROM DOCUMENTS;
SELECT * FROM MATCHES;
SELECT * FROM SKILLS;
GO