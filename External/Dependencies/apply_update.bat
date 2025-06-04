@echo off
REM -----------------------------
REM apply_update.bat
REM -----------------------------
REM Usage (from C# or manually):
REM   apply_update.bat program.exe program.update.exe
REM -----------------------------

REM Ensure two arguments: %1 = original EXE, %2 = update EXE
IF "%~2"=="" (
    ECHO Usage: %~nx0 original.exe update.exe
    EXIT /B 1
)

SET "ORIGINAL_EXE=%~1"
SET "UPDATE_EXE=%~2"

REM Extract just the process name (no path)
FOR %%F IN ("%ORIGINAL_EXE%") DO SET "ORIG_NAME=%%~nxF"
FOR %%G IN ("%UPDATE_EXE%")   DO SET "UPD_NAME=%%~nxG"

REM Full paths (in case you want paths rather than just names)
FOR %%F IN ("%ORIGINAL_EXE%") DO SET "ORIG_PATH=%%~fF"
FOR %%G IN ("%UPDATE_EXE%")   DO SET "UPD_PATH=%%~fG"

ECHO Waiting for "%ORIG_NAME%" to exit...
:WaitForExit
    REM Check if process is still running
    tasklist /FI "IMAGENAME eq %ORIG_NAME%" 2>NUL | find /I "%ORIG_NAME%" >NUL
    IF NOT ERRORLEVEL 1 (
        REM Process still exists → wait 1 second and loop
        TIMEOUT /T 1 /NOBREAK >NUL
        GOTO WaitForExit
    )

REM At this point, original EXE is no longer running.
ECHO "%ORIG_NAME%" has exited. Applying update...

REM Delete existing original if it still exists (just in case)
IF EXIST "%ORIG_PATH%" (
    DEL /F /Q "%ORIG_PATH%"
)

REM Move (rename) update.exe → original.exe
MOVE /Y "%UPD_PATH%" "%ORIG_PATH%" >NUL 2>&1

IF ERRORLEVEL 1 (
    ECHO Failed to rename "%UPD_NAME%" → "%ORIG_NAME%".
    PAUSE
    EXIT /B 2
)

ECHO Update applied: "%UPD_NAME%" renamed to "%ORIG_NAME%".

REM (Optional) Restart the updated EXE
START "" "%ORIG_PATH%"

EXIT /B 0
