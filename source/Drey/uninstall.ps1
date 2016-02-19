param (
	$InstallPath,
	$ToolsPath,
	$Package,
	$Project
)

$Project.Save();

[xml]$xml = Get-Content $Project.FullName

$startAction = $xml | Select-Xml -XPath "(//Project/PropertyGroup/StartAction)[1]"
$startProgram = $xml | Select-Xml -XPath "(//Project/PropertyGroup/StartProgram)[1]"

if ($startAction){
	$xml.RemoveChild($startAction)
}
if ($startProgram) {
	$xml.RemoveChild($startProgram)
}

$Project.Save();