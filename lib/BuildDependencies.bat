"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\LoggingServices\src\LoggingServices\bin\Release" ".\Dependency Builds\LoggingServices\DLLs\"

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Net35Essentials\src\Net35Essentials\bin\Release" ".\Dependency Builds\Net35Essentials\DLLs\"

PAUSE
