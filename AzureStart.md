## Setting up Azure AKS  

I have used following walkthrough as a starting point for setting up Azure AKS:  
https://docs.microsoft.com/sv-se/azure/aks/kubernetes-walkthrough

I will be using the Azure CLI Client to perform the different steps needed to create an AKS Cluster. If you are logged in to your Azure Portal you could use the CLI through the portal (Azure Cloud Shell) by clicking on it in the toolbar. It looks like this:  
  
![C L I](CLI.png)  
  
Try clicking on it! If you don't have an Azure Cloud Drive configured already, it will ask you to do so, simply continue and it will create everything needed for you.  
  
For doing a lot of steps I recommend you to download Azure CLI Client locally instead, it will add everything so you can work locally from a Powershell prompt, you don't even have to be in the portal.  

To install the Azure CLI Client:  
https://docs.microsoft.com/sv-se/cli/azure/install-azure-cli-windows?view=azure-cli-latest  
Or download it directly from:  
https://aka.ms/installazurecliwindows  

Once you have installed the Azure CLI Client you can continue learning about its possibilities here:  
https://docs.microsoft.com/sv-se/cli/azure/get-started-with-azure-cli?view=azure-cli-latest  


The first thing we will do now is to start powershell and perform a login:  

```az login```

This will open up a browser so you could do a login to your Azure account.  

If you look in the powershell window after you have logged in you will see that it got a json in return containing your subscription details. This means we are good to go.  

Next thing we will need is a resource group, if you don't have that already you can add it by executing:  

```az group create --name TestAKS --location northeurope```  

In return you will get a json with details about the group that is being created for you.  

Ok, so let's continue to create an AKS Cluster:

```az aks create --resource-group TestAKS --name myAKSCluster --node-count 1 --enable-addons monitoring --generate-ssh-keys```  

This is a lengthy process, overall it will take a couple of minutes, don't be worried if you think it got stucked...  
First of all it will reply with some information about your ssh key:  

```
SSH key files 'C:\Users\#USERNAME#\.ssh\id_rsa' and 'C:\Users\#USERNAME#\.ssh\id_rsa.pub' have been generated under ~/.ssh to allow SSH access to the VM. If using machines without permanent storage like Azure Cloud Shell without an attached file share, back up your keys to a safe location
Finished service principal creation[##################################]  100.0000%
```

Then it will say:  
``` - Running ..```

Then after a couple of minutes it will return a large json about what you are creating...  

What we need to do now is to install the KUBECTL, that's the Kubernetes command-line tool. In the Azure Cloud Shell it is already installed but to install it locally into your own machine, run following command:  

```az aks install-cli```  

Now it will show you that you need to add the installation path to your environment PATH so it could be found. Choose one of the options that is given for you...  

And then to configure it against the cluster and get the correct credentials you have to run:  

```az aks get-credentials --resource-group TestAKS --name myAKSCluster```  

Once that is finished you could try to connect to it by using the command:  

```kubectl get nodes```  

It will show you the single-noded cluster that you just created...  

If you get the following error:  

```error: You must be logged in to the server (the server has asked for the client to provide credentials (get nodes))```  

This means that you forgot to run the previous command "az aks get-credentials".

Now we have set everything up to use Azure AKS for our own software. Let's create a container registry where we can store our images needed for the containers.  
```
az acr create --resource-group TestResources --name LeJaContainerRegistry --sku Basic  
az acr login --name lejacontainerregistry  
```
Then we tag the local docker image to identify that it belongs to the container registry and push it up to Azure  
```
docker tag aspnetcore.node:2.2.203 lejacontainerregistry.azurecr.io/aspnetcore.node:v1  
docker push lejacontainerregistry.azurecr.io/aspnetcore.node:v1  
```
We can verify that it succeeded by running  
```
az acr repository show-tags --name LeJaContainerRegistry --repository aspnetcore.node --output table  
```
And afterwards we can delete the local image  
```
docker rmi lejacontainerregistry.azurecr.io/aspnetcore.node:v1  
#az group delete --name myResourceGroup
```  

Return to the previous document to see the next steps...