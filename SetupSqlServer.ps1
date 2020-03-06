﻿#Always start Azure scripts with this, to change your subscriptionid edit AzureLogin.ps1:
$ScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. ("$ScriptDirectory\AzureLogin.ps1")

#SQL Server: https://docs.microsoft.com/sv-se/cli/azure/sql?view=azure-cli-latest

"Active subscription $subscription"

##############################
# Setup Azure infrastructure #
##############################
$resourceGroup = "LennartSQLRG"
$location = "northeurope"
$keyvaultName = "LennartKV"