DECLARE @AuthoritySystem nvarchar(500)
SET @AuthoritySystem = SUSER_SNAME(0x010100000000000512000000)

--0x010100000000000512000000 - Это бинарное представление well-known SID S-1-5-18 учетной записи NT AUTHORITY\SYSTEM
--SELECT SUSER_SID('NT AUTHORITY\SYSTEM')

if not exists (select * from dbo.sysusers where name = @AuthoritySystem and uid < 16382)
BEGIN
	EXEC sp_grantdbaccess @AuthoritySystem, @AuthoritySystem
END

exec sp_addrolemember N'db_datareader', @AuthoritySystem
exec sp_addrolemember N'db_datawriter', @AuthoritySystem
exec sp_addrolemember N'db_owner', @AuthoritySystem