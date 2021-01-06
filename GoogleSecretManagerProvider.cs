using System;
using Microsoft.Extensions.Configuration;

namespace SecretConfigurationProvider
{
    public class GoogleSecretManagerProvider : ConfigurationProvider
    {
        public GoogleSecretManagerSource Source { get; }
        
        private SecretManager _secretManager;

        public GoogleSecretManagerProvider(GoogleSecretManagerSource source)
        {
            Source = source;
            _secretManager = new SecretManager();
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
                    value = _secretManager.AccessSecret(Source.ProjectId, key);
                    Set(key, value);
                }
                
                return (value != null);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Unable to retrieve secret {key}.", e);
            }
        }
    }

    public class GoogleSecretManagerSource : IConfigurationSource
    {
        public string ProjectId;

        public GoogleSecretManagerSource(string projectId)
        {
            ProjectId = projectId;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GoogleSecretManagerProvider(this);
        }
    }

    public static class GoogleSecretManagerExtensions
    {
        const string ProjectEnvVar = "GOOGLE_PROJECT_ID";

        public static IConfigurationBuilder AddGoogleSecretManagerConfiguration(
            this IConfigurationBuilder configuration, 
            string projectId)
        {
            projectId ??= Environment.GetEnvironmentVariable(ProjectEnvVar);
        
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId), 
                    $"Must provide a projectId or set {ProjectEnvVar} env variable.");
            
            configuration.Add(new GoogleSecretManagerSource(projectId));
            
            return configuration;
        }
    }
}