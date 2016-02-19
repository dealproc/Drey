properties {
	$version = "1.0.0.0"
	$versionSeries = $version
	$versionInfo = $version

	$projectName = "Drey"
	$rootDir = Resolve-Path .\
	$artifactsDir = "$rootDir\package"
	$reportsDir = "$rootDir\reports"
    $testDir = "$rootDir\tests"
	$sourceDir = "$rootDir\source"
	$dotCoverFolder = ""
	$configuration = "Release"
}

task default -depends Clean, UpdateVersion, Compile, RunTests, Package

task Clean {
	Remove-Item $artifactsDir -Force -Recurse -ErrorAction SilentlyContinue
	Remove-Item $reportsDir -Force -Recurse -ErrorAction SilentlyContinue

	exec { msbuild Drey.sln /nologo /verbosity:quiet /p:PlatformTarget=x86 /p:Configuration=$configuration /t:Clean }
}

task UpdateVersion {
	$assemblyInfoFilePath = "$rootDir\CommonAssemblyInfo.cs";
	$tmpFile = $assemblyInfoFilePath + ".tmp";

	$newAssemblyVersion = 'AssemblyVersion("' + $versionSeries + '")';
	$newAssemblyFileVersion = 'AssemblyFileVersion("' + $version + '")';
	$newAssemblyInfoVersion = 'AssemblyInformationalVersion("' + $versionInfo + '")';

	# - Keeping these for debugging purposes.
	#Write-Host "Assembly Version: $newAssemblyVersion";
	#Write-Host "Assembly File Version: $newAssemblyFileVersion";
	#Write-Host "Assembly Info Version: $newAssemblyInfoVersion";

	Get-Content $assemblyInfoFilePath |
		%{$_ -replace 'AssemblyVersion\("(\d+)\.(\d+)(\.(\d+)\.(\d+)|\.*)"\)', $newAssemblyVersion }  |
		%{$_ -replace 'AssemblyFileVersion\("(\d+)\.(\d+)(\.(\d+)\.(\d+)|\.*)"\)', $newAssemblyFileVersion } |
		%{$_ -replace 'AssemblyInformationalVersion\("(\d+)\.(\d+)(\.(\d+)\.(\d+)|\.*)"\)', $newAssemblyInfoVersion } |
		Out-File -Encoding UTF8 $tmpFile

	Move-Item $tmpFile $assemblyInfoFilePath -force
}

task Compile {
	exec { msbuild Drey.sln /nologo /verbosity:quiet /p:PlatformTarget=x86 /p:Configuration=$configuration /p:PackagePath=$artifactsDir /p:Version=$version /t:Rebuild }
}

task RunTests {
    if ($dotCoverFolder) {
        $dotCover = "$dotCoverFolder\dotCover.exe"
    } else {
        $dotCover = "$rootDir\..\..\tools\dotCover\dotCover.exe"
    }

    $xunitRunner = "$rootDir\packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe"

    Write-Host $dotCover

	$dotCoverAvailableForUse = (Test-Path $dotCover)

	$allPassed = $true
	
	Get-ChildItem  $testDir -Recurse -Include *Tests.csproj, *Tests.*.csproj | % {
		$project = $_.BaseName

		if(!(Test-Path $reportsDir\xUnit\$project)) {
			New-Item $reportsDir\xUnit\$project -Type Directory
		}

		try { 
			if ($dotCoverAvailableForUse) {
				#dotCover is available - execute tests with coverage
				$testAssembly = "$testDir\$project\bin\$configuration\$project.dll"
				exec { .$dotCover cover /AnalyseTargetArguments=False /TargetExecutable="$xunitRunner" /TargetArguments="$testAssembly" /Output="$reportsDir\xUnit\$project\$project.dcvr" }
				TeamCity-ImportDotNetCoverageResult "dotcover" "$reportsDir\xUnit\$project\$project.dcvr"
			} else {
				#not teamcity build - just run xunit
				exec { .$xunitRunner "$testDir\$project\bin\$configuration\$project.dll" -html "$reportsDir\xUnit\$project\index.html" }
			}
		} catch {
			$allPassed = $false
		}
	}

	if ($allPassed -ne $true) { 
		Write-Error "One or more tests failed."
	}
}

task Package {
	if (!(Test-Path $artifactsDir)) {
		New-Item $artifactsDir -Type Directory
	}

	Get-ChildItem "$rootDir" -Recurse -Include *.nuspec -Exclude Drey.nuspec,Drey.Sdk.nuspec | % {
		$project = $_.BaseName
		$nuspec = $_.FullName
		
		Write-Host "Creating nupkg for: $project"
		Write-Host "Project Folder: $projectFolder"
		.nuget\nuget.exe pack "$nuspec" -NoDefaultExcludes -NoPackageAnalysis -OutputDirectory "$artifactsDir" -Version "$versionInfo" -Properties "Configuration=$configuration"
	}

	.nuget\nuget.exe pack ".\source\Drey\Drey.nuspec" -NoDefaultExcludes -NoPackageAnalysis -OutputDirectory "$artifactsDir" -Version "$versionInfo" -Properties "Configuration=$configuration"
	.nuget\nuget.exe pack ".\source\Drey\Drey.Sdk.nuspec" -NoDefaultExcludes -NoPackageAnalysis -OutputDirectory "$artifactsDir" -Version "$versionInfo" -Properties "Configuration=$configuration"
}