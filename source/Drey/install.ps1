param (
	$installPath,
	$toolsPath,
	$package,
	$project
)

# Find Debug configuration and set debugger settings.

Write-Host "`tSetting startup information in Debug configuration"

$project.Save()

[xml]$prjXml = Get-Content $project.FullName

foreach ($PropertyGroup in $prjXml.project.ChildNodes)
{
	if ($PropertyGroup.StartAction -ne $null)
	{
		exit
	}
}

$propertyGroupElement = $prjXml.CreateElement("PropertyGroup", $prjXml.Project.GetAttribute("xmlns"));
$startActionElement = $prjXml.CreateElement("StartAction", $prjXml.Project.GetAttribute("xmlns"));
$propertyGroupElement.AppendChild($startActionElement)
$propertyGroupElement.StartAction = "Program"
$startProgramElement = $prjXml.CreateElement("StartProgram", $prjXml.Project.GetAttribute("xmlns"));
$propertyGroupElement.AppendChild($startProgramElement)
$propertyGroupElement.StartProgram = "`$(SolutionDir)Runtime\Runtime.exe"
$prjXml.project.AppendChild($propertyGroupElement);
$writerSettings = new-object System.Xml.XmlWriterSettings
$writerSettings.OmitXmlDeclaration = $false
$writerSettings.NewLineOnAttributes = $false
$writerSettings.Indent = $true
$projectFilePath = Resolve-Path -Path $project.FullName
$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
$prjXml.WriteTo($writer)
$writer.Flush()
$writer.Close()
