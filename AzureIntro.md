# Azure Dev

I denna solution kommer jag att gå igenom hur man utvecklar för Azure med hjälp av NET Core.  

Varje exempel kommer att ha ett motsvarande powershell script för att sätta upp de tjänster som används. Detta är en metod som kallas "Infrastructure as code".  

Till exemplen behövs också i vissa fall tillgång till Azure Devops med dess bygg och release pipeline, jag kommer att gå igenom hur man sätter upp allt detta.

För att kunna köra alla exempel behövs ett antal komponenter tillgängliga:

* MSDN abbonemang med Azureportalen aktiverad

* Azure Devops med ett repo för denna solution

* Visual Studio 2019

* NET Core 3.x

* Azure CLI för Powershell

* Docker for Windows

* Kubernetes CLI

* Kubernetes Helm

* Servicebus Explorer

I solution finns det ett powershell script som heter ```InstallLocal.ps1``` som installerar de fem sista tilläggen i denna listan med hjälp av Chocolatey. Scriptet installerar även Chocolatey om det inte finns redan på datorn.