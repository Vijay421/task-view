
namespace WebApiTests.IntegrationTests;

public class AuthUtil
{
    /// <summary>
    /// Tries to set the auth cookie in the given client, while the login response.
    /// </summary>
    /// <param name="loginResponse"></param>
    /// <param name="client"></param>
    /// <exception cref="Exception"></exception>
    public static void SetAuthCookie(HttpResponseMessage loginResponse, HttpClient client)
    {
        if (loginResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                break;
            }
        }
        else
        {
            throw new Exception("Could not find 'Set-Cookie' in the login response");
        }
    }
}
