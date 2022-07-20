namespace Blogifier.Plugin.AcmeCertificate.Config
{
    public class AcmeConfig
    {
        public bool EnableHttpsRedirection { get; set; }

        /// <summary>
        /// Chooses an storage engine for the certificate between KEYVAULT - Azure key vault BLOB -
        /// Azure blob storage DIRECTORY - Local file system DEFAULT - Local certificate store
        /// </summary>
        public string CertificateStorage { get; set; }
    }
}
