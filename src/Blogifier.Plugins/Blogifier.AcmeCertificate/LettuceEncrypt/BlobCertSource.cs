using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Blogifier.Plugin.AcmeCertificate.Azure;
using Blogifier.Plugin.AcmeCertificate.Config;

using LettuceEncrypt;
using LettuceEncrypt.Accounts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.AcmeCertificate.LettuceEncrypt
{
    internal class BlobX509CertStore : ICertificateSource, ICertificateRepository, IAccountStore
    {
        private readonly AzureBlobStorageConfig _azureOptions;
        private readonly BlobClient _client;
        private readonly ILogger<BlobX509CertStore> _logger;
        private readonly LettuceEncryptOptions _options;
        public bool AllowInvalidCerts { get; set; }

        public BlobX509CertStore(
            BlobClient client,
            IOptions<LettuceEncryptOptions> options,
            IOptions<AzureBlobStorageConfig> azureOptions,
            ILogger<BlobX509CertStore> logger)
        {
            _client = client;
            _options = options.Value;
            _azureOptions = azureOptions.Value;
            _logger = logger;
        }

        public async Task<AccountModel> GetAccountAsync(CancellationToken cancellationToken)
        {
            var bytes = await _client.ReadBinaryAsync(_azureOptions.CertificateKeyBlob, cancellationToken);
            if (bytes == null) return null;

            var account = await JsonSerializer.DeserializeAsync<AccountModel>(bytes.ToStream());

            var accountInfo = Encoding.UTF8.GetString(bytes);
            _logger.LogInformation("Retrieved account information {accountInfo}", accountInfo);

            return account;
        }

        public async Task<IEnumerable<X509Certificate2>> GetCertificatesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var content = await _client.ReadBinaryAsync(_azureOptions.CertificateBlob, cancellationToken);
                if (content == null)
                {
                    _logger.LogInformation("Certificate not found in external storage");
                    return new List<X509Certificate2>();
                }

                _logger.LogInformation("Certificate found in external storage");

                var certificate = new X509Certificate2(content.ToArray());
                return new List<X509Certificate2>() { certificate };
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Failed to get certificate from store");
                throw;
            }
        }

        public async Task SaveAccountAsync(AccountModel account, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Saving account information to {container} {object}", _azureOptions.ContainerName, _azureOptions.CertificateKeyBlob);

            var bytes = JsonSerializer.SerializeToUtf8Bytes(account);
            await _client.SaveAsync(_azureOptions.CertificateKeyBlob, bytes, cancellationToken);

            _logger.LogInformation("Saved account information to {container} {object}", _azureOptions.ContainerName, _azureOptions.CertificateKeyBlob);
        }

        public async Task SaveAsync(X509Certificate2 certificate, CancellationToken cancellationToken)
        {
            try
            {
                var domainName = certificate.GetNameInfo(X509NameType.DnsName, false);
                _logger.LogInformation("Saving certificate for {domainName} in Blob Storage.", domainName);

                var bytes = certificate.Export(X509ContentType.Pfx);
                await _client.SaveAsync(_azureOptions.CertificateBlob, bytes, cancellationToken);
                _logger.LogDebug("Certificate saved to external storage");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(0, ex, "Failed to save certificate to store");
                throw;
            }

            return;
        }
    }
}
