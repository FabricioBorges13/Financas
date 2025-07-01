
# 💰 Finanças - Sistema de Gerenciamento Financeiro

Ele simula operações bancárias como abertura de contas, transações financeiras (vendas, estornos, transferências), controle de saldo, resiliência, idempotência, auditoria, logs e mais.

## 📘 Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Rodando o Projeto com Docker](#rodando-o-projeto-com-docker)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Funcionalidades Implementadas](#funcionalidades-implementadas)
- [Testes](#testes)
- [Autor](#autor)

---

## 📝 Sobre o Projeto

Este projeto tem como objetivo simular um sistema financeiro completo, com foco em:

- Contas bancárias com diferentes tipos (Pessoa Física/Jurídica)
- Operações financeiras com suporte a:
  - Venda à vista, parcelada e débito
  - Transferência entre contas
  - Estorno de transações
- Suporte à concorrência e idempotência
- Auditoria e rastreabilidade
- Resiliência via Polly
- Lock distribuído com Redis
- Transações consistentes com Entity Framework
- **Logs estruturados com Serilog (arquivo e console)**

---

## 🚀 Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core
- SQL Server 2022
- Redis
- Docker & Docker Compose
- Polly (Resiliência)
- Serilog (Logs)
- Moq + xUnit (Testes)
- Clean Architecture

---

## 🐳 Rodando o Projeto com Docker

### ✅ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://docs.docker.com/get-docker/)

### ▶️ Passos para executar:

1. **Clone o repositório:**

```bash
git clone https://github.com/FabricioBorges13/Financas.git
cd Financas
```

2. **Suba os containers:**

```bash
docker-compose -f docker/docker-compose.yml up --build
```

**Para recriar o container, o volume do banco de dados deve ser removido:**

```bash
docker-compose down
docker volume ls
docker volume rm nome_do_volume
```

3. **Acesse o Swagger da API:**

```
http://localhost:5000/swagger
```

> ✅ O Swagger estará disponível na porta `5000`.  
> Inclui seed inicial com dois clientes e suas respectivas contas.

> Os testes podem ser realizados diretamente pelo Swagger.

---

## 📁 Estrutura do Projeto

```text
Financas/
├── docker/                      # Arquivos Docker e docker-compose
├── src/
│   ├── Financas.Api/            # Camada de apresentação (Controllers / Swagger)
│   ├── Financas.Application/    # Casos de uso, interfaces e DTOs
│   ├── Financas.Domain/         # Entidades e lógica de domínio
│   ├── Financas.Infrastructure/ # Infra (banco, Redis, logs, repositórios)
├── Financas.Tests/              # Testes unitários
├── Financas.sln                 # Solução .NET
```

---

## ⚙️ Funcionalidades Implementadas

- [x] CRUD de contas
- [x] Registro de vendas (à vista, parcelado, débito)
- [x] Transferência entre contas
- [x] Estorno de transações
- [x] Controle de saldo disponível, bloqueado e futuro
- [x] Auditoria detalhada das transações
- [x] Resiliência e fallback com Polly
- [x] Idempotência com Redis
- [x] Lock distribuído por chave de recurso (concorrência)
- [x] **Logs estruturados com Serilog (JSON)**
  - Escrita no console e em arquivo (log/app.log)
  

---

## 🧪 Testes

Para rodar os testes unitários:

```bash
dotnet test
```

Cobertura:

- Entidades de domínio
- UseCases (mockando dependências como repositórios e serviços)
- Testes de concorrência e resiliência com Polly

---

## 👨‍💻 Autor

Desenvolvido por **Fabricio Borges**  
[https://github.com/FabricioBorges13](https://github.com/FabricioBorges13)
