using SecretConfigurationProvider;

SecretManager.Quickstart();
System.Console.WriteLine("Wrote secret.");

GoogleSecretManagerSource source = new GoogleSecretManagerSource(
    new GoogleSecretManagerOptions() { ProjectId = "jasondel-grpc-test" }
);

GoogleSecretManagerProvider provider = new GoogleSecretManagerProvider(source);

string value = null;
provider.TryGet("my-secret", out value);
System.Console.WriteLine(value);