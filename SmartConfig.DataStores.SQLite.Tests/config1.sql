DROP TABLE IF EXISTS Setting;

CREATE TABLE IF NOT EXISTS Setting( 
        [Name] TEXT NOT NULL COLLATE NOCASE,
        [Value] TEXT NOT NULL COLLATE NOCASE
);

INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('Utf8SettingDE', 'äöüß');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('Utf8SettingPL', 'ąęśćżźó');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('ArraySetting[0]', '3');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('ArraySetting[1]', '7');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('DictionarySetting[foo]', '4');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('DictionarySetting[bar]', '8');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('NestedConfig.StringSetting', 'Bar');
INSERT OR REPLACE INTO Setting([Name], [Value]) VALUES('IgnoredConfig.StringSettingDE', 'Qux');

