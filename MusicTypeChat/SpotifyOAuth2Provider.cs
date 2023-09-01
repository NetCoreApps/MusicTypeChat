using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using ServiceStack.Auth;

namespace MusicTypeChat;

using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Create an OAuth2 App at: https://developer.spotify.com/dashboard/applications
/// The Apps Callback URL should match the CallbackUrl here.
/// Spotify OAuth2 info: https://developer.spotify.com/documentation/general/guides/authorization-guide/
/// </summary>
public class SpotifyAuthProvider : OAuth2Provider
{
    public const string Name = "spotify";
    public static string Realm = DefaultAuthorizeUrl;

    const string DefaultAuthorizeUrl = "https://accounts.spotify.com/authorize";
    const string DefaultAccessTokenUrl = "https://accounts.spotify.com/api/token";
    const string DefaultUserProfileUrl = "https://api.spotify.com/v1/me";

    public SpotifyAuthProvider(IAppSettings appSettings)
        : base(appSettings, Realm, Name, "ClientId", "ClientSecret")
    {
        AuthorizeUrl = appSettings.Get($"oauth.{Name}.AuthorizeUrl", DefaultAuthorizeUrl);
        AccessTokenUrl = appSettings.Get($"oauth.{Name}.AccessTokenUrl", DefaultAccessTokenUrl);
        UserProfileUrl = appSettings.Get($"oauth.{Name}.UserProfileUrl", DefaultUserProfileUrl);

        if (Scopes == null || Scopes.Length == 0)
        {
            Scopes = new[]
            {
                "user-read-private",
                "user-read-email"
            };
        }

        // You can customize the icon and sign-in button here.
        NavItem = new NavItem
        {
            Href = "/auth/" + Name,
            Label = "Sign In with Spotify",
            Id = "btn-" + Name,
            ClassName = "btn-social btn-spotify",
            IconClass = "fab fa-spotify",
        };
    }
    
    protected override async Task<string> GetAccessTokenJsonAsync(string code, AuthContext ctx,
        CancellationToken token = new())
    {
        var payload = $"code={code}&redirect_uri={CallbackUrl.UrlEncode()}&grant_type=authorization_code";
    
        var url = AccessTokenUrlFilter(ctx, AccessTokenUrl);
        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConsumerKey}:{ConsumerSecret}"));
    
        var contents = await url.PostToUrlAsync(payload, 
            responseFilter: (res) =>
            {
                // Log details
                Console.WriteLine($"Response Status Code: {res.StatusCode}");
                Console.WriteLine($"Response Status Description: {res.ReasonPhrase}");
            },
            requestFilter: req => req.Headers.Add("Authorization", $"Basic {base64String}"),
            token: token).ConfigAwait();
    
        return contents;
    }


    protected override async Task<Dictionary<string, string>> CreateAuthInfoAsync(string accessToken,
        CancellationToken token = new())
    {
        var json = await DefaultUserProfileUrl
            .GetJsonFromUrlAsync(request => { request.Headers.Add("Authorization", "Bearer " + accessToken); },
                token: token).ConfigAwait();
        var obj = JsonObject.Parse(json);
        
        obj.Add("name", obj["display_name"]);
        obj.MoveKey("id", "user_id");
        if (obj.ContainsKey("images") && !string.IsNullOrEmpty(obj["images"]))
        {
            if (JsonNode.Parse(obj["images"]) is JsonArray { Count: > 0 } imagesArray)
            {
                var firstImage = imagesArray[0]?.AsObject();
                if (firstImage != null && ((IDictionary<string, JsonNode?>)firstImage).TryGetValue("url", out var value))
                {
                    obj[AuthMetadataProvider.ProfileUrlKey] = value?.ToString();
                }
            }
        }
        return obj;
    }
}
