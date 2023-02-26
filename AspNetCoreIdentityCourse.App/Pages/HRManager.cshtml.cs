using AspNetCoreIdentityCourse.App.Authorization;
using AspNetCoreIdentityCourse.App.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        var httpClient = _httpClientFactory.CreateClient("OurWebApi");

        var response = await httpClient.PostAsJsonAsync("Auth", 
            new Credential { UserName = "admin", Password = "password" });

        response.EnsureSuccessStatusCode();
        var stringJwt = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<JwtToken>(stringJwt);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        WeatherForecasts = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");
    }
}
