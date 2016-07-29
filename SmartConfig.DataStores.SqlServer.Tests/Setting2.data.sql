
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

EXEC #insertOrUpdateSetting2 'StringSetting', 'Fooxy', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'ArraySetting[0]', '31', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'ArraySetting[1]', '71', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'DictionarySetting[foo]', '42', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'DictionarySetting[bar]', '82', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'NestedConfig.StringSetting', 'Barxy', 'WALDO', 'thudy'
EXEC #insertOrUpdateSetting2 'IgnoredConfig.StringSetting', 'Quxy', 'WALDO', 'thudy'

GO

SELECT * FROM [Setting2]

DROP PROC #insertOrUpdateSetting2
GO