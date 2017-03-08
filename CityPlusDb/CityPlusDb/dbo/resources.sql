CREATE TABLE [dbo].[resources]
(
	[id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [description] NVARCHAR(MAX) NOT NULL, 
    [quantity] INT NOT NULL DEFAULT 0
)
