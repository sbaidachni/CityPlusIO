CREATE TABLE [dbo].[Person]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	[Location] geography NOT NULL, 
    [Channel] NVARCHAR(50) NULL, 
    [AccountID] NVARCHAR(255) NULL, 
    [AccountName] NVARCHAR(255) NULL, 
    [ClientNotificationUri] NVARCHAR(MAX) NULL 
)