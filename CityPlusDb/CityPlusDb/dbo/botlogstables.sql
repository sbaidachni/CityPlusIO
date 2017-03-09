USE [cityplusdb]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 3/8/2017 3:24:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[ContentType] [nvarchar](max) NOT NULL,
	[ContentUrl] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 3/8/2017 3:24:35 PM ******/
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
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[EmotionDetails]    Script Date: 3/8/2017 3:24:35 PM ******/
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
/****** Object:  Table [dbo].[Emotions]    Script Date: 3/8/2017 3:24:35 PM ******/
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
/****** Object:  Table [dbo].[Person]    Script Date: 3/8/2017 3:24:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Location] [geography] NOT NULL,
	[Channel] [nvarchar](50) NULL,
	[AccountID] [nvarchar](255) NULL,
	[AccountName] [nvarchar](255) NULL,
	[ClientNotificationUri] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[PersonAsked]    Script Date: 3/8/2017 3:24:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonAsked](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[Category] [nvarchar](255) NOT NULL,
	[Location] [geography] NULL,
	[PersonId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Sessions]    Script Date: 3/8/2017 3:24:36 PM ******/
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
ALTER TABLE [dbo].[PersonAsked] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[PersonAsked] ADD  DEFAULT ('unknown') FOR [Category]
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
ALTER TABLE [dbo].[PersonAsked]  WITH CHECK ADD  CONSTRAINT [FK_PersonAsked_ToTable] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([Id])
GO
ALTER TABLE [dbo].[PersonAsked] CHECK CONSTRAINT [FK_PersonAsked_ToTable]
GO
