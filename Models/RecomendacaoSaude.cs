using System;
using System.Collections.Generic;

namespace OllamaRecomendacaoApi.Models
{
    public class RecomendacaoSaude
    {
        public int UsuarioId { get; set; }
        public DateTime DataRecomendacao { get; set; }
        public string DescricaoRecomendacao { get; set; }
        public string PromptUsado { get; set; }
        public string TituloRecomendacao { get; set; }
        public string TipoSaude { get; set; }
        public string NivelAlerta { get; set; }
        public string MensagemSaude { get; set; }
        public string FonteRecomendacao { get; set; }
    }
}    