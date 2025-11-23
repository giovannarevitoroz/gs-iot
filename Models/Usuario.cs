using System.Collections.Generic;

namespace OllamaRecomendacaoApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string AreaAtual { get; set; } = string.Empty;
        public string AreaInteresse { get; set; } = string.Empty;
        public string ObjetivoCarreira { get; set; } = string.Empty;
        public string NivelExperiencia { get; set; } = string.Empty;
        public List<Competencia> Competencias { get; set; } = new List<Competencia>();
        public string TipoRecomendacao {get; set;} = string.Empty;
    }
    }   