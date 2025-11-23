using System.Text.Json.Serialization;

namespace OllamaRecomendacaoApi.Models;
public class RegistroBemEstar
{
    public DateTime DataRegistro { get; set; }
    public string HumorRegistro { get; set; }
    public double HorasSono { get; set; }
    public double HorasTrabalho { get; set; }
    public int NivelEnergia { get; set; }
    public int NivelEstresse { get; set; }
    public string ObservacaoRegistro { get; set; }
}