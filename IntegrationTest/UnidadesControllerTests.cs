using System.Net.Http.Headers;
using Core;
using Core.Enums;
using Infrastructure.Extensions;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

public class UnidadesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UnidadesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var usuario = new User("Teste", "teste@email.com", "123", UserRole.Admin);
        
        var scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthTokenOptions>>().Value;

        var token = usuario.GenerateJwtToken(options);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GET_ReturnsHttpStatusCode200()
    {
        var request = "/api/Unidades";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GET_MoreEfficient_ReturnsHttpStatusCode200()
    {
        var request = "/api/Unidades/MoreEfficient";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GET_LessEfficient_ReturnsHttpStatusCode200()
    {
        var request = "/api/Unidades/LessEfficient";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }
}