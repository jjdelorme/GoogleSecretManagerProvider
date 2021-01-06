# .NET Configuration Provider for Google Secret Manager

ASP.NET offers a canonical way to retrieve configuration information without changing an applications code using a Configuration Provider interface.  This is working sample implementation of a [.NET Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) for [Google Secret Manager](https://cloud.google.com/secret-manager) to retrieve secrets natively in .NET.

## Prerequisites

1. Setup your .NET development environment for Google Cloud from these [instructions](https://cloud.google.com/dotnet/docs/setup).

1. Enable the Google Secret Manager API in your Google Cloud Console: https://console.developers.google.com/apis/api/secretmanager.googleapis.com/overview?project={your_project_number} or simply browse http://console.cloud.google.com for Google Secret Manager.

## Usage

First, clone this repository and add a reference to it in your project.  For example:

```bash
cd ~/src
git clone https://github.com/jjdelorme/GoogleSecretManagerProvider.git

cd ~/src/your-aspnet-project
dotnet add reference ../SecretConfigurationProvider/SecretConfigurationProvider.csproj
```

You can see additional information [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0) from Microsoft on the .NET Generic Host in ASP.NET Core (and non-http hosts as well).  For the purposes of this sample, we will use an ASP.NET Core 5.0 web application.  Modify your ```Program.cs``` to include the namespace and add the ```ConfigureAppConfiguration(...)``` method to  ```CreateHostBuilder``` with a call to the extension method ```AddGoogleSecretManagerConfiguration``` specifying your Google Project Id as the parameter. 

```csharp
using SecretConfigurationProvider;
        ...
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddGoogleSecretManagerConfiguration(projectId: "my-google-projectid");
                })            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```
To access the secret value in your code, use ASP.NET's built-in dependency injection within your constructor to obtain the ```IConfiguration``` object.  This object can retrieve configuration from all configured providers including the defaults such as commandline args and environment variables.

```csharp
        public GreeterService(ILogger<GreeterService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        ...

        public string SayHello()
        {
            return $"Hello, I have your secret: {_config["my-secret"]}";
        }
```


## Resources:

Additional resources can be found here on using the Google Secret Manager directly from code.  However, this sample abstracts this such that a client can simply use the canonical ```string value = Configuration[key]``` to retrieve.

- Learn how to install Secret Manager client library for .NET: 
https://cloud.google.com/secret-manager/docs/reference/libraries#client-libraries-install-csharp

- Learn how to create & access secrets using C# SDK:
https://cloud.google.com/secret-manager/docs/creating-and-accessing-secrets