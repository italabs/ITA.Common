-- Заводит логин и добавляет гранты для учетной записи DOMAIN\account (DOMAIN\NETBIOS$)

-- https://msdn.microsoft.com/en-us/library/ms189751.aspx
-- When you are creating logins that are mapped from a Windows domain account, 
-- you must use the pre-Windows 2000 user logon name in the format [<domainName>\<login_name>]. 
-- You cannot use a UPN in the format login_name@DomainName. 


if not exists (select * from master.dbo.syslogins where loginname = '@WS')
BEGIN
	CREATE LOGIN [@WS] FROM WINDOWS WITH DEFAULT_DATABASE=[master]	
END

--Под заданный windows-логин в БД может быть уже создан пользователь
DECLARE @db_user sysname
SET @db_user = '@WS'

if not exists (select dp.name
						from sys.server_principals sp
						join sys.database_principals dp on dp.sid = sp.sid
						where sp.name = '@WS')
BEGIN
	EXEC sp_grantdbaccess [@WS], [@WS]
END

select @db_user = dp.name
						from sys.server_principals sp
						join sys.database_principals dp on dp.sid = sp.sid
						where sp.name = '@WS'

IF lower(@db_user) != 'dbo'
BEGIN
	exec sp_addrolemember N'db_datareader', @db_user
	exec sp_addrolemember N'db_datawriter', @db_user
	exec sp_addrolemember N'db_owner', @db_user
END