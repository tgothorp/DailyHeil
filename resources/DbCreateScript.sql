
-- Create Newspaper table
CREATE TABLE Newspaper
(
	Id BIGINT IDENTITY(1, 1) NOT NULL,
	[Name] NVARCHAR(256) NOT NULL,
	[Scraper] NVARCHAR(64) NOT NULL,
	CONSTRAINT [PK_Newspaper] PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
)

-- Create Feed table with reference to Newspaper table
CREATE TABLE Feed
(
	Id BIGINT IDENTITY(1, 1) NOT NULL,
	[FeedUrl] NVARCHAR(255) NOT NULL,
	[NewspaperId] BIGINT NOT NULL,
	CONSTRAINT [PK_Feed] PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
)

ALTER TABLE [Feed]  WITH CHECK ADD  CONSTRAINT [FK_Feed_NewspaperId_Newspaper_Id] FOREIGN KEY([NewspaperId])
	REFERENCES [dbo].[Newspaper] ([Id])
GO

-- Create Article table with reference to Newspaper table
CREATE TABLE Article
(
	Id BIGINT IDENTITY(1, 1) NOT NULL,
	[Title] NVARCHAR(4000) NOT NULL,
	[Url] NVARCHAR(4000) NOT NULL,
	[PublishedDate] DATETIME2 NOT NULL,
	[Hash] NVARCHAR(32) NOT NULL,
	[NewspaperId] BIGINT NOT NULL,
	[Scraped] BIT NOT NULL,
	[MentionsHitler] BIT NOT NULL,
	CONSTRAINT [PK_Article] PRIMARY KEY CLUSTERED
	(
		Id ASC
	)

)

ALTER TABLE [Article]  WITH CHECK ADD  CONSTRAINT [FK_Article_NewspaperId_Newspaper_Id] FOREIGN KEY([NewspaperId])
	REFERENCES [dbo].[Newspaper] ([Id])
GO


-- Insert DM & feed
INSERT INTO dbo.Newspaper
( [Name], [Scraper] )
VALUES
(	N'Daily Mail', 'DailyMail' )


INSERT INTO dbo.Feed
( FeedUrl, NewspaperId )
VALUES
( N'https://www.dailymail.co.uk/articles.rss', SCOPE_IDENTITY())



SELECT * FROM dbo.Newspaper
SELECT * FROM dbo.Feed
SELECT * FROM dbo.Article WHERE MentionsHitler = 1
