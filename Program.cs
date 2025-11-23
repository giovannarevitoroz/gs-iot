using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaRecomendacaoApi.Models;
using OllamaRecomendacaoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<OllamaService>();

builder.Services.AddScoped<OllamaService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Synapse IA API", Version = "v1" });
});

var app = builder.Build();

// Habilita Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Synapse IA API V1");
    c.RoutePrefix = "swagger"; // http://localhost:5000/swagger
});

// Desativa redirecionamento HTTPS se não estiver configurado
app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});

// ========== ROTAS DE RECOMENDAÇÃO PROFISSIONAL ==========

// Rota para Recomendação de VAGAS
app.MapPost("/api/recomendacao/vagas", async (UsuarioRequest request, OllamaService ollamaService) =>
{
    if (request == null)
        return Results.BadRequest(new { erro = "Request inválido" });

    // Força o tipo como Vaga
    request.TipoRecomendacao = "Vaga";

    try
    {
        var recomendacao = await ollamaService.ObterRecomendacaoProfissionalAsync(request);
        return Results.Ok(recomendacao);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro ao gerar recomendação de vagas"
        );
    }
})
.WithName("ObterRecomendacaoVagas")
.WithTags("Recomendacao Profissional")
.Produces<RecomendacaoProfissional>(200)
.Produces(400)
.Produces(500);

// Rota para Recomendação de CURSOS
app.MapPost("/api/recomendacao/cursos", async (UsuarioRequest request, OllamaService ollamaService) =>
{
    if (request == null)
        return Results.BadRequest(new { erro = "Request inválido" });

    // Força o tipo como Curso
    request.TipoRecomendacao = "Curso";

    try
    {
        var recomendacao = await ollamaService.ObterRecomendacaoProfissionalAsync(request);
        return Results.Ok(recomendacao);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro ao gerar recomendação de cursos"
        );
    }
})
.WithName("ObterRecomendacaoCursos")
.WithTags("Recomendacao Profissional")
.Produces<RecomendacaoProfissional>(200)
.Produces(400)
.Produces(500);

// Rota GENÉRICA (aceita tipoRecomendacao no body)
app.MapPost("/api/recomendacao/profissional", async (UsuarioRequest request, OllamaService ollamaService) =>
{
    if (request == null)
        return Results.BadRequest(new { erro = "Request inválido" });

    if (string.IsNullOrWhiteSpace(request.TipoRecomendacao))
        return Results.BadRequest(new { erro = "Campo 'tipoRecomendacao' é obrigatório" });

    if (request.TipoRecomendacao != "Vaga" && request.TipoRecomendacao != "Curso")
        return Results.BadRequest(new { erro = "TipoRecomendacao deve ser 'Vaga' ou 'Curso'" });

    try
    {
        var recomendacao = await ollamaService.ObterRecomendacaoProfissionalAsync(request);
        return Results.Ok(recomendacao);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro ao gerar recomendação profissional"
        );
    }
})
.WithName("ObterRecomendacaoProfissional")
.WithTags("Recomendacao Profissional")
.Produces<RecomendacaoProfissional>(200)
.Produces(400)
.Produces(500);

// ========== ROTA DE RECOMENDAÇÃO DE SAÚDE ==========

app.MapPost("/api/recomendacao/saude", async (RecomendacaoSaudeRequest request, OllamaService ollamaService) =>
{
    Console.WriteLine($"[DEBUG] Request recebido - UsuarioId: {request?.UsuarioId}, Registros: {request?.RegistrosBemEstar?.Count ?? 0}");
    
    if (request == null)
    {
        Console.WriteLine("[DEBUG] Request é null");
        return Results.BadRequest(new { erro = "Request inválido" });
    }

    if (request.RegistrosBemEstar == null || !request.RegistrosBemEstar.Any())
    {
        Console.WriteLine("[DEBUG] RegistrosBemEstar está vazio ou null");
        return Results.BadRequest(new { erro = "É necessário fornecer ao menos um registro de bem-estar" });
    }

    try
    {
        var recomendacao = await ollamaService.ObterRecomendacaoSaudeAsync(request);
        return Results.Ok(recomendacao);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DEBUG] Erro: {ex.Message}");
        Console.WriteLine($"[DEBUG] StackTrace: {ex.StackTrace}");
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro ao gerar recomendação de saúde"
        );
    }
})
.WithName("ObterRecomendacaoSaude")
.WithTags("Recomendacao Saude")
.Produces<RecomendacaoSaude>(200)
.Produces(400)
.Produces(500);

// ========== HEALTH CHECK ==========

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "OK",
        timestamp = DateTime.Now,
        service = "OllamaRecomendacaoApi"
    });
})
.WithName("HealthCheck")
.WithTags("Health")
.Produces(200);

app.Run();