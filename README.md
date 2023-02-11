# Microsoft.CertGen.API
Generate Root Certifiicates ans use Root Certs to sign Leaf Certificates

Azure Function that Provides Certificate Services

1) CreateRootCert
2) CreateLeafCertificate
3) CreateLeafCertificateWithPrivateKey

## POST body JSON object
{
"RootCertName":"",
"SaveRootCertToKeyVault" : "true",
"LeafCertName":""
}

1) RootCertName - (string) the name of the Root Cert you want to create OR the name of an Existing Root Cert that you want to use to sign Leaf Certificates
2) SaveRootCertToKeyVault - (bool) determines if the Root Certificate will be saved to keyVault
3) LeafCertName - (string) the name of the Leaf Certificate that will be created and signed by the Root Cert 

## CreateRootCert
Creates a Self-Signed Certificate and saves it to KeyVault if the App/Context of the running use has permission to access Key Vault


## CreateLeafCertificate
Uses a Root Certificate that already exists in Key Vault and uses that to create a new certificate. This Certificate does not have a Private Key and cannot be saved to KeyVault

## CreateLeafCertificateWithPrivateKey
Uses a Root Certificate that already exists in Key Vault and uses that to create a new certificate. This Certificate does have a Private Key and cannot be saved to KeyVault

#### Disclaimer, I am not a Security, Cryptography or Certificate Expert.
