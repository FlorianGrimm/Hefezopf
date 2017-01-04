-- after HefezopfBase
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

EXECUTE [Hefezopf].[EnableDDLChangeTracking]
GO
EXECUTE [Hefezopf].[EnsureSchema] 'HefezopfOnline'
GO
---------------------------------------
--
-- [HefezopfOnline].[UserTokenCaches]
--
---------------------------------------
if (OBJECT_ID('[HefezopfOnline].[UserTokenCaches]') IS NULL) BEGIN
	CREATE TABLE [HefezopfOnline].[UserTokenCaches] (
		[WebUserUniqueId] nvarchar(200) NOT NULL,
		[CacheBits] varbinary(max) NULL,
		[UserTokenCaches_CreatedAt] datetime2 NOT NULL,
		[UserTokenCaches_ModifiedAt] datetime2 NOT NULL,
		[UserTokenCaches_RowVersion] rowversion NOT NULL,
		CONSTRAINT [PK-HefezopfOnline-UserTokenCaches] PRIMARY KEY CLUSTERED 
		(
			[WebUserUniqueId] ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	);
END;
GO

EXECUTE [Hefezopf].[CreateProcedureIfNeeded] '[HefezopfOnline].[UserTokenCaches_Upsert]'
GO
ALTER PROCEDURE [HefezopfOnline].[UserTokenCaches_Upsert]
(
	@WebUserUniqueId nvarchar(200),
	@CacheBits varbinary(max),
	@ModifiedAt datetime2
)
AS BEGIN
	SET NOCOUNT ON;
	DECLARE @now datetime2 = getdate();
	MERGE INTO [HefezopfOnline].[UserTokenCaches] as dst
	USING
	(
		SELECT
			WebUserUniqueId = @WebUserUniqueId,
			CacheBits = @CacheBits,
			UserTokenCaches_CreatedAt = @now,
			UserTokenCaches_ModifiedAt = ISNULL(@ModifiedAt, @now)
	) as src
	ON dst.WebUserUniqueId = src.WebUserUniqueId
	WHEN MATCHED 
		THEN UPDATE SET
			WebUserUniqueId = src.WebUserUniqueId,
			CacheBits 		= src.CacheBits,
			UserTokenCaches_ModifiedAt = src.UserTokenCaches_ModifiedAt
	WHEN NOT MATCHED
		THEN INSERT 
			( WebUserUniqueId, CacheBits, UserTokenCaches_CreatedAt, UserTokenCaches_ModifiedAt )
		VALUES ( src.WebUserUniqueId, src.CacheBits, src.UserTokenCaches_CreatedAt, src.UserTokenCaches_ModifiedAt )
	;
END;
GO

EXECUTE [Hefezopf].[CreateProcedureIfNeeded] '[HefezopfOnline].[UserTokenCaches_GetByWebUserUniqueId]'
GO
ALTER PROCEDURE [HefezopfOnline].[UserTokenCaches_GetByWebUserUniqueId]
(
	@WebUserUniqueId nvarchar(200),
	@UserTokenCaches_CreatedAt datetime2
)
AS BEGIN
	SET NOCOUNT ON;
	SELECT TOP(1) utc.WebUserUniqueId, utc.CacheBits, utc.[UserTokenCaches_CreatedAt], utc.[UserTokenCaches_ModifiedAt]
	FROM [HefezopfOnline].[UserTokenCaches] as utc
	WHERE (utc.WebUserUniqueId = @WebUserUniqueId)	
	;
END;
GO

EXECUTE [Hefezopf].[CreateProcedureIfNeeded] '[HefezopfOnline].[UserTokenCaches_Clear]'
GO
ALTER PROCEDURE [HefezopfOnline].[UserTokenCaches_Clear]
AS BEGIN
	SET NOCOUNT ON;
	DELETE FROM [HefezopfOnline].[UserTokenCaches];
END;
GO

---------------------------------------
--
-- [Hefezopf].[ApplicationSettings]
--
---------------------------------------
if (OBJECT_ID('[Hefezopf].[ApplicationSettingsHost]') IS NULL) BEGIN
	CREATE TABLE [Hefezopf].[ApplicationSettingsHost] (
		[ApplicationSettingsHostUid] uniqueidentifier NOT NULL,
		[HostName] nvarchar(200) NOT NULL,
		[ApplicationSettingsHost_CreatedAt] datetime2 NOT NULL,
		[ApplicationSettingsHost_ModifiedAt] datetime2 NOT NULL,
		[ApplicationSettingsHost_RowVersion] rowversion NOT NULL,

		CONSTRAINT [PK_Hefezopf.ApplicationSettingsHost] PRIMARY KEY CLUSTERED 
		(
			[ApplicationSettingsHostUid] ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	);
END;
GO
if (OBJECT_ID('[Hefezopf].[ApplicationSettingsValue]') IS NULL) BEGIN
	CREATE TABLE [Hefezopf].[ApplicationSettingsValue] (
		[ApplicationSettingsHostUid] uniqueidentifier NOT NULL,
		[SettingName] varchar(128) NOT NULL,
		[SettingValue] nvarchar(max) NOT NULL,
		[ApplicationSettingsValue_CreatedAt] datetime2 NOT NULL,
		[ApplicationSettingsValue_ModifiedAt] datetime2 NOT NULL,
		[ApplicationSettingsValue_RowVersion] rowversion NOT NULL,

		CONSTRAINT [PK_Hefezopf.ApplicationSettingsValue] PRIMARY KEY CLUSTERED 
		(
			[ApplicationSettingsHostUid] ASC, [SettingName] ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	);
END;
GO

EXECUTE [Hefezopf].[CreateProcedureIfNeeded] '[Hefezopf].[ApplicationSettings_Upsert1]'
GO
ALTER PROCEDURE [Hefezopf].[ApplicationSettings_Upsert1]
(
	@HostName nvarchar(200),
	@SettingName varchar(128),
	@SettingValue nvarchar(max)
)
AS BEGIN
	SET NOCOUNT ON;
	DECLARE @now datetime2 = getdate();
	DECLARE	@ApplicationSettingsHostUid uniqueidentifier;
	SELECT TOP(1) @ApplicationSettingsHostUid = ash.ApplicationSettingsHostUid 
	FROM [Hefezopf].[ApplicationSettingsHost] as ash 
	WHERE ash.HostName = @Hostname;
	if (@ApplicationSettingsHostUid IS NULL) BEGIN
		SET @ApplicationSettingsHostUid = NEWID();
		INSERT INTO [Hefezopf].[ApplicationSettingsHost]
			   (ApplicationSettingsHostUid
			   ,HostName
			   ,ApplicationSettingsHost_CreatedAt
			   ,ApplicationSettingsHost_ModifiedAt)
		 VALUES
			   ( @ApplicationSettingsHostUid
			   , @HostName
			   , @now
			   , @now)
	END;

	MERGE INTO [Hefezopf].[ApplicationSettingsValue] as dst
	USING
	(
		SELECT
			ApplicationSettingsHostUid = @ApplicationSettingsHostUid,
			SettingName				   = @SettingName,
			SettingValue			   = @SettingValue
	) as src
	ON (dst.ApplicationSettingsHostUid = src.ApplicationSettingsHostUid)
		AND (dst.SettingName = src.SettingName)
	WHEN MATCHED 
		THEN UPDATE SET
			SettingValue = src.SettingValue,
			ApplicationSettingsValue_ModifiedAt = @now
	WHEN NOT MATCHED
		THEN INSERT 
				   (ApplicationSettingsHostUid
				   ,SettingName
				   ,SettingValue
				   ,ApplicationSettingsValue_CreatedAt
				   ,ApplicationSettingsValue_ModifiedAt)
			 VALUES
				   ( @ApplicationSettingsHostUid
				   , @SettingName
				   , @SettingValue
				   , @now
				   , @now)
	;
END;
GO



EXECUTE [Hefezopf].[CreateProcedureIfNeeded] '[Hefezopf].[ApplicationSettings_GetByHostName]'
GO
ALTER PROCEDURE [Hefezopf].[ApplicationSettings_GetByHostName]
(
	@HostName nvarchar(200)
)
AS BEGIN
	SELECT  HostName = ash.HostName
		  , SettingName = asv.SettingName
		  , SettingValue = asv.SettingValue
		  , ApplicationSettingsHost_VersionNumber = CAST(ash.ApplicationSettingsHost_RowVersion AS bigint)
		  , ApplicationSettingsValue_VersionNumber = CAST(asv.ApplicationSettingsValue_RowVersion as bigint)
	FROM [Hefezopf].[ApplicationSettingsHost] as ash
	INNER JOIN [Hefezopf].[ApplicationSettingsValue] as asv
		ON ash.[ApplicationSettingsHostUid] = asv.[ApplicationSettingsHostUid]
	WHERE ash.[HostName] = @HostName	
	;
END;
GO