using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace api.Helpers
{
    public class GoogleTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(string clientId)
    {
        _clientId = clientId;
    }

    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string token)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new[] { _clientId }
        };
        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
        return payload;
    }
}
}