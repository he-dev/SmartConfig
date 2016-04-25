CREATE TABLE IF NOT EXISTS Setting( 
        [Name] TEXT NOT NULL,
        [Value] TEXT NOT NULL, 
        [Environment] TEXT NOT NULL, 
        PRIMARY KEY([Name], [Environment]));

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('Foo','SQLiteStore', 'Bar');

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('DE','SQLiteStore', 'äöüß');

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('PL','SQLiteStore', 'ąęśćżźó');

