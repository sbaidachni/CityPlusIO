USE [cityplusdb]
GO
/****** Object:  Table [dbo].[ActivityDialog]    Script Date: 3/8/2017 4:03:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityDialog](
	[ActivityDialogId] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ActivityDialog] PRIMARY KEY CLUSTERED 
(
	[ActivityDialogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 3/8/2017 4:03:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[ContentType] [nvarchar](max) NOT NULL,
	[ContentUrl] [nvarchar](max) NOT NULL,
	[isAdultContent] [bit] NULL,
	[isRacyContent] [bit] NULL,
	[adultScore] [real] NULL,
	[racyScore] [real] NULL,
 CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AttachmentTags]    Script Date: 3/8/2017 4:03:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentTags](
	[AttachmentTagId] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[confidence] [real] NOT NULL,
 CONSTRAINT [PK_AttachmentTags] PRIMARY KEY CLUSTERED 
(
	[AttachmentTagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 3/8/2017 4:03:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[ConversationId] [int] IDENTITY(1,1) NOT NULL,
	[SessionId] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[Locale] [nvarchar](256) NULL,
	[Address] [nvarchar](256) NULL,
	[GeoCoordinates] [geography] NULL,
	[UtcDateTime] [datetime] NOT NULL,
	[sentiment] [decimal](1, 1) NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[EmotionDetails]    Script Date: 3/8/2017 4:03:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmotionDetails](
	[EmotionDetailId] [int] IDENTITY(1,1) NOT NULL,
	[EmotionId] [int] NOT NULL,
	[scoreName] [nvarchar](max) NOT NULL,
	[scoreValue] [real] NOT NULL,
 CONSTRAINT [PK_EmotionDetails] PRIMARY KEY CLUSTERED 
(
	[EmotionDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Emotions]    Script Date: 3/8/2017 4:03:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Emotions](
	[EmotionId] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[left] [int] NOT NULL,
	[top] [int] NOT NULL,
	[width] [int] NOT NULL,
	[height] [int] NOT NULL,
 CONSTRAINT [PK_Emotions] PRIMARY KEY CLUSTERED 
(
	[EmotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Sessions]    Script Date: 3/8/2017 4:03:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sessions](
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[ChannelId] [nvarchar](256) NOT NULL,
	[UtcDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
ALTER TABLE [dbo].[Conversations] ADD  CONSTRAINT [DF_Conversations]  DEFAULT (getdate()) FOR [UtcDateTime]
GO
ALTER TABLE [dbo].[Sessions] ADD  CONSTRAINT [DF_Sessions]  DEFAULT (getdate()) FOR [UtcDateTime]
GO
ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD  CONSTRAINT [FK_Attachments_Conversations] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([ConversationId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attachments] CHECK CONSTRAINT [FK_Attachments_Conversations]
GO
ALTER TABLE [dbo].[AttachmentTags]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentTags_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([AttachmentId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachmentTags] CHECK CONSTRAINT [FK_AttachmentTags_Attachments]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Sessions] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Sessions] ([SessionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Sessions]
GO
ALTER TABLE [dbo].[EmotionDetails]  WITH CHECK ADD  CONSTRAINT [FK_EmotionDetails_Emotions] FOREIGN KEY([EmotionId])
REFERENCES [dbo].[Emotions] ([EmotionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmotionDetails] CHECK CONSTRAINT [FK_EmotionDetails_Emotions]
GO
ALTER TABLE [dbo].[Emotions]  WITH CHECK ADD  CONSTRAINT [FK_Emotions_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([AttachmentId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Emotions] CHECK CONSTRAINT [FK_Emotions_Attachments]
GO
