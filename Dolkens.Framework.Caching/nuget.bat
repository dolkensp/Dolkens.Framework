@echo Uploading to nuget
erase *.nupkg /Q /F
for /f "delims=|" %%f in ('dir /b *.csproj') do ..\.nuget\NuGet.exe pack %%f -build -properties Configuration=Release
..\.nuget\Nuget.exe push *.nupkg -Source https://api.nuget.org/v3/index.json