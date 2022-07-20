namespace Blogifier.Plugin.AcmeCertificate.Config
{
    public class AzureBlobStorageConfig
    {
        public string CertificateBlob { get; set; }
        public string CertificateKeyBlob { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string DataProtectionBlob { get; set; }
        public bool StoreDataProtectionInBlob { get; set; }
    }
}
