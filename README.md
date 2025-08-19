# Desafio de Microserviços: Aplicação E-commerce Completa (Full Stack)

## Descrição do Projeto

Este projeto implementa uma aplicação **full stack** com uma arquitetura de microserviços para simular o gerenciamento de estoque e vendas de uma plataforma de e-commerce. A solução foi projetada para ser escalável, robusta e segura, utilizando boas práticas de desenvolvimento, comunicação entre serviços e uma interface de usuário moderna.

O sistema é composto por **cinco serviços independentes**:
* **Front-end (`Ecommerce.WebApp`):** Uma Single Page Application (SPA) construída com **Blazor WebAssembly** que consome o back-end, gerencia a autenticação do usuário no lado do cliente e fornece uma interface rica para interagir com o sistema.
* **API Gateway (`ApiGateway`):** Atua como um ponto de entrada único (Single Point of Entry) para o front-end, roteando as requisições para o microserviço apropriado.
* **Serviço de Autenticação (`Auth.API`):** Responsável por validar credenciais e gerar tokens de acesso JWT.
* **Serviço de Estoque (`Stock.API`):** Gerencia o cadastro de produtos e controla a quantidade disponível em estoque.
* **Serviço de Vendas (`Sales.API`):** Gerencia os pedidos de venda, interagindo com o serviço de estoque para validação.

A comunicação entre os serviços de Vendas e Estoque é realizada de forma **assíncrona** utilizando **RabbitMQ**. Todo o sistema de back-end é instrumentado com **Serilog** para logging estruturado, com os logs centralizados e visualizados na ferramenta **Seq**.

## Escolhas de Tecnologia

### Front-end: Blazor WebAssembly
A escolha pelo **Blazor WebAssembly** se deu por permitir a criação de uma Single Page Application (SPA) moderna e desacoplada, utilizando **C#** em vez de JavaScript. Isso permite o reaproveitamento de habilidades e modelos de dados do ecossistema .NET, além de oferecer uma integração nativa com a arquitetura de back-end existente.

### Logging: Serilog + Seq
Para o monitoramento, a dupla **Serilog + Seq** foi escolhida devido à sua leveza e facilidade de implementação em um ambiente de desenvolvimento, especialmente em máquinas com recursos limitados. O Serilog permite a criação de logs estruturados (em JSON), e o Seq oferece uma interface gráfica poderosa e de baixo consumo para centralizar, visualizar e filtrar esses logs em tempo real, o que é crucial para depurar uma arquitetura de microserviços.

## Tecnologias e Versões

* **Linguagem:** C# 12
* **Frameworks:** .NET 8.0 (LTS), ASP.NET Core, Blazor WebAssembly
* **Banco de Dados:** MySQL 8.0
* **Mensageria:** RabbitMQ 3
* **Logging e Monitoramento:** Serilog e Seq
* **API Gateway:** Ocelot
* **ORM:** Entity Framework Core 8
* **Autenticação:** JWT (JSON Web Tokens)
* **Infraestrutura:** Docker & Docker Compose

## Dependências (Pacotes NuGet)

* **Back-end:**
    * `Pomelo.EntityFrameworkCore.MySql`
    * `Microsoft.EntityFrameworkCore.Design`
    * `Serilog.AspNetCore`, `Serilog.Sinks.Seq`
    * `RabbitMQ.Client`
    * `Ocelot`
    * `Microsoft.AspNetCore.Authentication.JwtBearer`
* **Front-end (`Ecommerce.WebApp`):**
    * `Microsoft.AspNetCore.Components.WebAssembly`
    * `Microsoft.AspNetCore.Components.Authorization`
    * `Blazored.LocalStorage` (para salvar o token JWT no navegador)
    * `Blazored.Toast` (para notificações)
    * `System.IdentityModel.Tokens.Jwt` (para decodificar o token)

## Como Executar o Projeto Completo

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos para Execução
1.  Clone este repositório.
2.  Na raiz do projeto, inicie toda a infraestrutura com Docker:
    ```bash
    # Inicia o MySQL e RabbitMQ
    docker-compose up -d

    # Inicia o Seq em um container separado
    docker run --name seq -d -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_NOAUTHENTICATION=true -p 5341:80 datalust/seq:latest
    ```
3.  Aguarde um minuto e verifique se os 3 containers (`mysql_db`, `rabbitmq`, `seq`) estão rodando com `docker ps`.

4.  Aplique as migrations para criar os bancos de dados:
    ```bash
    # Terminal 1
    cd src/Stock.API
    dotnet ef database update

    # Terminal 2
    cd src/Sales.API
    dotnet ef database update
    ```
5.  Inicie os **5 serviços** da aplicação. Você precisará de **5 terminais abertos**:
    ```bash
    # Terminais para o Back-end
    cd src/Auth.API && dotnet run
    cd src/Stock.API && dotnet run
    cd src/Sales.API && dotnet run
    cd src/ApiGateway && dotnet run

    # Terminal para o Front-end
    cd src/Ecommerce.WebApp && dotnet run
    ```
6.  Acesse a aplicação no seu navegador (a URL do `Ecommerce.WebApp`, ex: `http://localhost:5069`).
7.  Acesse a interface de logs no seu navegador em `http://localhost:5341`.

## Como Testar a Aplicação

1.  Acesse a aplicação no navegador.
2.  Clique em **"Login"** e use as credenciais `admin` / `password`.
3.  Você será redirecionado e verá a saudação "Olá, admin!". Links para "Produtos" e "Realizar Venda" aparecerão no menu.
4.  Navegue para a página **"Produtos"**, cadastre um novo produto e veja a lista ser atualizada.
5.  Navegue para a página **"Realizar Venda"**, selecione um produto, a quantidade e confirme. Uma notificação "toast" de sucesso deve aparecer.
6.  Volte para a página **"Produtos"** e confirme que o estoque do item vendido diminuiu.

