REM %1 = days not touched
set workdir=C:\temp\StpLog
set mydir=Remover
IF NOT EXIST %workdir%\%mydir% mkdir %workdir%\%mydir%
set tempdir=%RANDOM%
del %workdir%\%mydir%\Delete_*.txt
REM Move
ROBOCOPY %workdir% %workdir%\%tempdir% /S /MOV /MINLAD:%1 /XD %workdir%\%mydir% > %workdir%\%mydir%\Delete_%tempdir%.txt
REM Delete Files
ROBOCOPY %workdir% %workdir% /S /MOVE /MINAGE:%1 /XD %workdir%\%mydir% 
REM Delete remaining directories
rmdir %workdir%\%tempdir% /S /q >> %workdir%\%mydir%\Delete_%tempdir%.txt
