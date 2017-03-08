USE [cityplusdb]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 3/8/2017 2:28:34 PM ******/
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
/****** Object:  Table [dbo].[Conversations]    Script Date: 3/8/2017 2:28:40 PM ******/
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
/****** Object:  Table [dbo].[Sessions]    Script Date: 3/8/2017 2:28:40 PM ******/
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
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Sessions] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Sessions] ([SessionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Sessions]
GO
