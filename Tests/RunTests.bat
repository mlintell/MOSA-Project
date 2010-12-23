CD ..\Source
CALL Compile-Debug.bat
CD ..\Tests

IF EXIST "%ProgramFiles(x86)%\Gallio\bin\Gallio.Echo.exe" SET GALLIO="%ProgramFiles(x86)%\Gallio\bin\Gallio.Echo.exe"
IF EXIST "%ProgramFiles%\Gallio\bin\Gallio.Echo.exe" SET GALLIO="%ProgramFiles%\Gallio\bin\Gallio.Echo.exe"

CD ..\bin

%GALLIO% /rnf:Test /rt:Xml-Inline /report-directory:..\Tests\reports Mosa.Test.Runtime.CompilerFramework.dll

CALL ExtractResults.BAT

pause
