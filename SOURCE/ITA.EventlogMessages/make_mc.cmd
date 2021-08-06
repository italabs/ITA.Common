@echo off
echo LanguageNames=(Neutral=0x0:MSG00000)

echo MessageId=0
echo Language=Neutral
echo %%1%%n%%n
echo .
echo.

for /L %%i in (1000,1,4095) do (

echo MessageId=%%i
echo Language=Neutral
echo %%1%%n%%n
echo .
echo.
)