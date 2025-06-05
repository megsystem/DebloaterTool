@echo off
set msbuild="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
%msbuild% "DebloaterTool.sln" /t:Rebuild /p:Configuration=Release
%msbuild% "DebloaterTool.sln" /t:Rebuild /p:Configuration=Debug
echo  ,ggggggggggg,                                                           
echo dP"""88""""""Y8,                   ,dPYb,         8I                     
echo Yb,  88      `8b                   IP'`Yb         8I                     
echo  `"  88      ,8P              gg   I8  8I         8I                     
echo      88aaaad8P"               ""   I8  8'         8I                     
echo      88""""Y8ba  gg      gg   gg   I8 dP    ,gggg,8I   ,ggg,    ,gggggg, 
echo      88      `8b I8      8I   88   I8dP    dP"  "Y8I  i8" "8i   dP""""8I 
echo      88      ,8P I8,    ,8I   88   I8P    i8'    ,8I  I8, ,8I  ,8'    8I 
echo      88_____,d8',d8b,  ,d8b,_,88,_,d8b,_ ,d8,   ,d8b, `YbadP' ,dP     Y8,
echo     88888888P"  8P'"Y88P"`Y88P""Y88P'"Y88P"Y8888P"`Y8888P"Y8888P      `Y8