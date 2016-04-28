CREATE TABLE [dbo].[Setting]
(
    [Name] NVARCHAR(255) NOT NULL, 
    [Value] NVARCHAR(MAX) NULL, 
	[Environment] NVARCHAR(50) NOT NULL , 
    [Version] NVARCHAR(50) NOT NULL, 
    PRIMARY KEY ([Environment], [Name], [Version])
)
