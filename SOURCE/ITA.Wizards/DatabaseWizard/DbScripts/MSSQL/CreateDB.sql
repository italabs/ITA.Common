--IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '@DB')
BEGIN
CREATE DATABASE [@DB]
END

GO
USE [@DB]
