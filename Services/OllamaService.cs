using System.Text;
using System.Text.Json;
using OllamaRecomendacaoApi.Models;

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

            var baseUrlConfig = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";

            _baseUrl = $"{baseUrlConfig.TrimEnd('/')}/api/generate";

            _model = configuration["Ollama:Model"] ?? "llama3.2:3b";

            Console.WriteLine($"Ollama Service inicializado. URL: {_baseUrl}, Modelo: {_model}");
        }


        // Recomendação Profissional (Vagas ou Cursos)
        public async Task<RecomendacaoProfissional> ObterRecomendacaoProfissionalAsync(UsuarioRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.TipoRecomendacao != "Vaga" && request.TipoRecomendacao != "Curso")
            {
                throw new ArgumentException("TipoRecomendacao deve ser 'Vaga' ou 'Curso'");
            }

            var competenciasTexto = string.Join(", ",
                request.Competencias.Select(c => $"{c.NomeCompetencia} ({c.CategoriaCompetencia})"));

            var prompt = GerarPromptPersonalizado(request, competenciasTexto);

            var body = new
            {
                model = _model,
                prompt = prompt,
                stream = false,
                options = new
                {
                    temperature = 0.7,   
                    top_p = 0.9,         
                    top_k = 40,         
                    num_predict = 800    
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync();

                using var resultJson = JsonDocument.Parse(resultString);

                string recomendacaoTexto = resultJson.RootElement.GetProperty("response").GetString() ??
                                           $"Recomendação de {request.TipoRecomendacao} não gerada corretamente.";

                return new RecomendacaoProfissional
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = recomendacaoTexto.Trim(),
                    PromptUsado = prompt,
                    TituloRecomendacao = $"Recomendação de {request.TipoRecomendacao}",
                    UsuarioId = request.Id,
                    CategoriaRecomendacao = request.TipoRecomendacao,
                    AreaRecomendacao = request.AreaInteresse,
                    FonteRecomendacao = $"Gerado por {_model}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar recomendação: {ex.Message}");
                return GerarRecomendacaoFallback(request, prompt);
            }
        }

        private string GerarPromptPersonalizado(UsuarioRequest request, string competenciasTexto)
        {
            if (request.TipoRecomendacao == "Vaga")
            {
                return $@"Você é um especialista em recrutamento e desenvolvimento de carreira em tecnologia no Brasil.

**CONTEXTO DO CANDIDATO:**
Nome: {request.NomeUsuario}
Posição Atual: {request.AreaAtual}
Área de Interesse: {request.AreaInteresse}
Objetivo de Carreira: {request.ObjetivoCarreira}
Nível de Experiência: {request.NivelExperiencia}
Competências Técnicas: {competenciasTexto}

**SUA TAREFA:**
Analise cuidadosamente o perfil acima e recomende A VAGA IDEAL - aquela que melhor se encaixa com o momento de carreira, competências e objetivos do candidato.

Forneça uma recomendação detalhada incluindo:

1. **Título da Vaga**: Nome específico e realista da posição
2. **Por que esta é A vaga ideal**: Explicação clara de como ela se alinha perfeitamente ao perfil e objetivo do candidato
3. **Tipo de Empresa**: Segmento/indústria e porte (startup, scale-up, corporação)
4. **Responsabilidades Principais**: 4-5 atividades que o candidato realizaria no dia a dia
5. **Requisitos Técnicos**: Tecnologias e competências necessárias (relacione com as que o candidato já possui)
6. **Diferenciais do Candidato**: Como as competências atuais ({competenciasTexto}) se encaixam nesta vaga
7. **Faixa Salarial Estimada**: Valor realista para o nível {request.NivelExperiencia} no mercado brasileiro (em BRL)
8. **Modalidade**: Presencial/Híbrido/Remoto
9. **Perspectivas de Crescimento**: Como esta vaga contribui para alcançar '{request.ObjetivoCarreira}'
10. **Onde Buscar**: Sites/plataformas específicas onde vagas deste tipo são publicadas (LinkedIn, Gupy, Programathor, etc)
11. **Dicas para se Destacar**: 3 ações concretas para aumentar as chances de conseguir esta vaga

**IMPORTANTE:**
- Seja extremamente específico e realista com o mercado brasileiro atual
- Considere o nível de experiência para não sugerir algo inalcançável ou muito básico
- Foque na vaga que REALMENTE faz sentido agora para este candidato
- Use linguagem profissional mas acessível e motivadora

Estruture a resposta de forma clara, objetiva e inspiradora.";
            }
            else // Curso
            {
                return $@"Você é um mentor de desenvolvimento profissional especializado em tecnologia.

**CONTEXTO DO ESTUDANTE:**
Nome: {request.NomeUsuario}
Posição Atual: {request.AreaAtual}
Área de Interesse: {request.AreaInteresse}
Objetivo de Carreira: {request.ObjetivoCarreira}
Nível Atual: {request.NivelExperiencia}
Competências Atuais: {competenciasTexto}

**SUA TAREFA:**
Analise cuidadosamente o perfil acima e recomende O CURSO/CAPACITAÇÃO MAIS IMPORTANTE - aquele que trará o maior impacto para alcançar o objetivo '{request.ObjetivoCarreira}' neste momento da carreira.

Forneça uma recomendação detalhada incluindo:

1. **Nome do Curso**: Título específico e realista (se possível, mencione um curso real existente)
2. **Por que este é O curso ideal agora**: Explicação clara de como ele se alinha perfeitamente ao objetivo e fecha gaps importantes
3. **Plataforma Recomendada**: Onde encontrar (Udemy, Coursera, Alura, DIO, Rocketseat, etc) - seja específico
4. **Duração Estimada**: Tempo realista necessário para conclusão (considerando dedicação de estudo)
5. **Investimento**: Valor aproximado (gratuito, R$ X, assinatura mensal)
6. **Conteúdo Detalhado**: 6-8 tópicos principais que serão abordados no curso
7. **Nível de Dificuldade**: Iniciante/Intermediário/Avançado (e se é adequado para o nível {request.NivelExperiencia})
8. **Pré-requisitos**: O que já é esperado saber antes de começar (relacione com {competenciasTexto})
9. **Habilidades que serão desenvolvidas**: Competências práticas específicas que serão adquiridas
10. **Projeto Final/Certificado**: O que será entregue ao concluir (portfólio, certificação, projeto prático)
11. **Conexão com o Mercado**: Como este curso é visto por recrutadores e empresas na área de {request.AreaInteresse}
12. **Próximos Passos após o Curso**: O que estudar em seguida para dar continuidade ao aprendizado

**CRITÉRIOS IMPORTANTES:**
- Priorize cursos disponíveis em português ou com legendas em PT-BR
- Considere o nível atual ({request.NivelExperiencia}) para não sugerir algo muito básico ou muito avançado
- Foque no curso que dará o MAIOR retorno para alcançar '{request.ObjetivoCarreira}'
- Seja realista sobre disponibilidade e custo no contexto brasileiro
- Inclua dicas práticas de como aproveitar melhor o curso

Estruture a resposta de forma clara, motivadora e prática.";
            }
        }

        private RecomendacaoProfissional GerarRecomendacaoFallback(UsuarioRequest request, string prompt)
        {
            var competenciasTexto = string.Join(", ", request.Competencias.Select(c => c.NomeCompetencia));

            string descricaoFallback = request.TipoRecomendacao == "Vaga"
                ? $@"**A VAGA IDEAL PARA VOCÊ: {request.ObjetivoCarreira.ToUpper()}**

🎯 **Por que esta vaga é perfeita:**
Considerando seu objetivo de '{request.ObjetivoCarreira}', experiência como {request.AreaAtual} e interesse em {request.AreaInteresse}, esta posição oferece a transição ideal para sua carreira.

📋 **Título da Vaga:**
{request.ObjetivoCarreira} - Nível {request.NivelExperiencia}

🏢 **Tipo de Empresa:**
Startups e scale-ups de tecnologia, fintechs, empresas de produto digital

💼 **Responsabilidades Principais:**
• Desenvolver e manter aplicações em {request.AreaInteresse}
• Trabalhar com tecnologias como {competenciasTexto}
• Participar de code reviews e boas práticas de desenvolvimento
• Colaborar com equipes ágeis em projetos inovadores

✅ **Seus Diferenciais:**
• Domínio de: {competenciasTexto}
• Background em {request.AreaAtual} traz visão diferenciada
• Perfil {request.NivelExperiencia} ideal para crescimento na empresa

💰 **Faixa Salarial Estimada:**
{(request.NivelExperiencia == "Júnior" ? "R$ 3.000 - R$ 5.000" :
  request.NivelExperiencia == "Pleno" ? "R$ 6.000 - R$ 10.000" : "R$ 12.000 - R$ 18.000")}

🏠 **Modalidade:**
Híbrido ou Remoto (preferência do mercado atual)

📈 **Crescimento na Carreira:**
Esta vaga é o primeiro passo para alcançar '{request.ObjetivoCarreira}', oferecendo:
• Experiência prática em {request.AreaInteresse}
• Mentoria de profissionais seniores
• Oportunidades de evolução para níveis mais altos

🔍 **Onde Buscar:**
• LinkedIn: Configure alertas para {request.ObjetivoCarreira}
• Gupy: Maior plataforma de vagas tech no Brasil
• Programathor: Focado em desenvolvedores
• GeekHunter: Empresas vêm até você
• Trampos.co: Vagas em startups

💡 **Dicas para Se Destacar:**
1. **Portfólio Forte**: Crie 2-3 projetos usando {competenciasTexto} e publique no GitHub
2. **LinkedIn Otimizado**: Destaque suas competências em {request.AreaInteresse} e projetos realizados
3. **Networking**: Entre em comunidades tech (Discord/Telegram) de {request.AreaInteresse}

🎯 **Próximos Passos Imediatos:**
1. Atualize seu currículo focando em {competenciasTexto}
2. Configure alertas de vagas com as palavras-chave: '{request.ObjetivoCarreira}'
3. Prepare-se para entrevistas técnicas estudando casos comuns da área"
                : $@"**O CURSO IDEAL PARA VOCÊ: RUMO A {request.ObjetivoCarreira.ToUpper()}**

🎯 **Por que este curso é perfeito agora:**
Com base no seu objetivo '{request.ObjetivoCarreira}', background em {request.AreaAtual} e competências em {competenciasTexto}, este curso preencherá os gaps mais importantes e acelerará sua transição para {request.AreaInteresse}.

📚 **Nome do Curso:**
{request.AreaInteresse} Completo - Do Zero ao {request.NivelExperiencia} Avançado

🏫 **Plataforma Recomendada:**
{(request.AreaInteresse.Contains("Desenvolvimento") || request.AreaInteresse.Contains("Backend") || request.AreaInteresse.Contains("Frontend")
  ? "Alura (assinatura) ou Udemy (pagamento único)"
  : "Coursera (certificação reconhecida) ou DIO (gratuito com certificado)")}

⏱️ **Duração Estimada:**
{(request.NivelExperiencia == "Júnior" ? "40-60 horas (2-3 meses dedicando 1h/dia)" :
  request.NivelExperiencia == "Pleno" ? "60-80 horas (2-3 meses)" : "80-100 horas (3-4 meses)")}

💰 **Investimento:**
{(request.NivelExperiencia == "Júnior" ? "R$ 50-150 (pagamento único) ou R$ 30-80/mês (assinatura)" : "R$ 100-300")}

📖 **Conteúdo Detalhado:**
• Fundamentos de {request.AreaInteresse} e conceitos essenciais
• Aprofundamento em {competenciasTexto}
• Arquitetura e design patterns aplicados
• Boas práticas e clean code
• Testes automatizados e qualidade de software
• Projetos práticos reais do mercado
• Integração com ferramentas modernas
• Preparação para entrevistas técnicas

📊 **Nível de Dificuldade:**
{request.NivelExperiencia} → Intermediário
✅ Adequado para quem já tem base em {competenciasTexto}

✔️ **Pré-requisitos:**
• Conhecimentos que você já tem: {competenciasTexto}
• Lógica de programação básica
• Vontade de construir projetos práticos

🚀 **Habilidades que Você Desenvolverá:**
• Domínio completo de {request.AreaInteresse}
• Desenvolvimento de aplicações profissionais
• Metodologias ágeis e trabalho em equipe
• Problem solving e debugging avançado
• Portfólio com 3-5 projetos reais

🎓 **Projeto Final e Certificado:**
• Projeto completo de aplicação real para portfólio no GitHub
• Certificado reconhecido no mercado brasileiro
• Material de referência vitalício

💼 **Conexão com o Mercado:**
Este curso é altamente valorizado por empresas que buscam profissionais em {request.AreaInteresse}. O certificado é reconhecido e o portfólio gerado abre portas para vagas de {request.NivelExperiencia}.

📈 **Próximos Passos Após o Curso:**
1. **Curso Avançado**: Especialização em arquitetura e patterns avançados
2. **Certificação**: Buscar certificações oficiais da área (se aplicável)
3. **Projetos Open Source**: Contribuir com comunidade tech
4. **Inglês Técnico**: Melhorar compreensão de documentações

💡 **Dicas para Aproveitar Melhor:**
1. **Estude 1h/dia consistentemente** - melhor que maratonas
2. **Faça TODOS os projetos práticos** - não pule exercícios
3. **Compartilhe no LinkedIn** seu progresso e projetos
4. **Entre em comunidades** (Discord/Telegram) do curso

🎯 **Por que começar AGORA:**
• Alinha diretamente com '{request.ObjetivoCarreira}'
• Complementa perfeitamente suas competências em {competenciasTexto}
• Mercado está aquecido para profissionais com este perfil
• Cada semana de estudo é um passo mais perto do seu objetivo

🔗 **Onde Encontrar:**
• Alura: alura.com.br → Busque por '{request.AreaInteresse}'
• Udemy: udemy.com → Filtro por melhor avaliados em PT-BR
• DIO: dio.me → Bootcamps gratuitos
• Coursera: coursera.org → Certificados profissionais

**Comece hoje! Seu futuro em {request.AreaInteresse} está a um curso de distância.** 🚀";

            return new RecomendacaoProfissional
            {
                DataRecomendacao = DateTime.Now,
                DescricaoRecomendacao = descricaoFallback,
                PromptUsado = prompt,
                TituloRecomendacao = $"Recomendação de {request.TipoRecomendacao}",
                UsuarioId = request.Id,
                CategoriaRecomendacao = request.TipoRecomendacao,
                AreaRecomendacao = request.AreaInteresse,
                FonteRecomendacao = "Fallback estruturado do sistema"
            };
        }

        // Recomendação de Saúde
        public async Task<RecomendacaoSaude> ObterRecomendacaoSaudeAsync(RecomendacaoSaudeRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var registrosTexto = string.Join("\n", request.RegistrosBemEstar.Select(r =>
                $"• {r.DataRegistro:dd/MM/yyyy}: Humor {r.HumorRegistro}, Sono {r.HorasSono}h, Trabalho {r.HorasTrabalho}h, Energia {r.NivelEnergia}, Estresse {r.NivelEstresse}. Obs: {r.ObservacaoRegistro}"
            ));

            var prompt = $@"Você é um assistente de bem-estar especializado em saúde ocupacional e qualidade de vida.

**REGISTROS DE BEM-ESTAR DO USUÁRIO:**
{registrosTexto}

**SUA TAREFA:**
Analise os registros acima e crie uma recomendação personalizada de bem-estar incluindo:

1. **Análise Geral**: Resumo dos padrões identificados (sono, trabalho, energia, estresse)
2. **Alertas Importantes**: Pontos de atenção que precisam de cuidado imediato
3. **Recomendações de Rotina**:
   - Ajustes no sono
   - Gestão do tempo de trabalho
   - Pausas e descanso
4. **Práticas de Bem-Estar**:
   - Exercícios físicos adequados
   - Técnicas de relaxamento
   - Mindfulness/meditação
5. **Hábitos Saudáveis**: 4-5 ações práticas e imediatas
6. **Quando Buscar Ajuda**: Sinais de que é hora de consultar um profissional

**IMPORTANTE:**
- Seja empático e acolhedor
- Dê recomendações práticas e aplicáveis
- Considere o contexto de trabalho brasileiro
- Não dê diagnósticos médicos, apenas orientações gerais de bem-estar

Estruture sua resposta de forma clara e motivadora.";

            var body = new
            {
                model = _model,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();

                var resultString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resultString);

                var recomendacaoTexto = doc.RootElement
                    .GetProperty("response")
                    .GetString()
                    ?? "Recomendação não gerada corretamente.";


                var nivelAlerta = DeterminarNivelAlerta(request.RegistrosBemEstar);

                return new RecomendacaoSaude
                {
                    DataRecomendacao = DateTime.Now,
                    DescricaoRecomendacao = recomendacaoTexto.Trim(),
                    PromptUsado = prompt,
                    TituloRecomendacao = "Recomendação de Bem-Estar Personalizada",
                    UsuarioId = request.UsuarioId,
                    TipoSaude = "Bem-estar físico e emocional",
                    NivelAlerta = nivelAlerta,
                    MensagemSaude = recomendacaoTexto.Trim(),
                    FonteRecomendacao = $"Gerado por {_model}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar recomendação de saúde: {ex.Message}");
                return GerarRecomendacaoSaudeFallback(request, prompt);
            }
        }


        private string DeterminarNivelAlerta(List<RegistroBemEstar> registros)
        {
            var mediaHorasSono = registros.Average(r => r.HorasSono);
            var mediaHorasTrabalho = registros.Average(r => r.HorasTrabalho);

            if (mediaHorasSono < 6 || mediaHorasTrabalho > 10)
                return "Alto";
            else if (mediaHorasSono < 7 || mediaHorasTrabalho > 8)
                return "Moderado";
            else
                return "Baixo";
        }

        private RecomendacaoSaude GerarRecomendacaoSaudeFallback(RecomendacaoSaudeRequest request, string prompt)
        {
            var nivelAlerta = DeterminarNivelAlerta(request.RegistrosBemEstar);
            var mediaHorasSono = request.RegistrosBemEstar.Average(r => r.HorasSono);
            var mediaHorasTrabalho = request.RegistrosBemEstar.Average(r => r.HorasTrabalho);

            var mensagem = $@"**ANÁLISE DO SEU BEM-ESTAR**

📊 **Seus Números:**
- Sono médio: {mediaHorasSono:F1} horas/noite
- Trabalho médio: {mediaHorasTrabalho:F1} horas/dia
- Período analisado: {request.RegistrosBemEstar.Count} registros

⚠️ **Nível de Alerta: {nivelAlerta}**

🎯 **Recomendações Prioritárias:**

1. **Rotina de Sono:**
   {(mediaHorasSono < 7 ? "• Aumente gradualmente seu tempo de sono para 7-8h" : "• Mantenha sua boa rotina de sono")}
   • Estabeleça horários fixos para dormir e acordar
   • Evite telas 1h antes de dormir

2. **Gestão do Trabalho:**
   {(mediaHorasTrabalho > 8 ? "• Reduza as horas extras quando possível" : "• Sua carga de trabalho está equilibrada")}
   • Faça pausas de 5min a cada hora
   • Use técnica Pomodoro (25min foco + 5min pausa)

3. **Atividade Física:**
   • 30 minutos de caminhada diária
   • Alongamentos durante pausas do trabalho
   • Exercícios de respiração profunda

4. **Bem-Estar Emocional:**
   • Pratique mindfulness 10min/dia
   • Mantenha conexões sociais
   • Reserve tempo para hobbies

5. **Hábitos Alimentares:**
   • Hidratação adequada (2L água/dia)
   • Refeições regulares e balanceadas
   • Evite cafeína após 16h

🏥 **Quando Buscar Ajuda:**
- Insônia persistente por mais de 2 semanas
- Ansiedade ou estresse constante
- Fadiga que não melhora com descanso

💡 **Lembre-se:** Pequenas mudanças diárias geram grandes resultados!";

            return new RecomendacaoSaude
            {
                DataRecomendacao = DateTime.Now,
                DescricaoRecomendacao = mensagem,
                PromptUsado = prompt,
                TituloRecomendacao = "Recomendação de Bem-Estar",
                UsuarioId = request.UsuarioId,
                TipoSaude = "Bem-estar físico e emocional",
                NivelAlerta = nivelAlerta,
                MensagemSaude = mensagem,
                FonteRecomendacao = "Fallback estruturado do sistema"
            };
        }
    }
}