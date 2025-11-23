 namespace OllamaRecomendacaoApi.Models;
 public class RecomendacaoSaudeRequest
    {
        public int UsuarioId { get; set; }
        public List<RegistroBemEstar> RegistrosBemEstar { get; set; } = new List<RegistroBemEstar>();
    }