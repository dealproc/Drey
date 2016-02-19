param (
	$installPath,
	$toolsPath,
	$package,
	$project
)

# Find Debug configuration and set debugger settings.

Write-Host "`tSetting dll to run an external program for running applet."

$project.Save()

[xml]$xml = Get-Content $project.FullName

if ($xml.Project.PropertyGroup[0].StartAction -ne $null) { exit 0 }

$startAction = $xml.CreateElement("StartAction", $xml.DocumentElement.NamespaceURI)
$startAction.InnerText = "Program"
$startProgram = $xml.CreateElement("StartProgram", $xml.DocumentElement.NamespaceURI)
$startProgram.InnerText = "`$(SolutionDir)Runtime\Runtime.exe"

$xml.Project.PropertyGroup[0].AppendChild($startAction)
$xml.Project.PropertyGroup[0].AppendChild($startProgram)

$writerSettings = new-object System.Xml.XmlWriterSettings
$writerSettings.OmitXmlDeclaration = $false
$writerSettings.NewLineOnAttributes = $false
$writerSettings.Indent = $true
$projectFilePath = Resolve-Path -Path $project.FullName
$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
$xml.WriteTo($writer)
$writer.Flush()
$writer.Close()
