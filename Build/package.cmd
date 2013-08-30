call ..\..\..\..\..\paths.cmd

set projectDir=%~dp0..\
set buildDir=%~dp0package-files\
set packageName=avt.ActionForm_%1_Install

"%OwnToolsPath%xpath-update\bin\release\xpath-update.exe" --value="%1" --xpath="//packages/package" -attr="version" --file="avt.ActionForm.dnn"
"%OwnToolsPath%xpath-update\bin\release\xpath-update.exe" --value="%1" --xpath="//assemblies/assembly[1]/version" --file="avt.ActionForm.dnn"
"%OwnToolsPath%xpath-update\bin\release\xpath-update.exe" --value="%1" --xpath="//assemblies/assembly[2]/version" --file="avt.ActionForm.dnn"
"%OwnToolsPath%xpath-update\bin\release\xpath-update.exe" --value="%1" --xpath="//scripts/script[@type='UnInstall']/version" --file="avt.ActionForm.dnn"
echo %1 > version.txt

xcopy "%projectDir%templates\*.*" "%buildDir%templates\*.*" /e /s /y /q
xcopy "%projectDir%Config\*.*" "%buildDir%Config\*.*" /e /s /y /q
xcopy "%projectDir%js\*.*" "%buildDir%js\*.*" /e /s /y /q
xcopy "%projectDir%static\*.*" "%buildDir%static\*.*" /e /s /y /q
xcopy "%projectDir%RegCore\res\*.*" "%buildDir%RegCore\res\*.*" /e /s /y /q
xcopy "%projectDir%RegCore\*.aspx" "%buildDir%RegCore\*.aspx" /e /s /y /q
xcopy "%projectDir%RegCore\*.ascx" "%buildDir%RegCore\*.ascx" /e /s /y /q
copy "%projectDir%bin\release\avt.ActionForm.dll" "%buildDir%avt.ActionForm.dll"
copy "%projectDir%bin\release\avt.ActionForm.Core.dll" "%buildDir%avt.ActionForm.Core.dll"
copy "%projectDir%bin\release\LumenWorks.Framework.IO.dll" "%buildDir%LumenWorks.Framework.IO.dll"
xcopy "%projectDir%DataProviders\*.*" "%buildDir%DataProviders\*.*" /e /s /y /q
xcopy "%projectDir%*.css" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.ascx" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.aspx" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.ashx" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.txt" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.html" "%buildDir%*.*" /y /q
xcopy "%projectDir%*.dnn" "%buildDir%*.*" /y /q
xcopy "%projectDir%App_LocalResources\*.resx" "%buildDir%App_LocalResources\*.*" /y /q

rem minify our resources
java -jar "%buildDir%..\yuicompressor-2.4.2.jar" "%buildDir%/static/admin.js" -o "%buildDir%/static/admin.js" -v --charset utf8
java -jar "%buildDir%..\yuicompressor-2.4.2.jar" "%buildDir%/static/admin.css" -o "%buildDir%/static/admin.css" -v --charset utf8

rem remove_codefile_attrbute.pl

del "%buildDir%Config\Validators\MaxCompanyLength.xml" /Q

cd "%buildDir%"
for %%i in (*.as?x) do "%ToolsPath%UnixTools\sed.exe" -e "s/CodeFile[^=]*=/CodeBehind=/ig" "%%i" > "%%i-1" && move "%%i-1" "%%i"

cd "%buildDir%RegCore\"
for %%i in (*.as?x) do "%ToolsPath%UnixTools\sed.exe" -e "s/CodeFile\s*=\"/CodeBehind=\"/ig" "%%i" > "%%i-1" && move "%%i-1" "%%i"

cd "%buildDir%"

"%ToolsPath%infozip\zip.exe" -r -9 Resources.zip Config App_LocalResources RegCore templates js static *.ascx *.aspx *.ashx *.css  >>..\log.txt
"%ToolsPath%infozip\zip.exe" -r -9 "..\%packageName%.zip"  Resources.zip DataProviders *.txt Resources.zip *.dnn *.txt *.html *.dll >>..\log.txt
cd..


"%ToolsPath%s3.exe" put dl.dnnsharp.com/AFORM/ %packageName%.zip /key:%S3Key% /secret:%S3Secret%

echo http://dl.dnnsharp.com/AFORM/%packageName%.zip
echo http://dl.dnnsharp.com/AFORM/%packageName%.zip | clip


