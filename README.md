# Synapse - API - Global Solution  
### DISRUPTIVE ARCHITECTURES: IOT, IOB & GENERATIVE IA

Link vídeo:  https://youtu.be/mHp7ao4INg0

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
  "dataRecomendacao": "2025-11-23T08:49:51.615787-03:00",
  "descricaoRecomendacao": "Recomendação padrão para o usuário ID 1.",
  "promptUsado": "\r\nGere uma recomendação de carreira detalhada para o usuário com ID 1.\r\nConsidere a área de interesse: Desenvolvimento de Software.\r\nNão inclua informações pessoais.\r\nSe disponível, inclua habilidades ou competências do usuário.",
  "tituloRecomendacao": "Recomendação de Vagas e Cursos",
  "usuarioId": 1,
  "categoriaRecomendacao": "Profissional",
  "areaRecomendacao": "Desenvolvimento de Software",
  "fonteRecomendacao": "Fallback do sistema"
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
  "usuarioId": 1,
  "dataRecomendacao": "2025-11-23T08:50:54.4112517-03:00",
  "descricaoRecomendacao": "Recomendação padrão para o usuário ID 1.",
  "promptUsado": "\r\nGere uma recomendação de saúde detalhada considerando os seguintes registros de bem-estar:\r\n- Data: 2025-11-20, Humor: Feliz, Sono: 7h, Trabalho: 6h, Energia: 8, Estresse: 3, Observação: Dia produtivo\n- Data: 2025-11-21, Humor: Cansada, Sono: 5h, Trabalho: 8h, Energia: 4, Estresse: 7, Observação: Pouco sono e correria\r\n\r\nInclua sugestões de bem-estar físico e emocional, hábitos saudáveis e rotina de exercícios.\r\nNão inclua informações pessoais do usuário.",
  "tituloRecomendacao": "Recomendação de Bem-Estar",
  "tipoSaude": "Bem-estar físico e emocional",
  "nivelAlerta": "Moderado",
  "mensagemSaude": "Mantenha hábitos saudáveis e pratique exercícios regularmente.",
  "fonteRecomendacao": "Fallback do sistema",
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

---

## 📘 Códigos de Resposta

| Código HTTP                   | Significado                     | Quando ocorre                                         |
| ----------------------------- | ------------------------------- | ----------------------------------------------------- |
| **200 OK**                    | Recomendação gerada com sucesso | IA retornou a resposta corretamente                   |
| **400 Bad Request**           | Erro nos dados enviados         | Request inválido                                      |
| **500 Internal Server Error** | Erro interno                    | Problema ao comunicar com o Ollama ou processar dados |

---
