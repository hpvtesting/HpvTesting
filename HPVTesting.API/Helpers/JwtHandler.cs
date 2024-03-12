using Google.Apis.Auth;
using HPVTesting.Business.Helpers;
using HPVTesting.Business.Models;

namespace HPVTesting.API.Helpers
{
    public class JwtHandler
    {
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthModel externalAuth)
        {
            try
            {
                var clientId = new GoogleOAuth().ClientId;
                
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
