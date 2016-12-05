USE [Gsaelzbrot]
GO

DROP TABLE [Hefezopf].[ZombieState]
GO

DROP TYPE [Hefezopf].[ZombieState]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TYPE [Hefezopf].[ZombieState] FROM [tinyint] NOT NULL
GO

CREATE TABLE [Hefezopf].[ZombieState](
	[ZombieState] [Hefezopf].[ZombieState] NOT NULL,
	[Name] [varchar](32) NOT NULL,
	[HZRowVersion] [timestamp] NOT NULL,

    CONSTRAINT [PK_Hefezopf_ZombieState] PRIMARY KEY CLUSTERED ( [ZombieState] ASC )

) ON [PRIMARY]

GO

ALTER TABLE [Hefezopf].[ZombieState]  WITH CHECK ADD CHECK  ([ZombieState] IN (0,1,2,3))
GO


INSERT INTO [Hefezopf].[ZombieState]
           ([ZombieState], [Name])
     VALUES (0,'Alive'),(1,'SelfDeleted'),(2,'ParentDeleted'),(3,'ParentSelfDeleted');
GO

SELECT [ZombieState], [Name] FROM [Hefezopf].[ZombieState]