using System.Text.Json.Serialization;

public class RegistrosBemEstar
{
    public DateTime DataRegistro { get; set; }
    public string HumorRegistro { get; set; }
    public int HorasSono { get; set; }
    public int HorasTrabalho { get; set; }
    public int NivelEnergia { get; set; }
    public int NivelEstresse { get; set; }
    public string ObservacaoRegistro { get; set; }
}