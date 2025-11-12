using OllamaRecomendacaoApi.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace OllamaRecomendacaoApi.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _model;

        public OllamaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/api/generate";
            _model = configuration["Ollama:Model"] ?? "tinyllama:latest";
            Console.WriteLine($"Ollama Service inicializado. URL: {_baseUrl}, Modelo: {_model}");
        }

        // Recomendação Profissional
        public async Task<RecomendacaoProfissional> ObterRecomendacaoProfissionalAsync(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            var prompt = $@"
Gere uma recomendação de carreira detalhada para o usuário com ID {usuario.Id}.
Considere a área de interesse: {usuario.AreaInteresse}.
Não inclua informações pessoais.
Se disponível, inclua habilidades ou competências do usuário.";

            var body = new { model = _model, prompt = prompt, max_tokens = 300 };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync();

                using var resultJson = JsonDocument.Parse(resultString);
                string recomendacaoTexto = resultJson.RootElement[0].GetProperty("content").GetString() ??
                                           "Recomendação não gerada corretamente.";

                return new RecomendacaoProfissional
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = recomendacaoTexto,
                    PromptUsado = prompt,
                    TituloRecomendacao = "Recomendação de Vagas e Cursos",
                    UsuarioId = usuario.Id,
                    CategoriaRecomendacao = "Profissional",
                    AreaRecomendacao = usuario.AreaInteresse,
                    FonteRecomendacao = "Gerado pelo sistema"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar recomendação profissional: {ex.Message}");
                return new RecomendacaoProfissional
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = $"Recomendação padrão para o usuário ID {usuario.Id}.",
                    PromptUsado = prompt,
                    TituloRecomendacao = "Recomendação de Vagas e Cursos",
                    UsuarioId = usuario.Id,
                    CategoriaRecomendacao = "Profissional",
                    AreaRecomendacao = usuario.AreaInteresse,
                    FonteRecomendacao = "Fallback do sistema"
                };
            }
        }

        // Recomendação de Saúde
        public async Task<RecomendacaoSaude> ObterRecomendacaoSaudeAsync(RecomendacaoSaude request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var registrosTexto = string.Join("\n", request.RegistrosBemEstar.Select(r =>
                $"- Data: {r.DataRegistro:yyyy-MM-dd}, Humor: {r.HumorRegistro}, Sono: {r.HorasSono}h, Trabalho: {r.HorasTrabalho}h, Energia: {r.NivelEnergia}, Estresse: {r.NivelEstresse}, Observação: {r.ObservacaoRegistro}"
            ));

            var prompt = $@"
Gere uma recomendação de saúde detalhada considerando os seguintes registros de bem-estar:
{registrosTexto}

Inclua sugestões de bem-estar físico e emocional, hábitos saudáveis e rotina de exercícios.
Não inclua informações pessoais do usuário.";

            var body = new { model = _model, prompt = prompt, max_tokens = 300 };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync();

                using var resultJson = JsonDocument.Parse(resultString);
                string recomendacaoTexto = resultJson.RootElement[0].GetProperty("content").GetString() ??
                                           "Recomendação não gerada corretamente.";

                return new RecomendacaoSaude
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = recomendacaoTexto,
                    PromptUsado = prompt,
                    TituloRecomendacao = "Recomendação de Bem-Estar",
                    UsuarioId = request.UsuarioId,
                    TipoSaude = "Bem-estar físico e emocional",
                    NivelAlerta = "Moderado",
                    MensagemSaude = recomendacaoTexto,
                    RegistrosBemEstar = request.RegistrosBemEstar, // Mantém os registros enviados
                    FonteRecomendacao = "Gerado pelo sistema"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar recomendação de saúde: {ex.Message}");
                return new RecomendacaoSaude
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = $"Recomendação padrão para o usuário ID {request.UsuarioId}.",
                    PromptUsado = prompt,
                    TituloRecomendacao = "Recomendação de Bem-Estar",
                    UsuarioId = request.UsuarioId,
                    TipoSaude = "Bem-estar físico e emocional",
                    NivelAlerta = "Moderado",
                    MensagemSaude = "Mantenha hábitos saudáveis e pratique exercícios regularmente.",
                    RegistrosBemEstar = request.RegistrosBemEstar, // Mantém os registros enviados
                    FonteRecomendacao = "Fallback do sistema"
                };
            }
        }
    }
}
