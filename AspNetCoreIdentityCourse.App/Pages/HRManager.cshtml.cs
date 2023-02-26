using AspNetCoreIdentityCourse.App.Authorization;
using AspNetCoreIdentityCourse.App.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCoreIdentityCourse.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManagerModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public List<WeatherForecastDto> WeatherForecasts { get; set; } = new();

    public HRManagerModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGetAsync()
    {
        WeatherForecasts = await InvokeEndpoint<List<WeatherForecastDto>>("OurWebApi", "WeatherForecast");
    }

    private async Task<T> InvokeEndpoint<T>(string clientName, string url)
    {
        JwtToken? token = null;
        var stringTokenObject = HttpContext.Session.GetString("access_token");
        var httpClient = _httpClientFactory.CreateClient(clientName);

        if (string.IsNullOrEmpty(stringTokenObject))
        {
            token = await Authenticate(token, httpClient);
        }
        else
        {
            token = JsonSerializer.Deserialize<JwtToken>(stringTokenObject);
        }

        if (token is null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
        {
            token = await Authenticate(token, httpClient);
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return await httpClient.GetFromJsonAsync<T>(url);
    }

    private async Task<JwtToken> Authenticate(JwtToken? token, HttpClient httpClient)
    {
        var response = await httpClient.PostAsJsonAsync("Auth",
                        new Credential { UserName = "admin", Password = "password" });

        response.EnsureSuccessStatusCode();
        var stringJwt = await response.Content.ReadAsStringAsync();
        token = JsonSerializer.Deserialize<JwtToken>(stringJwt);

        HttpContext.Session.SetString("access_token", stringJwt);

        return token!;
    }
}
