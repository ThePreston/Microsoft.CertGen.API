using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.CertGen.Service;
using Microsoft.Extensions.Logging;
using Microsoft.CertGen.Service.Common;
using System.IO;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Microsoft.CertGen.API.Model;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.CertGen.API
{
    public class CertGen
    {
        private readonly ILogger<CertGen> _logger;

        private readonly ICertificateGenerator _svc;

        public CertGen(ILogger<CertGen> log, ICertificateGenerator service)
        {
            _logger = log;
            _svc = service;
        }

        [FunctionName("CreateRootCert3")]
        [OpenApiParameter(name: "CertGenModel", In = ParameterLocation.Query, Required = true, Type = typeof(CertGenModel), Description = "The Certificate information parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseMessage> CreateRootCert(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation(" CreateRootCert function processed a request.");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var model = JsonConvert.DeserializeObject<CertGenModel>(requestBody);
                                
                if (string.IsNullOrEmpty(model?.RootCertName) || !model.SaveRootCertToKeyVault.HasValue)
                    return HttpUtilities.RESTResponse<CertGenModel>(null);

                var rootCert = await _svc.CreateRootCertificate(model.RootCertName, model.SaveRootCertToKeyVault.Value);

                return model.SaveRootCertToKeyVault.Value ?
                       HttpUtilities.RESTResponse(new { friendlyName = rootCert.FriendlyName }) :
                       HttpUtilities.RESTResponse(new { certificate = rootCert });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateRootCert");

                return HttpUtilities.RESTResponse(ex);
            }
        }

        [FunctionName("CreateLeafCertificate")]
        [OpenApiParameter(name: "CertGenModel", In = ParameterLocation.Query, Required = true, Type = typeof(CertGenModel), Description = "The Certificate information parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(X509Certificate), Description = "The OK response")]
        public async Task<HttpResponseMessage> CreateLeafCertificate([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {

            _logger.LogInformation(" CreateCertificate function processed a request.");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var model = JsonConvert.DeserializeObject<CertGenModel>(requestBody);

                if (string.IsNullOrEmpty(model?.RootCertName) || string.IsNullOrEmpty(model?.LeafCertName))
                    return HttpUtilities.RESTResponse<CertGenModel>(null);

                return HttpUtilities.RESTResponse(new { certificate = await _svc.GenerateSignedCertificate(model.RootCertName, model.LeafCertName) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCertificate");

                return HttpUtilities.RESTResponse(ex);
            }
        }

        [FunctionName("CreateLeafCertificateWithPrivateKey")]
        [OpenApiParameter(name: "CertGenModel", In = ParameterLocation.Query, Required = true, Type = typeof(CertGenModel), Description = "The Certificate information parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(X509Certificate), Description = "The OK response")]
        public async Task<HttpResponseMessage> CreateLeafCertificateWithPrivateKey([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {

            _logger.LogInformation(" CreateCertificate function processed a request.");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var model = JsonConvert.DeserializeObject<CertGenModel>(requestBody);

                if (string.IsNullOrEmpty(model?.RootCertName) || string.IsNullOrEmpty(model?.LeafCertName))
                    return HttpUtilities.RESTResponse<CertGenModel>(null);

                return HttpUtilities.RESTResponse(new { certificate = await _svc.GenerateSignedCertificateWithPrivateKey(model.RootCertName, model.LeafCertName) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCertificate");

                return HttpUtilities.RESTResponse(ex);
            }

        }

    }
}
