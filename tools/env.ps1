$workingDir = Split-Path $MyInvocation.MyCommand.Path
$certificatePath = $workingDir + "\BrokerSSL.pfx"
$password = "admin1243"
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certificatePath, $password, "Exportable,PersistKeySet")
$thumbprint = $cert.Thumbprint.ToUpper()
$bindingIpAndPort = "0.0.0.0:81"
$appid = "{DF502D81-3309-4AAB-B115-D3D6379BE156}"


Write-Host "Working in: " $workingDir

Write-Host "Verifying certificate exists within store"
Write-Host "Using certificate: ${certificatePath}"
Write-Host "Certificate's thumbprint is: " $cert.Thumbprint

$certExists = Test-Path "Cert:\LocalMachine\Root\${thumbprint}" 

if ($certExists)
{
	Write-Host "Nothing to do, certificate is installed."
}
else
{
	Write-Host "Certificate does not exist."
	Write-Host "Installing Certificate."

	$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
	$store.Open("ReadWrite")
	$store.Add($cert)
	$store.Close()	

	Write-Host "Certificate installed into root certificate store."
}

Write-Host "Binding ${thumbprint} to ${bindingIpAndPort}"

try{
	Write-Host "Ensuring the ssl certificate is not bound on ${bindingIpAndPort}"

	& netsh http delete sslcert ipport="$(bindingIpAndPort)"
}
catch {

}

Write-Host "Creating SSL Binding..."
& netsh http add sslcert ipport="$bindingIpAndPort" certhash="$thumbprint" appid="$appid" certstorename=Root