#Always start Azure scripts with this, to change your subscriptionid edit AzureLogin.ps1:
$scriptPath = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. ("$scriptPath\AzureLogin.ps1")

#KeyVault: https://docs.microsoft.com/sv-se/cli/azure/keyvault?view=azure-cli-latest

"Active subscription $subscription"

##############################
# Setup Azure infrastructure #
##############################
$resourceGroup = "LennartKVRG2"
$location = "northeurope"

$keyvaultName = "LennartKV2"
$secretName = "AppSecret--MyAppSecret"
$secretValue = "This is a secret from keyvault"
$appName = "LennartKVApp"

$keyVaultNames = az keyvault list --query '[].[name]' --output tsv
if (!($keyVaultNames -Contains $KeyVaultName)) 
{
	Write-Output "Creating resource group $resourceGroup"
	$json = az group create --name $resourceGroup --location $location
	$json | Out-File -FilePath "$logPath\$resourceGroup.json"
	$group = $json | ConvertFrom-Json
	Write-Output "Your properties for the resourcegroup is stored in $resourceGroup.json"

	Write-Output "Creating a RBAC SP and assign Contributor rights to the resource group that will contain the key vault"
	$json = az ad sp create-for-rbac -n "http://$appName" --role contributor --scopes /subscriptions/$subscription/resourceGroups/$resourceGroup
	$json | Out-File -FilePath "$logPath\$appName.json"
	$rbac = $json | ConvertFrom-Json
	$clientId = $rbac.appId
	$clientSecret = $rbac.password
	Write-Output "Your properties for the RBAC SP is stored in $appName.json"

	Write-Output "Creating the KeyVault"
	$json = az keyvault create --location $location --name $keyvaultName --resource-group $resourceGroup
	$json | Out-File -FilePath "$logPath\$keyvaultName.json"
	$keyvault = $json | ConvertFrom-Json
	Write-Output "Your properties for the keyvault is stored in $keyvaultName.json"

	Write-Output "Setting access policy for the RBAC SP to the KeyVault"
	$json = az keyvault set-policy --name $keyvaultName --spn $rbac.appId --secret-permissions delete get list set --certificate-permissions create delete get import list update --key-permissions create delete get list update
	$json | Out-File -FilePath "$logPath\$appName-policy.json"
	$policy = $json | ConvertFrom-Json
	Write-Output "Your policy for the RBAC is stored in $appName-policy.json"

	Write-Output "Creating a secret in the keyvault"
	$json = az keyvault secret set --vault-name $keyvaultName --name $secretName --value $secretValue
	$json | Out-File -FilePath "$logPath\$secretName.json"
	$secret = $json | ConvertFrom-Json
	Write-Output "Your properties for the secret is stored in $secretName.json"
	Write-Output ""
	Write-Output "Use following setting when using this keyvault from .NET Core:"
	Write-Output "Name: $keyvaultName"
	Write-Output "ClientId: $clientId"
	Write-Output "ClientSecret: $clientSecret"
	
	$json = "{""KeyVault"":{""Name"": ""$keyvaultName"",""ClientId"": ""$clientId"", ""ClientSecret"": ""$clientSecret""}}"
	$json | Out-File -FilePath ".\keyvault.json"
} 
else 
{
	Write-Output "Key vault $keyvaultName already created"
}