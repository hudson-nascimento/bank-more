# BankMore

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-ready-blue)
![License](https://img.shields.io/badge/license-MIT-green)

API para operações bancárias simples do Bank More, como **conta corrente**
e **transferências**, construída com **.NET 8** e executada com suporte
a **Docker**.

O objetivo do projeto é demonstrar uma arquitetura baseada em
**microserviços**, separando responsabilidades entre serviços
independentes.

------------------------------------------------------------------------

# Arquitetura

O sistema é dividido em dois serviços principais:

-   **ContaCorrente** --- gerenciamento de contas
-   **Transferencia** --- processamento de transferências entre contas

 
``` mermaid
flowchart LR

Client --> ContaCorrenteAPI
Client --> TransferenciaAPI

TransferenciaAPI --> ContaCorrenteAPI
```

Cada serviço pode ser executado de forma independente.

------------------------------------------------------------------------

# Tecnologias utilizadas

-   .NET 8
-   ASP.NET Core Web API
-   Docker / Docker Compose
-   Swagger / OpenAPI

------------------------------------------------------------------------

# Como executar o projeto

## Pré-requisitos

Antes de iniciar, certifique-se de possuir:

-   .NET SDK 8.x
-   Docker, Podman ou Rancher
-   Sistema operacional:
    -   Linux
    -   macOS
    -   Windows 64-bit

------------------------------------------------------------------------

# 1. Clonar o repositório

``` bash
git clone https://github.com/hudson-nascimento/bank-more.git
```

------------------------------------------------------------------------

# 2. Acessar o diretório do projeto

``` bash
cd BankMore
```

------------------------------------------------------------------------

# 3. Subir a infraestrutura com Docker

O projeto possui um **docker compose** responsável por subir os serviços
necessários para execução local.

Para iniciar os contêineres:

``` bash
docker compose --file SolutionItems/compose.yaml up -d
```

Para parar os contêineres:

``` bash
docker compose --file SolutionItems/compose.yaml down
```

------------------------------------------------------------------------

# 4. Executar as aplicações

Após subir a infraestrutura, execute os serviços da API.

### Conta Corrente

``` bash
dotnet run --project src/ContaCorrente/BankMore.Contacorrente.API.csproj
```

### Transferência

``` bash
dotnet run --project src/Transferencia/BankMore.Transferencia.API.csproj
```

Você pode executar em **terminais separados** ou em segundo plano:

``` bash
dotnet run --project src/ContaCorrente/API.csproj &
dotnet run --project src/Transferencia/API.csproj
```

------------------------------------------------------------------------

# Documentação da API

A documentação interativa está disponível via **Swagger**.

Após iniciar as aplicações, acesse:

    http://localhost:5000/swagger

ou

    https://localhost:5001/swagger

------------------------------------------------------------------------

# Melhorias futuras

-   Autenticação com JWT
-   Testes automatizados
-   Observabilidade (OpenTelemetry)
-   Mensageria para transferências
-   CI/CD

------------------------------------------------------------------------

# Licença

Este projeto está disponível sob a licença **MIT**.
