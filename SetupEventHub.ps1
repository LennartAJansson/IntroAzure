#Always start Azure scripts with this, to change your subscriptionid edit AzureLogin.ps1:
$scriptPath = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. ("$scriptPath\AzureLogin.ps1")

#KeyVault: https://docs.microsoft.com/sv-se/cli/azure/keyvault?view=azure-cli-latest

"Active subscription $subscription"
if($subscription -eq "")
{
	exit 1
}

##############################
# Setup Azure infrastructure #
##############################
$resourceGroup = "$($Prefix)EvtRG"
$location = "northeurope"
$keyvaultName = "$($Prefix)KV"

$storageAccount = "$($Prefix)EvtStorage"
$storageContainer = "$($Prefix)EvtContainer"
$eventHubNamespaceName = "$($Prefix)EvtNS"
$eventHubName = "$($Prefix)EvtHub"

$eventHubNames = az eventhubs namespace list --query '[].name' --output tsv
if (!($eventHubNames -Contains $eventHubName)) 
{
	Write-Output "Creating a resource group"
	$json = az group create --name $resourceGroup --location $location
	$json | Out-File -FilePath "$logPath\$resourceGroup.json"
	$group = $json | ConvertFrom-Json
	Write-Output "Your properties for the resourcegroup is stored in $logPath\$resourceGroup.json"

	$json = az storage account create --name $storageAccount.ToLower() --resource-group $resourceGroup --location $location --sku Standard_LRS --kind StorageV2
	$json | Out-File -FilePath "$logPath\$storageAccount.json"
	$storage = $json | ConvertFrom-Json
	Write-Output "Your properties for the storage account is stored in $logPath\$storageAccount.json"
	$connectionStringBlob = az storage account show-connection-string --name $storageAccount.ToLower() --output tsv

	$json = az storage container create --name $storageContainer.ToLower() --connection-string "$connectionStringBlob"
	$json | Out-File -FilePath "$logPath\$storageContainer.json"
	$container = $json | ConvertFrom-Json
	Write-Output "Your properties for the resourcegroup is stored in $logPath\$storageContainer.json"

	Write-Output "Creating a eventhub namespace"
	$json = az eventhubs namespace create --name $eventHubNamespaceName --resource-group $resourceGroup --location $location
	$json | Out-File -FilePath "$logPath\$eventHubNamespaceName.json"
	$eventhubnamespace = $json | ConvertFrom-Json
	Write-Output "Your properties for the eventhub namespace is stored in $logPath\$eventHubNamespaceName.json"

	$json = az eventhubs eventhub create --name $eventHubName --resource-group $resourceGroup --namespace-name $eventHubNamespaceName
	$json | Out-File -FilePath "$logPath\$eventHubName.json"
	$eventhub = $json | ConvertFrom-Json
	Write-Output "Your properties for the eventhub is stored in $logPath\$eventHubName.json"

	Write-Output "Getting the connectionstring for the eventhub namespace"
	$connectionStringEvt = $(az eventhubs namespace authorization-rule keys list --resource-group $resourceGroup --namespace-name $eventHubNamespaceName --name RootManageSharedAccessKey --query primaryConnectionString --output tsv)

	Write-Output "Creating a secret in the keyvault for the connectionstring"

	$json = az keyvault secret set --name "EventHub--EventHubConnectionString" --vault-name $keyvaultName --value $connectionStringEvt
	$json | Out-File -FilePath "$logPath\EventHub--EventHubConnectionString.json"
	$secret = $json | ConvertFrom-Json
	Write-Output "Your connectionstring for the eventhub namespace is stored in the keyvault as EventHub--EventHubConnectionString (EventHub:EventHubConnectionString)"
	
	$json = az keyvault secret set --name "EventHub--EventHubName" --vault-name $keyvaultName --value $eventHubName
	$json | Out-File -FilePath "$logPath\EventHub--EventHubName.json"

	$json = az keyvault secret set --name "EventHub--BlobConnectionString" --vault-name $keyvaultName --value $connectionStringBlob
	$json | Out-File -FilePath "$logPath\EventHub--BlobConnectionString.json"

	$json = az keyvault secret set --name "EventHub--BlobContainerName" --vault-name $keyvaultName --value $storageContainer
	$json | Out-File -FilePath "$logPath\EventHub--BlobContainerName.json"

	Write-Output "Your connectionstring to the eventhub namespace is $connectionStringEvt"
	Write-Output "Your eventhub name is $eventHubName"
	Write-Output "Your connectionstring to the blob storage is $connectionStringBlob"
	Write-Output "Your blob container name is $storageContainer"
	Write-Output "It is stored in the keyvault and in .\eventhub.json"
	Write-Output "Remember that if you are using EventHub SDK, you will have to create EventHubs on your own."
	Write-Output "This script has created a eventhub named $eventHubName, this is the name used in the SDK examples"

	"{""EventHub"":{""EventHubConnectionString"":""$connectionStringEvt"",""EventHubName"":""$eventHubName"",""BlobConnectionString"":""$connectionStringBlob"",""BlobContainerName"":""$storageContainer""}}" | Out-File -FilePath ".\eventhub.json"
	Write-Output "Your connectionstring for the eventhub namespace is stored in .\eventhub.json"
}
else
{
	Write-Output "EventHub namespace $eventHubName already created"
}