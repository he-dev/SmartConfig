DROP TABLE IF EXISTS Setting1;

CREATE TABLE IF NOT EXISTS Setting1( 
        [Name] TEXT NOT NULL COLLATE NOCASE,
        [Value] TEXT NOT NULL COLLATE NOCASE
);

INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('Utf8SettingDE', 'äöüß');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('Utf8SettingPL', 'ąęśćżźó');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('ArraySetting[0]', '3');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('ArraySetting[1]', '7');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('DictionarySetting[foo]', '4');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('DictionarySetting[bar]', '8');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('NestedConfig.StringSetting', 'Bar');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('IgnoredConfig.StringSettingDE', 'Qux');

INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.Utf8SettingDE', 'äöüß');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.Utf8SettingPL', 'ąęśćżźó');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.ArraySetting[0]', '3');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.ArraySetting[1]', '7');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.DictionarySetting[foo]', '4');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.DictionarySetting[bar]', '8');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.NestedConfig.StringSetting', 'Bar');
INSERT OR REPLACE INTO Setting1([Name], [Value]) VALUES('todyx.IgnoredConfig.StringSettingDE', 'Qux');


DROP TABLE IF EXISTS Setting2;

CREATE TABLE IF NOT EXISTS Setting2( 
        [Name] TEXT NOT NULL COLLATE NOCASE,
        [Value] TEXT NOT NULL COLLATE NOCASE,
        [Corge] TEXT NOT NULL COLLATE NOCASE,
        [Config] TEXT NOT NULL COLLATE NOCASE
);

INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('Utf8SettingDE', 'äöüß', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('Utf8SettingPL', 'ąęśćżźó', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('ArraySetting[0]', '3', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('ArraySetting[1]', '7', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('DictionarySetting[foo]', '4', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('DictionarySetting[bar]', '8', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('NestedConfig.StringSetting', 'Bar', 'thudyx', 'waldo');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('IgnoredConfig.StringSettingDE', 'Qux', 'thudyx', 'waldo');

-- few other settings to be sure they don't get selected
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('Utf8SettingDE', 'äöüß', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('Utf8SettingPL', 'ąęśćżźó', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('ArraySetting[0]', '3', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('ArraySetting[1]', '7', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('DictionarySetting[foo]', '4', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('DictionarySetting[bar]', '8', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('NestedConfig.StringSetting', 'Bar', 'thudyx', 'waldox');
INSERT OR REPLACE INTO Setting2([Name], [Value], [Corge], [Config]) VALUES('IgnoredConfig.StringSettingDE', 'Qux', 'thudyx', 'waldox');
