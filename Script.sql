USE master
GO
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TaskManagement_DEV')
DROP DATABASE TaskManagement_DEV;
GO
CREATE DATABASE TaskManagement_DEV;
GO
USE TaskManagement_DEV;
GO

-- Create tables
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(300) NOT NULL,
    Role INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION
);

CREATE TABLE Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status INT NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

CREATE TABLE TaskHistories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status INT NOT NULL,
    UpdatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
    FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);

CREATE TABLE TaskComments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CommentText NVARCHAR(1000),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Message NVARCHAR(500),
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id)
);

CREATE TABLE UserTasks (
    Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    PRIMARY KEY (UserId, TaskId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id)
);

-- Create indexes
CREATE NONCLUSTERED INDEX IX_UserTasks_UserId ON UserTasks(UserId);
CREATE NONCLUSTERED INDEX IX_UserTasks_TaskId ON UserTasks(TaskId);
CREATE NONCLUSTERED INDEX IX_Tasks_CreatedBy ON Tasks(CreatedBy);
CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE NONCLUSTERED INDEX IX_Notifications_TaskId ON Notifications(TaskId);
CREATE NONCLUSTERED INDEX IX_Comments_TaskId ON TaskComments(TaskId);
CREATE NONCLUSTERED INDEX IX_Comments_UserId ON TaskComments(UserId);
CREATE NONCLUSTERED INDEX IX_History_TaskId ON TaskHistories(TaskId);
CREATE NONCLUSTERED INDEX IX_History_UpdatedBy ON TaskHistories(UpdatedBy);

-- Create stored procedures
GO
CREATE OR ALTER PROCEDURE sp_GetUserTasks
    @UserId UNIQUEIDENTIFIER,
    @Status INT = NULL,
    @Title NVARCHAR(200) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.*
    FROM Tasks t
    INNER JOIN UserTasks ut ON ut.TaskId = t.Id
    WHERE ut.UserId = @UserId
      AND (@Status IS NULL OR t.Status = @Status)
      AND (@Title IS NULL OR t.Title = @Title)
      AND (@Description IS NULL OR t.Description = @Description)
    ORDER BY t.CreatedAt DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;

GO
CREATE OR ALTER PROCEDURE sp_GetTaskFullHistory
    @TaskId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Timeline (
        TaskId UNIQUEIDENTIFIER,
        DateModified DATETIME2,
        Status INT NULL,
        CommentText NVARCHAR(1000) NULL,
        NotificationMessage NVARCHAR(500) NULL
    );

    INSERT INTO #Timeline (TaskId, DateModified, Status, CommentText, NotificationMessage)
    SELECT TaskId, ISNULL(UpdatedAt, CreatedAt), Status, NULL, NULL
    FROM TaskHistories
    WHERE TaskId = @TaskId;

    INSERT INTO #Timeline (TaskId, DateModified, Status, CommentText, NotificationMessage)
    SELECT TaskId, ISNULL(UpdatedAt, CreatedAt), NULL, CommentText, NULL
    FROM TaskComments
    WHERE TaskId = @TaskId;

    INSERT INTO #Timeline (TaskId, DateModified, Status, CommentText, NotificationMessage)
    SELECT TaskId, ISNULL(UpdatedAt, CreatedAt), NULL, NULL, Message
    FROM Notifications
    WHERE TaskId = @TaskId;

    SELECT *
    FROM #Timeline
    ORDER BY DateModified;

    DROP TABLE #Timeline;
END;

GO
CREATE OR ALTER PROCEDURE sp_GetTaskContributors
    @TaskId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT u.*
    FROM Users u
    WHERE u.Id IN (
        SELECT UserId FROM TaskComments WHERE TaskId = @TaskId
        UNION
        SELECT UpdatedBy FROM TaskHistories WHERE TaskId = @TaskId
    );
END;
