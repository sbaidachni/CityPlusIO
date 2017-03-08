CREATE TABLE [dbo].[person]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[location] geography NOT NULL, 
    [channel] NVARCHAR(50) NULL
)