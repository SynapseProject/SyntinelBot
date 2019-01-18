using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using RestSharp;
using RestSharp.Authenticators;

namespace AwsAPI
{
    public class Sig4Authenticator : IAuthenticator
    {
        private const string AuthorizationHeader = "Authorization";
        private const string AmazonDateHeader = "X-Amz-Date";
        private const string EncryptionHeader = "x-amz-content-sha256";
        private const string ServiceName = "execute-api";
        private const string Algorithm = "AWS4-HMAC-SHA256";
        private const string ContentType = "application/json";
        private const string SignedHeaders = "content-type;host;x-amz-date";//;x-api-key
        private const string DateTimeFormat = "yyyyMMddTHHmmssZ";
        private const string DateFormat = "yyyyMMdd";
        private const string AwsRequest = "aws4_request";
        private const string AwsVersion = "AWS4";

        internal string RequestMethod { get; private set; }
        internal string Host { get; private set; }
        internal string XApiKey { get; private set; }
        internal string AbsolutePath { get; private set; }
        internal string QueryString { get; private set; }
        internal AwsApiKey Credentials { get; set; }
        internal string RequestDate { get; private set; }
        internal IList<Parameter> RequestHeaders { get; private set; }
        internal IDictionary<string, string> QueryStringParameters { get; set; }

        public Sig4Authenticator(AwsApiKey credentials)
        {
            Credentials = credentials;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            AssignLocalMembers(client, request);
            var payLoad = GetPayload(request);
            var hashedRequest = HashPayload(payLoad);
            var signature = Sign(hashedRequest);
            request.AddHeader(AuthorizationHeader, signature);

            request.AddHeader(AmazonDateHeader, RequestDate);
            request.AddBody(EncryptionHeader, hashedRequest);
        }

        private string Sign(string payload)
        {
            var currentDateTime = DateTime.UtcNow;
            var dateStamp = currentDateTime.ToString(DateFormat);
            RequestDate = currentDateTime.ToString(DateTimeFormat);

            var credentialScope = $"{dateStamp}/{Credentials.Region}/{ServiceName}/{AwsRequest}";

            var headers = GetCanoncicalHeaders(RequestDate);
            var canonicalHeaders = string.Join("\n", headers.Select(x => x.Key.ToLowerInvariant() + ":" + x.Value.Trim())) + "\n";

            // Step 1: Create the canonical request
            var canonicalRequest = $"{RequestMethod}\n{AbsolutePath}\n{QueryString}\n{canonicalHeaders}\n{SignedHeaders}\n{payload}";
            var hashedCanonicalRequest = GetHashedCanonicalRequest(canonicalRequest);

            // Step 2: Create the string to sign
            var stringToSign = $"{Algorithm}\n{RequestDate}\n{credentialScope}\n{hashedCanonicalRequest}";

            // Step 3: Create the signature
            var signingKey = GetSignatureKey(@Credentials.SecretKey, dateStamp, Credentials.Region, ServiceName);
            var signature = HmacSha256(stringToSign, signingKey).HexEncode();

            // Step 4: format the auth field with signature                      
            var authorization = $"{Algorithm} Credential={Credentials.AccessKey}/{dateStamp}/{Credentials.Region}/{ServiceName}/{AwsRequest}, SignedHeaders={SignedHeaders}, Signature={signature}";

            return authorization;
        }

        private void AssignLocalMembers(IRestClient client, IRestRequest request)
        {
            RequestMethod = request.Method.ToString();
            AbsolutePath = request.Resource;
            RequestHeaders = request.Parameters.FindAll(e => e.Type == ParameterType.HttpHeader);

            var qs = request.Parameters.FindAll(e => e.Type == ParameterType.QueryString);
            foreach (var parameter in qs)
            {
                QueryStringParameters.Add(parameter.Name, parameter.Value.ToString());
            }

            Host = client.BaseUrl.Host;
            XApiKey = string.Empty;
        }

        private SortedDictionary<string, string> GetCanoncicalHeaders(string requestDate)
        {
            var headers = new SortedDictionary<string, string> {
                { "content-type", ContentType },
                { "host", Host },
                { "x-amz-date", requestDate }
            };

            return headers;
        }

        private byte[] GetSignatureKey(string secretKey, string dateStamp, string regionName, string serviceName)
        {
            var kBytes = (AwsVersion + secretKey).ToBytes();
            var kDate = HmacSha256(dateStamp, kBytes);
            var kRegion = HmacSha256(regionName, kDate);
            var kService = HmacSha256(serviceName, kRegion);
            return HmacSha256(AwsRequest, kService);
        }

        private string GetHashedCanonicalRequest(string content)
        {
            return content.ToBytes().Hash().HexEncode();
        }

        private string HashPayload(string payload)
        {
            return payload.ToBytes().Hash().HexEncode();
        }

        private string GetPayload(IRestRequest request)
        {
            var body = request.Parameters.Find(e => e.Type == ParameterType.RequestBody);
            return body.Value.ToString();
        }
        private byte[] HmacSha256(string data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(data.ToBytes());
        }
    }
}