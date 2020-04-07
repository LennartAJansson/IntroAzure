#############################
# Setup local prerequisites #
#
#############################
#https://aka.ms/installazurecliwindows

if($env:ChocolateyInstall)
{
}


if($env:Path -notmatch "chocolatey")
{
	iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
	setx PATH "%PATH%;%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
}
else
{
	choco upgrade chocolatey
}

if($env:Path -notmatch "Docker\\Resources")
{
	choco install docker-for-windows -y
}
else
{
	choco upgrade docker-for-windows -y
}

if($env:Path -notmatch "\\.azure-kubectl")
{
	choco install kubernetes-cli -y
}
else
{
	choco upgrade kubernetes-cli -y
}

if($env:Path -notmatch "Helm")
{
	choco install kubernetes-helm -y
}
else
{
	choco upgrade kubernetes-helm -y
}

if($env:Path -notmatch "7-zip")
{
	choco install 7zip -y
}
else
{
	choco upgrade 7zip -y
}

if($env:Path -notmatch "Azure\\CLI2")
{
	choco install azure-cli -y
}
else
{
	choco upgrade azure-cli -y
}

if($env:Path -notmatch "Azure\\CLI2")
{
	choco install azure-cli -y
}
else
{
	choco upgrade azure-cli -y
}

az --version
kubectl version
helm version
