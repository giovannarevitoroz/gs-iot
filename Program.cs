using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaRecomendacaoApi.Models;
using OllamaRecomendacaoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona HttpClient para OllamaService
builder.Services.AddHttpClient<OllamaService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilita Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ollama API V1");
    c.RoutePrefix = "swagger"; // http://localhost:5000/swagger
});

// Desativa redirecionamento HTTPS se não estiver configurado
app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});

// Rota para Recomendação Profissional
app.MapPost("/api/recomendacao/profissional", async (Usuario usuario, OllamaService ollamaService) =>
    {
        var recomendacao = await ollamaService.ObterRecomendacaoProfissionalAsync(usuario);
        return recomendacao != null
            ? Results.Ok(recomendacao)
            : Results.BadRequest(new { mensagem = "Não foi possível gerar a recomendação profissional." });
    })
    .WithName("ObterRecomendacaoProfissional")
    .WithTags("Recomendacao");

// Rota para Recomendação de Saúde
app.MapPost("/api/recomendacao/saude", async (RecomendacaoSaude request, OllamaService ollamaService) =>
    {
        var recomendacao = await ollamaService.ObterRecomendacaoSaudeAsync(request);
        return recomendacao != null
            ? Results.Ok(recomendacao)
            : Results.BadRequest(new { mensagem = "Não foi possível gerar a recomendação de saúde." });
    })
    .WithName("ObterRecomendacaoSaude")
    .WithTags("Recomendacao");

app.Run();