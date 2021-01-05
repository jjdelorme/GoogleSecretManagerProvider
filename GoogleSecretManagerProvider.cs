using System;
using Microsoft.Extensions.Configuration;

namespace SecretConfigurationProvider
{
    public class GoogleSecretManagerProvider : ConfigurationProvider
    {
        public GoogleSecretManagerSource Source { get; }

        public GoogleSecretManagerProvider(GoogleSecretManagerSource source)
        {
            Source = source;
        }

        public override bool TryGet(string key, out string value)
        {
            try
            {
                if (Data.ContainsKey(key))
                { 
                    value = Data[key];
                }
                else 
                {
                    value = SecretManager.AccessSecret(Source.ProjectId, key);
                    Set(key, value);
                }
                
                return (value != null);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(
                    $"Unable to retrieve secret {key}. \nError: {e.Message}");

                value = null;
                return false;
            }
        }
    }

    public class GoogleSecretManagerOptions
    {
        public string ProjectId { get; set; }
    }

    public class GoogleSecretManagerSource : IConfigurationSource
    {
        public string ProjectId;

        public GoogleSecretManagerSource(GoogleSecretManagerOptions options)
        {
            ProjectId = options.ProjectId;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GoogleSecretManagerProvider(this);
        }
    }

    public static class GoogleSecretManagerExtensions
    {
        public static IConfigurationBuilder AddGoogleSecretManagerConfiguration(this IConfigurationBuilder configuration, 
            Action<GoogleSecretManagerOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            
            var configOptions = new GoogleSecretManagerOptions();
            options(configOptions);
            configuration.Add(new GoogleSecretManagerSource(configOptions));
            
            return configuration;
        }
    }
}