CREATE TABLE [dbo].[Neccessity](
	[Name] [nvarchar](50) NOT NULL,
	[Food] BIT NOT NULL,
	[Shelter] BIT NOT NULL,
	[Clothes] BIT NOT NULL,
	[Medicine] BIT NOT NULL,
    [Id]                    BIGINT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
