using System;
using System.IO;
using System.Net;

using Blogifier.Core.Plugins;
using Blogifier.Plugin.AcmeCertificate.Azure;
using Blogifier.Plugin.AcmeCertificate.Config;
using Blogifier.Plugin.AcmeCertificate.LettuceEncrypt;

using LettuceEncrypt;
using LettuceEncrypt.Accounts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.AcmeCertificate
{
    public class PluginStartup : IPluginBootstrapper
    {
        private AzureBlobStorageConfig _blobConfig;
        private IConfigurationSection _blobConfigSection;
        private DirectoryStorageConfig _directoryConfig;
        private IConfigurationSection _directoryConfigSection;
        private IConfigurationSection _lettuceConfigSection;
        private AcmeConfig _sslConfig;
        private IConfigurationSection _sslConfigSection;

        public PluginStartup()
        {
            (_sslConfig, _blobConfig, _directoryConfig) = LoadPluginConfiguration();
        }

        public void AddServices(IServiceCollection services)
        {
            Console.WriteLine("Lettuce AddServices");

            AddHttpsRedirection(services);

            var storage = _sslConfig.CertificateStorage.ToUpper();
            switch (storage)
            {
                case "KEYVAULT":
                    EnableLettuceWithKeyVault(services);
                    break;

                case "DIRECTORY":
                    EnableLettuceWithFileSystemPersistence(services);
                    break;

                case "BLOB":
                    EnableLettuceWithBlogStoragePersistence(services);
                    break;

                default:
                    EnableLettuceWithDefaultOptions(services);
                    break;
            }
        }

        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseHttpsRedirection();
        }

        public void ConfigureHost(IWebHostBuilder webHostBuilder)
        {
            Console.WriteLine("Lettuce Configurehost");
        }

        public void ConfigureKestrel(KestrelServerOptions kestrel)
        {
            Console.WriteLine("Lettuce ConfigureKestrel");

            var appServices = kestrel.ApplicationServices;
            kestrel.ConfigureHttpsDefaults(h =>
            {
                //h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                h.UseLettuceEncrypt(appServices);
            });
        }

        private static void EnableLettuceWithDefaultOptions(IServiceCollection services)
        {
            services.AddLettuceEncrypt();
        }

        private static void EnableLettuceWithKeyVault(IServiceCollection services)
        {
            services.AddLettuceEncrypt()
                .PersistCertificatesToAzureKeyVault();
        }

        private void EnableLettuceWithBlogStoragePersistence(IServiceCollection services)
        {
            Console.WriteLine("Configure BLOB storage");

            AddBlobStorageConfigurationClasses(services);
            AddBlobStorage(services);

            services.AddLettuceEncrypt();
            services.AddSingleton<ICertificateRepository, BlobX509CertStore>();
            services.AddSingleton<ICertificateSource, BlobX509CertStore>();
            services.AddSingleton<IAccountStore, BlobX509CertStore>();
            services.AddSingleton<BlobClient>();
        }

        private void EnableLettuceWithFileSystemPersistence(IServiceCollection services)
        {
            services.AddLettuceEncrypt()
                .PersistDataToDirectory(new DirectoryInfo(_directoryConfig.Directory), _directoryConfig.Password);
        }

        #region Private methods

        private void AddHttpsRedirection(IServiceCollection services)
        {
            if (_sslConfig.EnableHttpsRedirection)
                Console.WriteLine("Enabling Https redirection");

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
                options.HttpsPort = 443;
            });
        }

        private void AddBlobStorage(IServiceCollection services)
        {
            if (_blobConfig.StoreDataProtectionInBlob)
            {
                // The data protection library doesn't create the container. We do it now if needed.
                var azureClient = new BlobClient(_blobConfig);
                azureClient.GetContainerClient(_blobConfig.ContainerName).Wait();

                Console.WriteLine("Redirecting data protection keys to blob storage");
                services
                  .AddDataProtection()
                  .PersistKeysToAzureBlobStorage(_blobConfig.ConnectionString, _blobConfig.ContainerName, _blobConfig.DataProtectionBlob);
            }
        }

        private (AcmeConfig sslConfig, AzureBlobStorageConfig blobConfig, DirectoryStorageConfig directoryConfig) LoadPluginConfiguration()
        {
            // Read plugin config to a temporary object

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var location = typeof(PluginStartup).Assembly.GetFiles()[0].Name;
            var path = Path.GetDirectoryName(location);

            Console.WriteLine("Loading config files in " + path);
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"settings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var lettuceConfig = new LettuceEncryptOptions();
            _lettuceConfigSection = config.GetSection("LettuceEncrypt");
            _lettuceConfigSection.Bind(lettuceConfig);

            var sslConfig = new AcmeConfig();
            _sslConfigSection = config.GetSection("AcmeSettings");
            _sslConfigSection.Bind(sslConfig);

            var blobConfig = new AzureBlobStorageConfig();
            _blobConfigSection = config.GetSection("LettuceEncrypt:AzureBlobStorage");
            _blobConfigSection.Bind(blobConfig);

            var directoryConfig = new DirectoryStorageConfig();
            _directoryConfigSection = config.GetSection("LettuceEncrypt:DirectoryStorage");
            _directoryConfigSection.Bind(directoryConfig);

            return (sslConfig, blobConfig, directoryConfig);
        }

        private void AddBlobStorageConfigurationClasses(IServiceCollection services)
        {
            services.AddOptions<LettuceEncryptOptions>();
            services.AddSingleton<IConfigureOptions<LettuceEncryptOptions>>(s =>
                new ConfigureOptions<LettuceEncryptOptions>(o => _lettuceConfigSection.Bind(o))
            );

            services.AddOptions<AzureBlobStorageConfig>();
            services.AddSingleton<IConfigureOptions<AzureBlobStorageConfig>>(s =>
                new ConfigureOptions<AzureBlobStorageConfig>(o => _blobConfigSection.Bind(o))
            );

            services.AddOptions<DirectoryStorageConfig>();
            services.AddSingleton<IConfigureOptions<DirectoryStorageConfig>>(s =>
                new ConfigureOptions<DirectoryStorageConfig>(o => _directoryConfigSection.Bind(o))
            );
        }

        #endregion Private methods
    }
}
