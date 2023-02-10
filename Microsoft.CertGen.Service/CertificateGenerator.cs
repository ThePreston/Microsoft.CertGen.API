using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.CertGen.Service
{
    public class CertificateGenerator : ICertificateGenerator
    {
        private readonly ILogger<CertificateGenerator> _logger;

        private readonly IConfiguration _config;

        private CertificateClient _client;

        public CertificateGenerator(ILogger<CertificateGenerator> log, IConfiguration config, CertificateClient client )
        {
            _logger = log;
            _config = config;
            _client = client;
        }

        #region Public Methods

        public async Task<X509Certificate2> GetKVCertificate(string certName)
        {    
            return await _client.DownloadCertificateAsync(certName);
        }

        public async Task<X509Certificate2> CreateRootCertificate(string rootCertName, bool saveToKeyVault)
        {
            var rootCertificate = GenerateRootCertificate(rootCertName);

            if(saveToKeyVault)
                await SaveCert(rootCertificate);

            return rootCertificate;
        }

        public async Task<X509Certificate2> GenerateSignedCertificate(string rootCertName, string leafCertName)
        {
            var rootCert = await GetKVCertificate(rootCertName);

            var signedCert = GenerateSignedCertificate(rootCert, leafCertName);

            return signedCert;
        }

        public async Task<X509Certificate2> GenerateSignedCertificateWithPrivateKey(string rootCertName, string leafCertName)
        {            
            return GenerateSignedCertificateWithPrivateKey(await GetKVCertificate(rootCertName), leafCertName);
        }

        #endregion

        #region Private Methods

        private async Task<KeyVaultCertificateWithPolicy> SaveCert(X509Certificate2 cert)
        {            
            return await _client.ImportCertificateAsync(new ImportCertificateOptions(cert.FriendlyName, cert.Export(X509ContentType.Pkcs12)) );
        }

        private  X509Certificate2 GenerateRootCertificate(string rootCertName)
        {
            _logger.LogInformation("Entered GenerateRootCertificate");

            try
            {

                // Create a new self-signed certificate using RSA encryption
                using (RSA rootKey = RSA.Create(Convert.ToInt32(_config["CertificateRSA"])))
                {
                    var rootCertificateRequest = new CertificateRequest(_config["CertificateSubject"], rootKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    rootCertificateRequest.CertificateExtensions.Add(
                        new X509BasicConstraintsExtension(true, false, 0, true));

                    var rootCertificate = rootCertificateRequest.CreateSelfSigned(new DateTimeOffset(DateTime.Now), new DateTimeOffset(DateTime.Now.AddYears(Convert.ToInt32(_config["RootCertValidYears"]))));

                    rootCertificate.FriendlyName = rootCertName;

                    return rootCertificate;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error generating Root Certificate");
                throw ex;
            }

        }

        private X509Certificate2 GenerateSignedCertificate(X509Certificate2 rootCertificate, string leafCertificateName)
        {
            _logger.LogInformation("Entered SignCertificate");

            try
            {

                var signedCertificateRequest = new CertificateRequest(rootCertificate.Subject, rootCertificate.GetRSAPrivateKey(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var signedCertificate = signedCertificateRequest.Create(rootCertificate, new DateTimeOffset(DateTime.Now), new DateTimeOffset(rootCertificate.NotAfter), Guid.NewGuid().ToByteArray());

                signedCertificate.FriendlyName = leafCertificateName;

                return signedCertificate;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error generating Self-Signed Certificate");
                throw ex;
            }

        }

        private X509Certificate2 GenerateSignedCertificateWithPrivateKey(X509Certificate2 rootCertificate, string leafCertificateName)
        {
            _logger.LogInformation("Entered SignCertificate");

            try
            {
                
                var signedCertificateRequest = new CertificateRequest(rootCertificate.Subject, rootCertificate.GetRSAPrivateKey(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                
                var signedCertificate = signedCertificateRequest.Create(rootCertificate, new DateTimeOffset(DateTime.Now), new DateTimeOffset(rootCertificate.NotAfter), Guid.NewGuid().ToByteArray())
                                                                .CopyWithPrivateKey(rootCertificate.GetRSAPrivateKey());

                signedCertificate.FriendlyName = leafCertificateName;

                return signedCertificate;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error generating Self-Signed Certificate");
                throw ex;
            }

        }

        #endregion

    }
}