#Always start Azure scripts with this, to change your subscriptionid edit AzureLogin.ps1:
$ScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. ("$ScriptDirectory\AzureLogin.ps1")

"Active subscription $subscription"
if($subscription -eq "")
{
    exit 1
}

##############################
# Setup Azure infrastructure #
##############################
$resourceGroup = "LennartAKS"
$location = "northeurope"

$acrName = "LennartACR"
$aksCluster = "LennartCluster"
$servicePrincipal = "LennartClusterSP"


$clusterNames = az aks list --query '[].[name]' --output tsv
if (!($clusterNames -Contains $aksCluster)) 
{
	Write-Output "Creating a resource group"
	$json = az group create --name $resourceGroup --location $location
	$json | Out-File -FilePath "$logPath\$resourceGroup.json"
	$group = $json | ConvertFrom-Json
	Write-Output "Your properties for the resourcegroup $resourceGroup is stored in $resourceGroup.json"

	Write-Output "Creating a container registry"
	$json = az acr create --resource-group $resourceGroup --name $acrName --sku Basic
	$json | Out-File -FilePath "$logPath\$acrName.json"
	$repository = $json | ConvertFrom-Json
	Write-Output "Your properties for container registry $acrName is stored in $acrName.json"

	Write-Output "Logging in to the repository"
	az acr login --name $repository.name.ToLower()

	Write-Output "Registering a service principal for rbac. Warnings here are normal to get!"
	$json = az ad sp create-for-rbac -n "http://$servicePrincipal"
	$json | Out-File -FilePath "$logPath\$servicePrincipal.json"
	$rbac = $json | ConvertFrom-Json
	Write-Output "Your properties for the RBAC $servicePrincipal is stored in $servicePrincipal.json. The script will pause now for 90 seconds to activate the RBAC."
    Start-Sleep -s 90

	Write-Output "Getting the latest version of AKS"
	$latestK8sVersion = $(az aks get-versions -l $location --query 'orchestrators[-1].orchestratorVersion' -o tsv)
	$latestK8sVersion

	Write-Output "Creating the cluster"
	Write-Output "We are about to create the AKS Cluster. Once created (the creation could take ~10 min) we will continue..."
	$json = az aks create --location $location --name $aksCluster --resource-group $resourceGroup --generate-ssh-keys --kubernetes-version $latestK8sVersion --service-principal $rbac.appId --client-secret $rbac.password --node-count 1
	$json | Out-File -FilePath "$logPath\$aksCluster.json"
	$cluster = $json | ConvertFrom-Json
	Write-Output "Your properties for the cluster $aksCluster is stored in $aksCluster.json"

	Write-Output "Getting the credentials for AKS and the cluster"
	az aks get-credentials --name $aksCluster --resource-group $resourceGroup --overwrite-existing

	Write-Output "Setting acrpull access policy for the RBAC SP"
	$json = az role assignment create --assignee $rbac.appId --role acrpull --scope $repository.id
	$json | Out-File -FilePath "$logPath\$clientId-Pull.json"
	$role = $json | ConvertFrom-Json
	Write-Output "Your properties for the ServicePrincipalProfile $($rbac.appId)-Pull is stored in $($rbac.appId)-Pull.json"
}
else 
{
	Write-Output "Cluster $aksCluster already created"
}
