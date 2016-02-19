param (
	$installPath,
	$toolsPath,
	$package,
	$project
)

[xml]$xml = Get-Content $project.FullName

$nodes = $xml.SelectNodes("/Project/PropertyGroup")
$nodes.ChildNodes | ? { 
	$_.Name -eq "StartAction" -or $_.Name -eq "StartProgram" } | % {
	$_.ParentNode.RemoveChildNode($_)
}

$project.Save()

#$writerSettings = new-object System.Xml.XmlWriterSettings
#$writerSettings.OmitXmlDeclaration = $false
#$writerSettings.NewLineOnAttributes = $false
#$writerSettings.Indent = $true
#$projectFilePath = Resolve-Path -Path $project.FullName
#$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
#$xml.WriteTo($writer)
#$writer.Flush()
#$writer.Close()
