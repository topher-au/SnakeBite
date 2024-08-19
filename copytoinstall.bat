REM Sync with files listed in .nsi
echo copytoinstall.bat
set solutionDir=%~1
set targetDir=%~2
xcopy /y %solutionDir%SnakeBite.nsi %solutionDir%installerbuild\
xcopy /y %solutionDir%fpk_dictionary.txt %solutionDir%installerbuild\
xcopy /y %solutionDir%qar_dictionary.txt %solutionDir%installerbuild\
xcopy /y %solutionDir%README.md %solutionDir%installerbuild\
xcopy /y %solutionDir%ChangeLog.txt %solutionDir%installerbuild\
xcopy /y %solutionDir%license.txt %solutionDir%installerbuild\
xcopy /y %solutionDir%mgsvfile.ico %solutionDir%installerbuild\
xcopy /y %solutionDir%mgsvpreset.ico %solutionDir%installerbuild\

xcopy /y %targetDir%SnakeBite.exe %solutionDir%installerbuild\
xcopy /y %targetDir%SnakeBite.exe.config %solutionDir%installerbuild\
REM xcopy /y %targetDir%MakeBite.exe %solutionDir%installerbuild\ - done from makebites solution
REM xcopy /y %targetDir%MakeBite.exe.config %solutionDir%installerbuild\ - done from makebites solution
xcopy /y %targetDir%CityHash.dll %solutionDir%installerbuild\
xcopy /y %targetDir%GzsTool.Core.dll %solutionDir%installerbuild\
xcopy /y %targetDir%ICSharpCode.SharpZipLib.dll %solutionDir%installerbuild\
xcopy /y %targetDir%Zlib.Portable.dll %solutionDir%installerbuild\