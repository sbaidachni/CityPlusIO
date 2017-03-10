CREATE TABLE [dbo].[Conversations] (
    [ConversationId] INT               IDENTITY (1, 1) NOT NULL,
    [SessionId]      INT               NOT NULL,
    [Text]           NVARCHAR (MAX)    NOT NULL,
    [Locale]         NVARCHAR (256)    NULL,
    [Address]        NVARCHAR (256)    NULL,
    [GeoCoordinates] [sys].[geography] NULL,
    [UtcDateTime]    DATETIME          CONSTRAINT [DF_Conversations] DEFAULT (getdate()) NOT NULL,
    [sentiment]      REAL              NULL,
    CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED ([ConversationId] ASC),
    CONSTRAINT [FK_Conversations_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([SessionId]) ON DELETE CASCADE ON UPDATE CASCADE
);

