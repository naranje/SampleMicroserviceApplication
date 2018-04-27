CREATE DATABASE BookRequest
GO

USE [BookRequest]
GO

CREATE TABLE [dbo].[BookRequest](
  [ID] int IDENTITY(1,1) PRIMARY KEY,
  [UserId] [bigint] NOT NULL,
  CONSTRAINT BookRequestUnique UNIQUE([ID], [UserID])
)
GO

CREATE INDEX BookRequest_UserId 
ON [dbo].[BookRequest] (UserId)
GO

CREATE TABLE [dbo].[BookRequestItems](
  [ID] int IDENTITY(1,1) PRIMARY KEY,
	[BookRequestId]  [INT] NOT NULL,
	[BookCatalogId]  UNIQUEIDENTIFIER NOT NULL,
	[Title]          NVARCHAR (500)   NOT NULL
)

GO

ALTER TABLE [dbo].[BookRequestItems]  WITH CHECK ADD CONSTRAINT [FK_BookRequest] FOREIGN KEY([BookRequestId])
REFERENCES [dbo].[BookRequest] ([Id])
GO

ALTER TABLE [dbo].[BookRequestItems] CHECK CONSTRAINT [FK_BookRequest]
GO

CREATE INDEX BookRequestItems_BookRequestId 
ON [dbo].[BookRequestItems] (BookRequestId)
GO

CREATE TABLE [dbo].[EventStore](
  [ID] int IDENTITY(1,1) PRIMARY KEY,
  [Name] [nvarchar](100)  NOT NULL,
  [OccuredAt] [datetimeoffset] NOT NULL,
  [Content][nvarchar](max) NOT NULL
)
GO

