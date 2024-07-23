using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using api.Exceptions;
using Newtonsoft.Json.Linq;

namespace api.Helpers
{
    public class FacebookTokenValidator
    {
        private readonly string _appSecret;

        public FacebookTokenValidator(string appSecret)
        {
            _appSecret = appSecret;
        }

        public async Task<JObject> ValidateAndGetPayloadAsync(string accessToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Get user info from the token
                    var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,birthday&access_token={accessToken}";
                    var userInfoResponse = await httpClient.GetStringAsync(userInfoUrl);
                    var userInfoJson = JObject.Parse(userInfoResponse);

                    return userInfoJson;
                }
            }
 
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new AppException($"Token is not valid", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
