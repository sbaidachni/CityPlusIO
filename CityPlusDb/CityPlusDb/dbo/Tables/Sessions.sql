CREATE TABLE [dbo].[Sessions] (
    [SessionId]   INT            IDENTITY (1, 1) NOT NULL,
    [ChannelId]   NVARCHAR (256) NOT NULL,
    [UtcDateTime] DATETIME       CONSTRAINT [DF_Sessions] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC)
);

