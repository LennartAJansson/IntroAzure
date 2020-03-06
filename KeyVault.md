## To use Azure KeyVault

In normal case when you have configuration values that you don't want to expose in any repository you could use User Secrets, it could be sensitive information like connection strings or account information. If you look in the ```Intro``` solution you will find more information about how to setup and use User Secrets.  

User Secrets is stored outside of your repo under your current user profile in ```%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json```.  

### Command line way of handling secrets
To initialize your project to use secrets run the command (in your project folder):  
```
dotnet user-secrets init
```
This will generate a UserSecretsId in your projectfile.  

To add a secret run the command:  

```
dotnet user-secrets set "AppSettings:ConnectionString" "some-value"
```  
This will add a json-group named AppSettings to your ```secrets.json``` with a value named ConnectionString containing the value "some-value"

### Visual Studios way of handling secrets
You can also edit your secrets from Visual Studio by right-clicking your project file in your solution and choose "Manage User Secrets". This will open up your ```secrets.json``` inside Visual Studio.

### References
Once you have your secrets it is possible to include them into your application configuration just by adding a reference to the package "Microsoft.Extensions.Configuration.UserSecrets", this will read the ```secret.json``` after ```appsettings.json``` and ```appsettings.Development.json```.
  
### Azures way of handling secrets
Azure has its own implementation of handling Secrets in a centralized repository that is quite powerful, not only is it possible to store Secrets but also Keys, Certificates etc. This service is called Azure KeyVault. The only thing we need to access the KeyVault is the url, a login and a password(token).  

### Excercise
This excercise is about how to create a KeyVault and how to use it to retrieve KeyVault Secrets from a NET Core application.  

Open the ```IntroAzure``` solution in Visual Studio (most likely you already have it open)  
Edit ```AzureLogin.ps1```, change the subscription value to your own subscription id  
Start a Powershell prompt  
Switch to the folder where the ```IntroAzure``` solution exist  
Run the script ```SetupKeyvault.ps1```  
Remember/note the values returned from the script (the text at the end).  

In Visual Studio, rightclick on ```UsingKeyVault``` project and select Manage User Secrets, it should contain:
```
{
  "KeyVault": {
    "Name": "The name from $resourceName in the powershell script",
    "ClientId": "The appId guid from $appName.json",
    "ClientSecret": "The password guid from $appName.json"
  },
  "AppSecret": {
    "MyAppSecret": "This is a secret from local UserSecrets"
  }
}
```  
Change KeyVault:Name, KeyVault:ClientId and KeyVault:ClientSecret to the values you used in the script.  

On line 29 in Program.cs there is an if statement that looks like this:  
```
if (hostContext.HostingEnvironment.IsProduction())
```  
Toggle this line by commenting/uncommenting and try to run the program with and without the if statement.  
You should see different outputs:

```
15:31:44|9132|07|INF: (UsingKeyVault.Worker) DoWork has been called. AppSecret is: This is a secret from local UserSecrets
```  
Or
```
15:32:56|42584|05|INF: (UsingKeyVault.Worker) DoWork has been called. AppSecret is: This is a secret from keyvault
```
