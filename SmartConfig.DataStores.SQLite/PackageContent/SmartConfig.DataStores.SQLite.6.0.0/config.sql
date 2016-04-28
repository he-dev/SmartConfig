CREATE TABLE IF NOT EXISTS Setting( 
        [Name] TEXT NOT NULL,
        [Value] TEXT NOT NULL, 
        [Environment] TEXT NOT NULL, 
        PRIMARY KEY([Name], [Environment]));

INSERT OR REPLACE INTO Setting([Name], [Environment], [Value])
VALUES('Greeting','SQLiteStore', 'Hallo SmartConfig!');

