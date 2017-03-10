CREATE TABLE [dbo].[Neccessity](
	[Name] [nvarchar](50) NOT NULL,
	[Food] [int] NOT NULL,
	[Shelter] [int] NOT NULL,
	[Clothes] [int] NOT NULL,
	[Medicine] [int] NOT NULL,
    [Id]                    BIGINT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
