using System;
using System.Text;
using Google.Cloud.SecretManager.V1;
using Google.Api.Gax.ResourceNames;
using Google.Protobuf;

namespace SecretConfigurationProvider
{
    public class SecretManager
    {
        public static String AccessSecret(string projectId, string secretId)
        {
            string secretVersionId = "latest";

            // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, secretVersionId);

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();
            return payload;
        }           
        public static void CreateSecret(string projectId, string secretId, string secretValue)
        {
            // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the parent project name.
            ProjectName projectName = new ProjectName(projectId);

            // Build the secret to create.
            Secret secret = new Secret
            {
                Replication = new Replication
                {
                    Automatic = new Replication.Types.Automatic(),
                },
            };

            Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

            // Build a payload.
            SecretPayload payload = new SecretPayload
            {
                Data = ByteString.CopyFrom(secretValue, Encoding.UTF8),
            };

            // Add a secret version.
            SecretVersion createdVersion = client.AddSecretVersion(createdSecret.SecretName, payload);

            // Access the secret version.
            AccessSecretVersionResponse result = client.AccessSecretVersion(createdVersion.SecretVersionName);

            // Print the results
            //
            // WARNING: Do not print secrets in production environments. This
            // snippet is for demonstration purposes only.
            string data = result.Payload.Data.ToStringUtf8();
            Console.WriteLine($"Plaintext: {data}");
        }     
    }
}
