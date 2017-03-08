CREATE TABLE [dbo].[PersonAsked]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(MAX) NULL, 
    [Quantity] INT NOT NULL DEFAULT 0, 
    [Category] NVARCHAR(255) NOT NULL DEFAULT 'unknown',
	[Location] geography NULL, 
	[PersonId] bigint NOT NULL,
    CONSTRAINT [FK_PersonAsked_ToTable] FOREIGN KEY ([PersonId]) REFERENCES [Person]([Id])
)
