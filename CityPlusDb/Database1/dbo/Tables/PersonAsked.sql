CREATE TABLE [dbo].[PersonAsked] (
    [Id]          BIGINT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (MAX)    NULL,
    [Quantity]    INT               DEFAULT ((0)) NOT NULL,
    [Category]    NVARCHAR (255)    DEFAULT ('unknown') NOT NULL,
    [Location]    [sys].[geography] NULL,
    [PersonId]    BIGINT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PersonAsked_ToTable] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id])
);

