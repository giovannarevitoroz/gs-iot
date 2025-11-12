using System.Text.Json.Serialization;

namespace OllamaRecomendacaoApi.Models
{
    public class RecomendacaoProfissional
    {
        public DateTime DataRecomendacao { get; set; }
        public string DescricaoRecomendacao { get; set; }
        public string PromptUsado { get; set; }
        public string TituloRecomendacao { get; set; }
        public int UsuarioId { get; set; }
        public string CategoriaRecomendacao { get; set; }
        public string AreaRecomendacao { get; set; }
        public string FonteRecomendacao { get; set; }

        // Remover ou marcar como ignoradas
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string NomeUsuario { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string AreaAtual { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ObjetivoCarreira { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string NivelExperiencia { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Competencia> Competencias { get; set; }
    }
}