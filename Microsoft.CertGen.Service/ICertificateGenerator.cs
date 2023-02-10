using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.CertGen.Service
{
    public interface ICertificateGenerator
    {

        Task<X509Certificate2> GetKVCertificate(string rootCertName);

        Task<X509Certificate2> CreateRootCertificate(string rootCertName, bool saveToKeyVault);

        Task<X509Certificate2> GenerateSignedCertificate(string rootCertName, string leafCertName);

        Task<X509Certificate2> GenerateSignedCertificateWithPrivateKey(string rootCertName, string leafCertName);
    }
}