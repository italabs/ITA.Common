DECLARE
  p_usr_name   VARCHAR2(30)  := '@login';         -- NEW USER NAME HERE
  p_usr_pass   VARCHAR2(30)  := '@password';      -- NEW USER PASSWORD HERE
  -----------------------------------------------------------------------------
  -----------------------------------------------------------------------------
  obj_found    NUMBER(1) DEFAULT 0;
  ins_clause VARCHAR2(100);

BEGIN
--
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


obj_found := 0;

SELECT COUNT(*) INTO obj_found
  FROM SYS.DBA_USERS
  WHERE USERNAME = p_usr_name;

IF obj_found = 0
  THEN
  EXECUTE IMMEDIATE 'CREATE USER "'||p_usr_name||'" 
                       IDENTIFIED BY '||p_usr_pass||' 
                       ACCOUNT UNLOCK';
  EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO "'||p_usr_name||'"';                     
  EXECUTE IMMEDIATE 'GRANT CREATE ANY SEQUENCE,
			       DROP ANY SEQUENCE,
			       SELECT ANY SEQUENCE,
	       
			       ALTER SESSION,
       CREATE ANY INDEX,
       DROP ANY INDEX,
       CREATE ANY TRIGGER,
       DROP ANY TRIGGER,
			       CREATE ANY TABLE,
			       ALTER ANY TABLE,
			       DELETE ANY TABLE,
			       DROP ANY TABLE,
			       UPDATE ANY TABLE,
			       INSERT ANY TABLE,
			       SELECT ANY TABLE
				TO "'|| p_usr_name || '"';

END IF;

END;