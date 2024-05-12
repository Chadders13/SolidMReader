CREATE TABLE [dbo].[Accounts](
	[AccountId] [int] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[MeterReadings](
	[MeterReadingGuid] [uniqueidentifier] NOT NULL,
	[AccountId] [int] NOT NULL,
	[MeterReadingDateTime] [datetime2](7) NOT NULL,
	[MeterReadValue] [int] NOT NULL,
 CONSTRAINT [PK_MeterReadings] PRIMARY KEY CLUSTERED 
(
	[MeterReadingGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MeterReadings] ADD  CONSTRAINT [DF_MeterReadings_MeterReadingGuid]  DEFAULT (newid()) FOR [MeterReadingGuid]
GO

ALTER TABLE [dbo].[MeterReadings]  WITH CHECK ADD  CONSTRAINT [FK_MeterReadings_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO

ALTER TABLE [dbo].[MeterReadings] CHECK CONSTRAINT [FK_MeterReadings_Accounts]
GO