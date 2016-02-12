function RunTestsWithCoverage {

    $configuration = 'Debug'
    $rootDir = Resolve-Path '.\'
    $reportsDir = Resolve-Path '.\'
    $testDir = Resolve-Path '.\tests'

    $dotCover = "$rootDir\..\..\tools\dotCover\dotCover.exe"
    $xunitRunner = "$rootDir\packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe"

    Write-Host $dotCover

    if (!(Test-Path $dotCover)) {
        #not teamcity build - just run xunit
        Get-ChildItem  $testDir -Recurse -Include *Tests.csproj, Tests.*.csproj | % {
            $project = $_.BaseName

            if(!(Test-Path $reportsDir\xUnit\$project)) {
                New-Item $reportsDir\xUnit\$project -Type Directory
            }

            .$xunitRunner "$testDir\$project\bin\$configuration\$project.dll" -html "$reportsDir\xUnit\$project\index.html"
        }
    }
    else{
        #dotCover is available - execute tests with coverage
        $dotCover = Resolve-Path $dotCover
        Get-ChildItem  $testDir -Recurse -Include *Tests.csproj, Tests.*.csproj | % {
            $project = $_.BaseName
    
            if(!(Test-Path $reportsDir\xUnit\$project)) {
                New-Item $reportsDir\xUnit\$project -Type Directory
            }
    
            $testAssembly = "$testDir\$project\bin\$configuration\$project.dll"
            .$dotCover cover /AnalyseTargetArguments=False /TargetExecutable="$xunitRunner" /TargetArguments="$testAssembly" /Output="$reportsDir\xUnit\$project\$project.dcvr"
            TeamCity-ImportDotNetCoverageResult "dotcover" "$reportsDir\xUnit\$project\$project.dcvr"
        }
    }
}

function TeamCity-ImportDotNetCoverageResult ([String]$toolName, [String]$importFileLocation) {
    Write-Host ("##teamcity[importData type='dotNetCoverage' tool='{0}' path='{1}']" -f $toolName, $importFileLocation)
}

RunTestsWithCoverage