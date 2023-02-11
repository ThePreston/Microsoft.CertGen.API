# Microsoft.CertGen.API
Generate Root Certifiicates ans use Root Certs to sign Leaf Certificates

Azure Function that Provides Certificate Services

1) CreateRootCert
2) CreateLeafCertificate
3) CreateLeafCertificateWithPrivateKey

### Disclaimer, I am not a Security, Cryptography, Certificate Expert. The Root Cert Saves to KeyVault but the Certificates it signs do not, for some reason.

## CreateRootCert
Creates a Self-Signed Certificate and saves it to KeyVault if the App/Context of the running use has permission to access Key Vault


## CreateLeafCertificate
Uses a Root Certificate that already exists in Key Vault and uses that to create a new certificate. This Certificate does not have a Private Key and cannot be saved to KeyVault

## CreateLeafCertificateWithPrivateKey
Uses a Root Certificate that already exists in Key Vault and uses that to create a new certificate. This Certificate does have a Private Key and cannot be saved to KeyVault
