using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Octokit.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Bot
{
    public static class GitHubClientFactory
    {
        public static  GitHubClient CreateGitHubAppClient(GitHubOption option)
        {
            return GetAppClient(option, option.AppName);

        }

        public static async Task<InstallationContext> CreateGitHubInstallationClient(GitHubClient appClient, long installationId, string appName)
        {
            return await GetInstallationContext(appClient, installationId, appName);
        }

        public static async Task<InstallationContext> CreateGitHubInstallationClient(GitHubOption option,long installationId)
        {
            return await CreateGitHubInstallationClient(CreateGitHubAppClient(option), installationId, option.AppName);
        }

        private static async Task<InstallationContext> GetInstallationContext(GitHubClient appClient, long installationId, string appName)
        {
            var accessToken = await appClient.GitHubApps.CreateInstallationToken(installationId);

            var installationClient = new ResilientGitHubClientFactory()
                .Create(new ProductHeaderValue($"{appName}-Installation{installationId}"), new Credentials(accessToken.Token), new InMemoryCacheProvider());

            return new InstallationContext(installationClient,accessToken);
        }

        private static GitHubClient GetAppClient(GitHubOption option, string appName)
        {
            var rsaParams = GetRsaParameters(option.PrivateKey);

            var payload = GetPayload(option);

            string jwtToken;

            using (var csp = new RSACryptoServiceProvider())
            {
                csp.ImportParameters(rsaParams);
                var encoder = GetRS256JWTEncoder(csp);
                jwtToken = encoder.Encode(payload, new byte[0]);
            }

           return GetGitHubClient(appName, jwtToken);
        }

        private static GitHubClient GetGitHubClient(string appName, string jwtToken)
        {
            return new ResilientGitHubClientFactory()
                .Create(new ProductHeaderValue(appName), new Credentials(jwtToken, AuthenticationType.Bearer), new InMemoryCacheProvider());
        }

        private static Dictionary<string, object> GetPayload(GitHubOption option)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>
                {
                    { "iat", now },
                    { "exp", now + 550 },
                    { "iss", option.AppIdentifier }
                };
            return payload;
        }

        private static IJwtEncoder GetRS256JWTEncoder(RSACryptoServiceProvider csp)
        {
            var algorithm = new RS256Algorithm(publicKey: csp, privateKey: csp); // sending public key is not nesseccary but we have to to pass it.
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder;
        }

        private static RSAParameters GetRsaParameters(string rsaPrivateKey)
        {
            var byteArray = Encoding.ASCII.GetBytes(rsaPrivateKey);
            using (var ms = new MemoryStream(byteArray))
            {
                using (var sr = new StreamReader(ms))
                {
                    var pemReader = new PemReader(sr);

                    var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                    return DotNetUtilities.ToRSAParameters(keyPair.Private as RsaPrivateCrtKeyParameters);
                }
            }
        }
    }
}
