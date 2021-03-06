USE [League_staging]
GO
/****** Object:  Table [dbo].[Combat]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Combat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[PlayerId] [nvarchar] (max) NOT NULL,
	[ChampionId] [nvarchar](max) NULL,
	[Kills] [int] NULL,
	[Deaths] [int] NULL,
	[Assists] [int] NULL,
	[LKS] [int] NULL,
	[LMK] [int] NULL,
	[FirstBlood] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DamageDealt]    Script Date: 2/6/2016 11:54:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageDealt](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[PlayerId] [nvarchar] (max) NOT NULL,
	[ChampionId] [nvarchar](max)  NULL,
	[TDC] [int] NULL,
	[PDC] [int] NULL,
	[MDC] [int] NULL,
	[TrDC] [int] NULL,
	[DamageDealt] [int] NULL,
	[PhyDamageDealt] [int] NULL,
	[MagicDamageDealt] [int] NULL,
	[TrueDamageDealt] [int] NULL,
	[LCS] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DamageTaken]    Script Date: 2/6/2016 11:54:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageTaken](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[PlayerId] [nvarchar] (max) NOT NULL,
	[ChampionId] [nvarchar](max)  NULL,
	[DamageHealed] [int] NULL,
	[DamageTaken] [int] NULL,
	[PhyDamageTaken] [int] NULL,
	[MagicDamageTaken] [int] NULL,
	[TrueDamageTaken] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Games]    Script Date: 2/6/2016 11:54:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Games](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[BlueTeam] [nvarchar](max) NULL,
	[RedTeam] [nvarchar](max) NULL,
	[GameDate] [datetime] NULL,
	[Season] [int] NULL,
	[League] [int] NULL,
	[Patch] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[Duration] [time](7) NOT NULL,
	[MatchHistory] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Income]    Script Date: 2/6/2016 11:54:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Income](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[PlayerId] [nvarchar] (max) NOT NULL,
	[ChampionId] [nvarchar](max)  NULL,
	[GoldEarned] [int] NULL,
	[GoldSpent] [int] NULL,
	[MinionsKilled] [int] NULL,
	[NMK] [int] NULL,
	[NMKTeam] [int] NULL,
	[NMKEnemy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Wards]    Script Date: 2/6/2016 11:54:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wards](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[PlayerId] [nvarchar] (max) NOT NULL,
	[ChampionId] [nvarchar](max)  NULL,
	[WardsPlaced] [int] NULL,
	[WardsDestroyed] [int] NULL,
	[SWardsPurchased] [int] NULL,
	[VWardsPurchased] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[Bans](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Team] bit NULL,
	[GameId] [int] NOT NULL,
	[ChampionId] [nvarchar](max)  NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
