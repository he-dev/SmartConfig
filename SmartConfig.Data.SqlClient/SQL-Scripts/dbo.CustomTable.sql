﻿CREATE TABLE [dbo].[CustomTable]
(
	[Environment] NVARCHAR(50) NOT NULL , 
    [Version] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(255) NOT NULL, 
    [Value] NVARCHAR(MAX) NULL, 
    PRIMARY KEY ([Environment], [Name], [Version])
)