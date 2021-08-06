DECLARE
  obj_found NUMBER;
  p_usr_name   VARCHAR2(30)  := '@DB';        -- NEW SCHEMA NAME HERE
  p_usr_pass   VARCHAR2(30)  := '@DB';        -- NEW SCHEMA PASSWORD HERE ( NOT NECESSARY )
BEGIN

--Oracle naming rules support.
--
--1. Quoted usernames can't contain lower case symbols. (Quoting is required to support spaces in names) 
--   This fact doesn't actually limits users to login with lower-case symbols due to the case-NON-sensitive Oracle authorisation.
--   E.g. user with login "Big Brother" will be stored in Oracle as "BIG BROTHER".
--   Attempt of login as "Big Brother" will be successful.
--
--2. The same here about passwords. Yeah..... Oracle has case-NON-sensitive passwords authorisation!   

p_usr_name := UPPER(p_usr_name);
p_usr_pass := UPPER(p_usr_pass);

-- CREATE TABLESPACE

obj_found := 0;

SELECT COUNT(*) INTO obj_found
  FROM SYS.DBA_TABLESPACES 
  WHERE TABLESPACE_NAME = p_usr_name;
  
IF ( obj_found = 0 )
  THEN
  EXECUTE IMMEDIATE 'CREATE TABLESPACE "' || p_usr_name ||'" DATAFILE ''@DB.dat'' SIZE 100M REUSE AUTOEXTEND ON NEXT 10M';
END IF;

-- CREATE SCHEMA


obj_found := 0;

SELECT COUNT(*) INTO obj_found
  FROM SYS.DBA_USERS
  WHERE USERNAME = p_usr_name;

IF obj_found = 0
  THEN
  EXECUTE IMMEDIATE 'CREATE USER "'||p_usr_name||'" 
                       IDENTIFIED BY '||p_usr_pass||' 
                       DEFAULT TABLESPACE ' || p_usr_name || 
                       ' TEMPORARY TABLESPACE temp
                       ACCOUNT LOCK';
  EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO "'||p_usr_name||'"';                     
END IF;

----------------------------

EXECUTE IMMEDIATE 'CREATE OR REPLACE FUNCTION ' ||p_usr_name || '.TABLE_EXISTS(
                                           t_name IN VARCHAR2
                                           )
                                           RETURN NUMBER 
                                           AUTHID DEFINER IS
 co NUMBER(1);
BEGIN
 select count(*) into co from user_tables WHERE TABLE_NAME = t_name;
 return co;
END;';

----------------------------

EXECUTE IMMEDIATE 'CREATE OR REPLACE FUNCTION ' ||p_usr_name || '.SEQUENCE_EXISTS(
                                           s_name IN VARCHAR2
                                           )
                                           RETURN NUMBER 
                                           AUTHID DEFINER IS
 co NUMBER(1);
BEGIN
 select count(*) into co from user_sequences WHERE SEQUENCE_NAME = s_name;
 return co;
END;';

----------------------------

EXECUTE IMMEDIATE 'CREATE OR REPLACE FUNCTION ' ||p_usr_name || '.TRIGGER_EXISTS(
                                           t_name IN VARCHAR2
                                           )
                                           RETURN NUMBER 
                                           AUTHID DEFINER IS
 co NUMBER(1);
BEGIN
 select count(*) into co from user_triggers WHERE TRIGGER_NAME = t_name;
 return co;
END;';

----------------------------

  EXECUTE IMMEDIATE 'grant execute on ' || p_usr_name || '.table_exists to public';
  EXECUTE IMMEDIATE 'grant execute on ' || p_usr_name || '.sequence_exists to public';
  EXECUTE IMMEDIATE 'grant execute on ' || p_usr_name || '.trigger_exists to public';

END;