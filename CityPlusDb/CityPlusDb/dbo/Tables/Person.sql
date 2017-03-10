CREATE TABLE [dbo].[Person] (
    [Id]                    BIGINT            IDENTITY (1, 1) NOT NULL,
    [Location]              [sys].[geography] NOT NULL,
    [Channel]               NVARCHAR (50)     NULL,
    [AccountID]             NVARCHAR (255)    NULL,
    [AccountName]           NVARCHAR (255)    NULL,
    [ClientNotificationUri] NVARCHAR (MAX)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

