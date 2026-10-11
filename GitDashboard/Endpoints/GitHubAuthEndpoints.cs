using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GitDashboard.Authentication;
using GitDashboard.Configuration;
using GitDashboard.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GitDashboard.Endpoints;

public static class GitHubAuthEndpoints
{
    public static IEndpointRouteBuilder MapGitHubAuthEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapGet("/api/auth/github/login", LoginAsync);
        endpoints.MapGet("/api/auth/github/callback", CallbackAsync);

        return endpoints;
    }

    private static IResult LoginAsync(
        HttpContext context,
        IOptions<GitHubAppOptions> options
    )
    {
        var settings = options.Value;

        // This unique state value is used to prevent CSRF attacks.
        var state = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(32)
        );

        // This is the PKCE verifier and its S256 challenge.
        var codeVerifier = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(32)
        );
        var codeChallenge = WebEncoders.Base64UrlEncode(
            SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier))
        );

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10),
            IsEssential = true,
            Path = "/api/auth/github/callback"
        };

        // Keep state and verifier server-inaccessible to JavaScript.
        context.Response.Cookies.Append(
            "github_oauth_state",
            state,
            cookieOptions
        );
        context.Response.Cookies.Append(
            "github_oauth_verifier",
            codeVerifier,
            cookieOptions
        );

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = settings.ClientId,
            ["redirect_uri"] = settings.CallbackUrl,
            ["state"] = state,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256"
        };

        var authorizationUrl = QueryHelpers.AddQueryString(
            "https://github.com/login/oauth/authorize",
            parameters
        );

        return Results.Redirect(authorizationUrl);
    }

    private static async Task<IResult> CallbackAsync(
        HttpContext context,
        string? code,
        string? state,
        string? error,
        IOptions<GitHubAppOptions> options,
        IHttpClientFactory httpClientFactory,
        GitHubUserTokenStore tokenStore,
        CancellationToken cancellationToken
    )
    {
        // First, read the cookies created by the login endpoint
        var cookies = context.Request.Cookies;
        var expectedState = cookies["github_oauth_state"];
        var codeVerifier = cookies["github_oauth_verifier"];

        // These cookies should only be usable for the callback
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth/github/callback"
        };
        context.Response.Cookies.Delete("github_oauth_state", cookieOptions);
        context.Response.Cookies.Delete("github_oauth_verifier", cookieOptions);

        // Check for error
        if (!string.IsNullOrWhiteSpace(error))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "GitHub authorisation failed",
                detail: "GitHub authorisation was denied or could not be completed"
            );
        }

        // Check for incomplete authorisation
        if (string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(state) ||
                string.IsNullOrWhiteSpace(expectedState) ||
                string.IsNullOrWhiteSpace(codeVerifier))
        {
            return Results.BadRequest("The OAuth callback is incomplete.");
        }

        // Compare state values in constant time
        var suppliedStateBytes = Encoding.UTF8.GetBytes(state);
        var expectedStateBytes = Encoding.UTF8.GetBytes(expectedState);
        if (!CryptographicOperations.FixedTimeEquals(suppliedStateBytes, expectedStateBytes))
        {
            return Results.BadRequest("Invalid OAuth state.");
        }

        var settings = options.Value;
        var client = httpClientFactory.CreateClient("GitHubOAuth");

        // Exchange the temporary authorisation code for a user access token.
        using var tokenRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://github.com/login/oauth/access_token"
        )
        {
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["client_id"] = settings.ClientId,
                    ["client_secret"] = settings.ClientSecret,
                    ["code"] = code,
                    ["redirect_uri"] = settings.CallbackUrl,
                    ["code_verifier"] = codeVerifier
                }
            )
        };

        using var tokenResponse = await client.SendAsync(
            tokenRequest,
            cancellationToken
        );

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "GitHub token exchange failed"
            );
        }

        var token = await tokenResponse.Content
            .ReadFromJsonAsync<GitHubTokenResponse>(cancellationToken: cancellationToken);
        
        if (string.IsNullOrWhiteSpace(token?.AccessToken))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "GitHub did not return an access token"
            );
        }

        // Fetch the identity associated with this token.
        using var userRequest = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.github.com/user"
        );

        userRequest.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", token.AccessToken);

        userRequest.Headers.Accept.ParseAdd("application/vnd.github+json");

        using var userResponse = await client.SendAsync(userRequest, cancellationToken);

        if (!userResponse.IsSuccessStatusCode)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Could not retrieve the GitHub user"
            );
        }

        var user = await userResponse.Content
            .ReadFromJsonAsync<GitHubUserResponse>(cancellationToken: cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Login))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "GitHub returned an invalid user response"
            );
        }

        // Create an opaque session identifier; never put this token into a cookie.
        var sessionId = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(32)
        );

        tokenStore.Set(sessionId, token.AccessToken);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, sessionId),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("github_user_id", user.Id.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            }
        );

        return Results.Redirect("/");
    }
}