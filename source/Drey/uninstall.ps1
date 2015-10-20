param (
	$InstallPath,
	$ToolsPath,
	$Package,
	$Project
)

$Project.Properties.Item("StartAction").Value = [int]3; # https://msdn.microsoft.com/en-us/library/aa983990(v=vs.71).aspx - Corresponds to prjStartActionNone
$Project.Properties.Item("StartProgram").Value = "";