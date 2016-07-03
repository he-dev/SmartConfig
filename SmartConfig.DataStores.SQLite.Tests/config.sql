CREATE TABLE IF NOT EXISTS Setting( 
        [Name] TEXT NOT NULL,
        [Value] TEXT NOT NULL, 
        [Environment] TEXT NULL, 
        PRIMARY KEY([Name], [Environment]));

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('Foo', NULL, 'Bar');

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('Bar', "Baz", 'Qux');

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('DE','UTF8', 'äöüß');

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('PL','UTF8', 'ąęśćżźó');

