CREATE TABLE [dbo].[Faces] (
    [Id]           BIGINT IDENTITY (1, 1) NOT NULL,
    [AttachmentId] INT    NOT NULL,
    [heightSize]   INT    NULL,
    [leftSize]     INT    NULL,
    [topSize]      INT    NULL,
    [widthSize]    INT    NULL,
    [anger]        REAL   NULL,
    [contempt]     REAL   NULL,
    [disgust]      REAL   NULL,
    [fear]         REAL   NULL,
    [happiness]    REAL   NULL,
    [neutral]      REAL   NULL,
    [sadness]      REAL   NULL,
    [surprise]     REAL   NULL,
    CONSTRAINT [PK_Faces] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Faces_Attachments] FOREIGN KEY ([AttachmentId]) REFERENCES [dbo].[Attachments] ([AttachmentId])
);

