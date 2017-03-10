CREATE TABLE [dbo].[AttachmentTags] (
    [AttachmentTagId] INT            IDENTITY (1, 1) NOT NULL,
    [AttachmentId]    INT            NOT NULL,
    [name]            NVARCHAR (MAX) NOT NULL,
    [confidence]      REAL           NOT NULL,
    CONSTRAINT [PK_AttachmentTags] PRIMARY KEY CLUSTERED ([AttachmentTagId] ASC),
    CONSTRAINT [FK_AttachmentTags_Attachments] FOREIGN KEY ([AttachmentId]) REFERENCES [dbo].[Attachments] ([AttachmentId]) ON DELETE CASCADE ON UPDATE CASCADE
);

