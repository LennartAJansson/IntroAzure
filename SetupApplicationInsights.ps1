#Always start Azure scripts with this, to change your subscriptionid edit AzureLogin.ps1:
$ScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. ("$ScriptDirectory\AzureLogin.ps1")

#Application Insights: https://docs.microsoft.com/sv-se/cli/azure/monitor?view=azure-cli-latest

"Active subscription $subscription"

##############################
# Setup Azure infrastructure #
##############################
$resourceGroup = "LennartAIRG"
$location = "northeurope"
$keyvaultName = "LennartKV"