
namespace OllamaRecomendacaoApi.Models;
public class UsuarioRequest
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; }
        public string AreaAtual { get; set; }
        public string AreaInteresse { get; set; }
        public string ObjetivoCarreira { get; set; }
        public string NivelExperiencia { get; set; }
        public string TipoRecomendacao { get; set; } // "Vaga" ou "Curso"
        public List<Competencia> Competencias { get; set; }
    }