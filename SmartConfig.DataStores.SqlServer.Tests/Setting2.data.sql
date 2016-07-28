
USE [SmartConfigTest]
GO

CREATE PROC #insertOrUpdateSetting2	
	@name NVARCHAR(200), 
	@value NVARCHAR(MAX),
	@corge NVARCHAR(200),
	@config NVARCHAR(200)
AS
BEGIN
	UPDATE [Setting2] SET [Value] = @value WHERE [Name]= @name AND [Corge] = @corge AND [Config]= @config
	IF @@ROWCOUNT = 0 INSERT INTO [Setting2]([Name], [Value], [Corge], [Config]) VALUES (@name, @value, @corge, @config)
END
GO

DELETE FROM [Setting2]

EXEC #insertOrUpdateSetting2 'StringSetting', 'Foo', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'ArraySetting[0]', '3', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'ArraySetting[1]', '7', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'DictionarySetting[foo]', '4', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'DictionarySetting[bar]', '8', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'NestedConfig.StringSetting', 'Bar', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'IgnoredConfig.StringSetting', 'Qux', 'WALDO', 'thudy'

GO

SELECT * FROM [Setting2]

DROP PROC #insertOrUpdateSetting
GO