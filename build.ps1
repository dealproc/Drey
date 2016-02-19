param(
	[int] $buildNumber = 0,
	[string] $branch = "master",
	[string] $dotCoverFolder = ""
)

$majorMinorVersion = "0.1"

$version = "$majorMinorVersion.$buildNumber.0"
$versionSeries = "$majorMinorVersion.0.0"
$versionInfo = if ($branch -eq "master" -or $branch -eq "") { $version } else { "$version-$branch" }

"Product Version: `t$version"
"Product Series: `t$versionSeries"
"Informational Version: `t$versionInfo"

$packageConfigs = Get-ChildItem . -Recurse | where {  $_.Name -eq "packages.config" }

# Restores nuget packages.
foreach ($packageConfig in $packageConfigs) {
	Write-Host "Restoring $packageConfig.FullName"
	.\.nuget\nuget.exe install $packageConfig.FullName -o packages\
}

Import-Module .\packages\psake.4.5.0\tools\psake.psm1
Import-Module .\packages\psake-contrib.1.1.0\tools\teamcity.psm1
TeamCity-SetBuildNumber($versionInfo)
exec { Invoke-Psake .\default.ps1 default -framework "4.5.2" -properties @{ version=$version; versionSeries = $versionSeries; versionInfo = $versionInfo; dotCoverFolder = $dotCoverFolder } }
Remove-Module teamcity
Remove-Module psake