
# Synapse - API - Global Solution  
### DISRUPTIVE ARCHITECTURES: IOT, IOB & GENERATIVE IA

## 👩‍💻 Integrantes

- Giovanna Revito Roz - RM558981
- Kaian Gustavo de Oliveira Nascimento - RM558986
- Lucas Kenji Kikuchi - RM554424

---

## 🧠 Descrição do Projeto

O **Synapse** é uma plataforma inteligente que combina:

### 🔹 Orientação Profissional  
O usuário informa área atual, área de interesse, competências e objetivos.  
A API envia esses dados para o **Ollama (IA local)**, que responde com recomendações personalizadas de:

- Vagas potenciais  
- Cursos e trilhas de aprendizado  
- Áreas sugeridas  
- Próximos passos de carreira  

### 🔹 Bem-estar (Saúde Emocional & Rotina)  
O usuário registra diariamente informações como:  

- Horas de sono  
- Horas de trabalho  
- Humor  
- Nível de estresse  
- Energia  

A IA analisa os registros e gera:

- Alertas de saúde emocional  
- Sugestões de rotina  
- Hábitos saudáveis  
- Recomendações personalizadas  

O Synapse une **carreira + bem-estar** em um ambiente inteligente para apoiar o desenvolvimento pessoal e profissional.

---

## ⚙️ Instalação

### 📋 Pré-requisitos

- .NET 9 SDK  
- Ollama instalado  
- Visual Studio 2022+ ou JetBrains Rider  
- Git  

---

### 📥 Clone o repositório

```bash
git clone https://github.com/giovannarevitoroz/gs-iot.git
cd gs-iot
````

### 📦 Instale dependências

```bash
dotnet restore
```

### ▶️ Execute a aplicação

```bash
dotnet run
```

---

## 📚 Swagger

Após iniciar o projeto:

👉 Acesse a documentação da API:

```
http://localhost:5000/swagger
```

---

# 📡 **Rotas da API**

As três rotas principais são **POST**, pois enviam dados para a IA.

---

# 1️⃣ **Recomendação Profissional**

### 📍 **POST /api/recomendacao/profissional**

### ✔️ Exemplo de Request

```json
{
  "id": 1,
  "nomeUsuario": "Giovanna",
  "areaAtual": "Assistente Torre de Controle",
  "areaInteresse": "Desenvolvimento de Software",
  "objetivoCarreira": "Conseguir estágio em backend",
  "nivelExperiencia": "Júnior",
  "competencias": [
    {
      "nomeCompetencia": "Java",
      "categoriaCompetencia": "Programação",
      "descricaoCompetencia": "Conhecimento intermediário em Java."
    },
    {
      "nomeCompetencia": "SQL",
      "categoriaCompetencia": "Banco de Dados",
      "descricaoCompetencia": "Experiência criando consultas e tabelas."
    }
  ]
}
```

### ✔️ Exemplo de Response

```json
{
  "dataRecomendacao": "2025-11-13T20:45:57.669Z",
  "descricaoRecomendacao": "Recomendação gerada pela IA...",
  "promptUsado": "...",
  "tituloRecomendacao": "Recomendação de Vagas e Cursos",
  "usuarioId": 1,
  "categoriaRecomendacao": "Profissional",
  "areaRecomendacao": "Desenvolvimento de Software",
  "fonteRecomendacao": "Gerado pelo sistema"
}
```

---

# 2️⃣ **Recomendação de Saúde / Bem-estar**

### 📍 **POST /api/recomendacao/saude**

### ✔️ Exemplo de Request

```json
{
  "usuarioId": 1,
  "registrosBemEstar": [
    {
      "dataRegistro": "2025-11-20T08:00:00",
      "humorRegistro": "Feliz",
      "horasSono": 7,
      "horasTrabalho": 6,
      "nivelEnergia": 8,
      "nivelEstresse": 3,
      "observacaoRegistro": "Dia produtivo"
    },
    {
      "dataRegistro": "2025-11-21T08:00:00",
      "humorRegistro": "Cansada",
      "horasSono": 5,
      "horasTrabalho": 8,
      "nivelEnergia": 4,
      "nivelEstresse": 7,
      "observacaoRegistro": "Pouco sono e correria"
    }
  ]
}
```

### ✔️ Exemplo de Response

```json
{
  "dataRecomendacao": "2025-11-21T09:00:00",
  "descricaoRecomendacao": "Com base nos registros enviados...",
  "promptUsado": "...",
  "tituloRecomendacao": "Recomendação de Bem-Estar",
  "usuarioId": 1,
  "tipoSaude": "Bem-estar físico e emocional",
  "nivelAlerta": "Moderado",
  "mensagemSaude": "Sugestões personalizadas...",
  "fonteRecomendacao": "Gerado pelo sistema"
}
```

---

## 📘 Códigos de Resposta

| Código HTTP                   | Significado                     | Quando ocorre                                         |
| ----------------------------- | ------------------------------- | ----------------------------------------------------- |
| **200 OK**                    | Recomendação gerada com sucesso | IA retornou a resposta corretamente                   |
| **400 Bad Request**           | Erro nos dados enviados         | Request inválido                                      |
| **500 Internal Server Error** | Erro interno                    | Problema ao comunicar com o Ollama ou processar dados |

---
