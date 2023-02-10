namespace Microsoft.CertGen.API.Model
{
    public class CertGenModel
    {
        
        public string RootCertName { get; set; }

        public bool? SaveRootCertToKeyVault { get; set; }

        public string LeafCertName { get; set; }
    }
}