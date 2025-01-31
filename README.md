# ChatApplication-Server
 


## Usage
1. Open the project in Visual Studio.
2.1   Open SQL Server Management Studio and Login on your existing Server Instance.
2.2 Open new Query and paste the Script to Generate Database and ChatHistory Table

   USE [master]
GO
/****** Object:  Database [ChatDb]    Script Date: 12/16/2024 8:39:34 PM ******/
CREATE DATABASE [ChatDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ChatDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.TESTSERVER\MSSQL\DATA\ChatDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ChatDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.TESTSERVER\MSSQL\DATA\ChatDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [ChatDb] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ChatDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ChatDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ChatDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ChatDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ChatDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ChatDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [ChatDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ChatDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ChatDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ChatDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ChatDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ChatDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ChatDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ChatDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ChatDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ChatDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ChatDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ChatDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ChatDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ChatDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ChatDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ChatDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ChatDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ChatDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ChatDb] SET  MULTI_USER 
GO
ALTER DATABASE [ChatDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ChatDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ChatDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ChatDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ChatDb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ChatDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ChatDb] SET QUERY_STORE = OFF
GO
USE [ChatDb]
GO
/****** Object:  Table [dbo].[ChatHistory]    Script Date: 12/16/2024 8:39:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatHistory](
	[HistoryID] [int] IDENTITY(1,1) NOT NULL,
	[DatetimeCreated] [datetime] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ChatHistory] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [ChatDb] SET  READ_WRITE 
GO



3. Build the project and start the application.
4. The server will run on `ws://localhost:6000`.