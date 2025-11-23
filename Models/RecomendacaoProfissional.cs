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
    }
}