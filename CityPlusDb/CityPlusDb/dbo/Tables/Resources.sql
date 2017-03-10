CREATE TABLE [dbo].[Resources] (
    [Name]     NVARCHAR (100) NULL,
    [Location] NVARCHAR (100) NULL,
    [Food]     NVARCHAR (50)  NULL,
    [Shelter]  NVARCHAR (50)  NULL,
    [Clothes]  NVARCHAR (50)  NULL,
    [Medicine] NVARCHAR (50)  NULL,
	[latlong] sys.geography,
    [Id]                    BIGINT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);