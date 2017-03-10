CREATE TABLE [dbo].[Attachments] (
    [AttachmentId]   INT            IDENTITY (1, 1) NOT NULL,
    [ConversationId] INT            NOT NULL,
    [ContentType]    NVARCHAR (MAX) NOT NULL,
    [ContentUrl]     NVARCHAR (MAX) NOT NULL,
    [isAdultContent] BIT            NULL,
    [isRacyContent]  BIT            NULL,
    [adultScore]     REAL           NULL,
    [racyScore]      REAL           NULL,
    CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED ([AttachmentId] ASC),
    CONSTRAINT [FK_Attachments_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([ConversationId]) ON DELETE CASCADE ON UPDATE CASCADE
);

