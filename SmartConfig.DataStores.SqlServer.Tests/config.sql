
DECLARE @userTable as nvarchar(50) = 'U'

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Setting1' AND xtype=@userTable)
    CREATE TABLE [dbo].[Setting1] (
		[Name]  NVARCHAR (50)  NOT NULL,
		[Value] NVARCHAR (MAX) NULL,
		PRIMARY KEY CLUSTERED ([Name] ASC)
	);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Setting3' AND xtype=@userTable)
    CREATE TABLE [dbo].[Setting3] (
		[Name]        NVARCHAR (50)  NOT NULL,
		[Value]       NVARCHAR (MAX) NULL,
		[Environment] NVARCHAR (50)  NOT NULL,
		[Config]      NVARCHAR (50)  NOT NULL,
		CONSTRAINT [PK_Setting3] PRIMARY KEY CLUSTERED ([Name] ASC, [Environment] ASC, [Config] ASC)
	);


TRUNCATE TABLE [Setting1];

INSERT INTO [Setting1]([Name], [Value]) VALUES('Utf8SettingDE', 'äöüß');
INSERT INTO [Setting1]([Name], [Value]) VALUES('Utf8SettingPL', 'ąęśćżźó');
INSERT INTO [Setting1]([Name], [Value]) VALUES('ArraySetting[0]', '5');
INSERT INTO [Setting1]([Name], [Value]) VALUES('ArraySetting[1]', '8');
INSERT INTO [Setting1]([Name], [Value]) VALUES('DictionarySetting[foo]', '21');
INSERT INTO [Setting1]([Name], [Value]) VALUES('DictionarySetting[bar]', '34');
INSERT INTO [Setting1]([Name], [Value]) VALUES('NestedConfig.StringSetting', 'Bar');
INSERT INTO [Setting1]([Name], [Value]) VALUES('IgnoredConfig.StringSettingDE', 'Qux');

INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.Utf8SettingDE', 'äöüß');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.Utf8SettingPL', 'ąęśćżźó');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.ArraySetting[0]', '51');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.ArraySetting[1]', '81');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.DictionarySetting[foo]', '212');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.DictionarySetting[bar]', '342');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.NestedConfig.StringSetting', 'Bar');
INSERT INTO [Setting1]([Name], [Value]) VALUES('TestConfig1.IgnoredConfig.StringSettingDE', 'Qux');

TRUNCATE TABLE [Setting3];

INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('Utf8SettingDE', 'äöüß', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('Utf8SettingPL', 'ąęśćżźó', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('ArraySetting[0]', '52', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('ArraySetting[1]', '82', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('DictionarySetting[foo]', '213', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('DictionarySetting[bar]', '343', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('NestedConfig.StringSetting', 'Bar', 'TEST', 'TestConfig3');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('IgnoredConfig.StringSettingDE', 'Qux', 'TEST', 'TestConfig3');

-- some noise settings to be sure they don't get selected
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('Utf8SettingDE', 'äöüß-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('Utf8SettingPL', 'ąęśćżźó-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('ArraySetting[0]', '54-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('ArraySetting[1]', '84-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('DictionarySetting[foo]', '214-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('DictionarySetting[bar]', '344-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('NestedConfig.StringSetting', 'Bar-', 'TEST', 'UndefinedConfig');
INSERT INTO [Setting3]([Name], [Value], [Environment], [Config]) VALUES('IgnoredConfig.StringSettingDE', 'Qux-', 'TEST', 'UndefinedConfig');
