REM %1 = days not touched
set workdir=C:\temp\StpLog
set mydir=Remover
set tempdir=%RANDOM%
del %workdir%\%mydir%\Delete_*.txt
ROBOCOPY %workdir% %workdir%\%tempdir% /S /MOV /MINLAD:%1 /XD %workdir%\%mydir% > %workdir%\%mydir%\Delete_%tempdir%.txt
ROBOCOPY %workdir% %workdir% /S /MOVE /MINAGE:%1 /XD %workdir%\%mydir%
rmdir %workdir%\%tempdir% /S /q >> %workdir%\%mydir%\Delete_%tempdir%.txt
