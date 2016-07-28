
USE [SmartConfigTest]
GO

CREATE PROC #insertOrUpdateSetting	
	@name NVARCHAR(200), 
	@value NVARCHAR(MAX)
AS
BEGIN
	UPDATE [Setting1]	SET [Value] = @value WHERE [Name]= @name
	IF @@ROWCOUNT = 0 INSERT INTO [Setting1]([Name], [Value]) VALUES (@name, @value)
END
GO

DELETE FROM [Setting1]

EXEC #insertOrUpdateSetting 'StringSetting', 'Foo'
EXEC #insertOrUpdateSetting 'ArraySetting[0]', '3'
EXEC #insertOrUpdateSetting 'ArraySetting[1]', '7'
EXEC #insertOrUpdateSetting 'DictionarySetting[foo]', '4'
EXEC #insertOrUpdateSetting 'DictionarySetting[bar]', '8'
EXEC #insertOrUpdateSetting 'NestedConfig.StringSetting', 'Bar'
EXEC #insertOrUpdateSetting 'IgnoredConfig.StringSetting', 'Qux'

EXEC #insertOrUpdateSetting 'thud.StringSetting', 'Foox'
EXEC #insertOrUpdateSetting 'thud.ArraySetting[0]', '33'
EXEC #insertOrUpdateSetting 'thud.ArraySetting[1]', '77'
EXEC #insertOrUpdateSetting 'thud.DictionarySetting[foo]', '44'
EXEC #insertOrUpdateSetting 'thud.DictionarySetting[bar]', '88'
EXEC #insertOrUpdateSetting 'thud.NestedConfig.StringSetting', 'Barx'
EXEC #insertOrUpdateSetting 'thud.IgnoredConfig.StringSetting', 'Quxx'

GO

SELECT * FROM [Setting1]

DROP PROC #insertOrUpdateSetting
GO