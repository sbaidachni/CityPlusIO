CREATE TABLE [dbo].[ActivityDialog] (
    [ActivityDialogId] INT            IDENTITY (1, 1) NOT NULL,
    [Type]             NVARCHAR (MAX) NOT NULL,
    [Text]             NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ActivityDialog] PRIMARY KEY CLUSTERED ([ActivityDialogId] ASC)
);

