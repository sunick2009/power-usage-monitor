USE [master]
GO
/****** Object:  Database [power_usage_monitor]    Script Date: 2023/1/11 下午 07:54:39 ******/
CREATE DATABASE [power_usage_monitor]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'power_usage_monitor', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\power_usage_monitor.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'power_usage_monitor_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\power_usage_monitor_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [power_usage_monitor] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [power_usage_monitor].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [power_usage_monitor] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [power_usage_monitor] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [power_usage_monitor] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [power_usage_monitor] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [power_usage_monitor] SET ARITHABORT OFF 
GO
ALTER DATABASE [power_usage_monitor] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [power_usage_monitor] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [power_usage_monitor] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [power_usage_monitor] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [power_usage_monitor] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [power_usage_monitor] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [power_usage_monitor] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [power_usage_monitor] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [power_usage_monitor] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [power_usage_monitor] SET  DISABLE_BROKER 
GO
ALTER DATABASE [power_usage_monitor] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [power_usage_monitor] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [power_usage_monitor] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [power_usage_monitor] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [power_usage_monitor] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [power_usage_monitor] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [power_usage_monitor] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [power_usage_monitor] SET RECOVERY FULL 
GO
ALTER DATABASE [power_usage_monitor] SET  MULTI_USER 
GO
ALTER DATABASE [power_usage_monitor] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [power_usage_monitor] SET DB_CHAINING OFF 
GO
ALTER DATABASE [power_usage_monitor] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [power_usage_monitor] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [power_usage_monitor] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'power_usage_monitor', N'ON'
GO
ALTER DATABASE [power_usage_monitor] SET QUERY_STORE = OFF
GO
USE [power_usage_monitor]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [power_usage_monitor]
GO
/****** Object:  User [pum_ADMIN]    Script Date: 2023/1/11 下午 07:54:40 ******/
CREATE USER [pum_ADMIN] FOR LOGIN [pum_ADMIN] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [pum_ADMIN]
GO
ALTER ROLE [db_datareader] ADD MEMBER [pum_ADMIN]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [pum_ADMIN]
GO
/****** Object:  Table [dbo].[Abnormal]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Abnormal](
	[Abnormal_ID] [int] IDENTITY(1,1) NOT NULL,
	[Device_ID] [int] NOT NULL,
	[Abnormal_Usage] [float] NOT NULL,
	[Abnormal_Time] [datetime] NOT NULL,
 CONSTRAINT [PK_Abnormal] PRIMARY KEY CLUSTERED 
(
	[Abnormal_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Category_ID] [int] IDENTITY(1,1) NOT NULL,
	[Engnery_Name] [nvarchar](50) NOT NULL,
	[Device_Category_Name] [nvarchar](50) NOT NULL,
	[Category_Avg_Power] [float] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Category_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Device]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Device](
	[Device_ID] [int] NOT NULL,
	[Device_Name] [nvarchar](50) NOT NULL,
	[StandbyTime] [nvarchar](50) NOT NULL,
	[UseTime] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[Category_ID] [int] NOT NULL,
	[User_Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[Device_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Statistics]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Statistics](
	[Device_ID] [int] NOT NULL,
	[Period] [nvarchar](50) NOT NULL,
	[Total_Usage] [float] NOT NULL,
	[AveragePower] [float] NOT NULL,
 CONSTRAINT [PK_Statistics] PRIMARY KEY CLUSTERED 
(
	[Device_ID] ASC,
	[Period] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usage]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usage](
	[Time] [datetime] NOT NULL,
	[Device_ID] [int] NOT NULL,
	[PowerUsed] [float] NOT NULL,
 CONSTRAINT [PK_Usage] PRIMARY KEY CLUSTERED 
(
	[Time] ASC,
	[Device_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2023/1/11 下午 07:54:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[User_Name] [nvarchar](50) NOT NULL,
	[User_Email] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[User_Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Abnormal]  WITH CHECK ADD  CONSTRAINT [FK_Abnormal_Device] FOREIGN KEY([Device_ID])
REFERENCES [dbo].[Device] ([Device_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Abnormal] CHECK CONSTRAINT [FK_Abnormal_Device]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Category] FOREIGN KEY([Category_ID])
REFERENCES [dbo].[Category] ([Category_ID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_Category]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_User] FOREIGN KEY([User_Name])
REFERENCES [dbo].[User] ([User_Name])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_User]
GO
ALTER TABLE [dbo].[Statistics]  WITH CHECK ADD  CONSTRAINT [FK_Statistics_Device] FOREIGN KEY([Device_ID])
REFERENCES [dbo].[Device] ([Device_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Statistics] CHECK CONSTRAINT [FK_Statistics_Device]
GO
ALTER TABLE [dbo].[Usage]  WITH NOCHECK ADD  CONSTRAINT [FK_Usage_Device] FOREIGN KEY([Device_ID])
REFERENCES [dbo].[Device] ([Device_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Usage] NOCHECK CONSTRAINT [FK_Usage_Device]
GO
USE [master]
GO
ALTER DATABASE [power_usage_monitor] SET  READ_WRITE 
GO
