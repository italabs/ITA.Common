USE @DB

-- При попытке добавить права системной роли sa - возникает ошибка - исправляем
if '@login' <> 'sa'
begin
	if not exists (select * from master.dbo.syslogins where loginname = '@login')
	BEGIN
		exec sp_addlogin [@login], [@password], [@DB], N'us_english'
	END

	DECLARE @user_name sysname 

	--Смотрим маппинг логина и пользователя
	SELECT @user_name = U.name
	FROM sys.sql_Logins l
	INNER JOIN
	sys.database_principals u
	ON l.sid = u.SID
	WHERE u.principal_id < 16382
	AND l.name = '@login'

	IF @user_name IS NULL
		BEGIN 
		    
			-- Если в БД нет пользователя, создаем нового. Имя логина = имя пользователя.
			EXEC sp_grantdbaccess [@login], [@login]

			EXEC sp_addrolemember N'db_datareader', [@login]
			EXEC sp_addrolemember N'db_datawriter', [@login]
			EXEC sp_addrolemember N'db_owner', [@login]
		END
	ELSE
		BEGIN
		    -- Если в БД уже есть пользователь - предоставляем необходимые права текущему пользователю - если он не владелец БД
			IF (@user_name <> 'dbo')
				BEGIN
					EXEC sp_addrolemember N'db_datareader',  @user_name
					EXEC sp_addrolemember N'db_datawriter',  @user_name
					EXEC sp_addrolemember N'db_owner',  @user_name
				END
		END
end;