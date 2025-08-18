# Desafio de Microserviços: E-commerce de Gestão de Estoque e Vendas

## Descrição do Projeto

Este projeto é uma aplicação back-end desenvolvida com uma arquitetura de microserviços para simular o gerenciamento de estoque e vendas de uma plataforma de e-commerce. A solução foi projetada para ser escalável, robusta e segura, utilizando boas práticas de desenvolvimento e comunicação entre serviços.

O sistema é composto por quatro serviços independentes:
* **Serviço de Autenticação (`Auth.API`):** Responsável por validar credenciais e gerar tokens de acesso JWT.
* **Serviço de Estoque (`Stock.API`):** Gerencia o cadastro de produtos e controla a quantidade disponível em estoque.
* **Serviço de Vendas (`Sales.API`):** Gerencia os pedidos de venda, interagindo com o serviço de estoque para validação.
* **API Gateway:** Atua como um ponto de entrada único (Single Point of Entry) para todas as requisições, roteando-as para o microserviço apropriado.

A comunicação entre os serviços de Vendas e Estoque é realizada de forma assíncrona utilizando **RabbitMQ** para garantir a resiliência e o desacoplamento das operações de atualização de estoque. Todo o sistema é instrumentado com **Serilog** para logging estruturado, com os logs centralizados e visualizados na ferramenta **Seq**.

## Arquitetura Proposta

A solução segue o padrão de arquitetura de microserviços com um API Gateway como fachada. A comunicação síncrona (validação de estoque) é feita via HTTP, enquanto a comunicação assíncrona (confirmação de venda e baixa de estoque) é feita via mensageria.

## Tecnologias e Versões

* **Linguagem:** C# 12
* **Framework:** .NET 8.0 (LTS)
* **Arquitetura:** ASP.NET Core Web API
* **Banco de Dados:** MySQL 8.0
* **Mensageria:** RabbitMQ 3
* **Logging e Monitoramento:** Serilog e Seq
* **API Gateway:** Ocelot
* **ORM:** Entity Framework Core 8
* **Autenticação:** JWT (JSON Web Tokens)
* **Infraestrutura:** Docker & Docker Compose

## Dependências (Pacotes NuGet)

* `Pomelo.EntityFrameworkCore.MySql`: Provedor do Entity Framework Core para comunicação com o banco de dados MySQL.
* `Microsoft.EntityFrameworkCore.Design`: Ferramentas de design do EF Core para a criação de migrations.
* `Serilog.AspNetCore`, `Serilog.Sinks.Seq`, `Serilog.Sinks.File`: Para logging estruturado.
* `RabbitMQ.Client`: Cliente oficial .NET para comunicação com o RabbitMQ.
* `Ocelot`: Framework para implementação do API Gateway.
* `Microsoft.AspNetCore.Authentication.JwtBearer`: Middleware para validação de tokens JWT.
* `Swashbuckle.AspNetCore`: Geração automática de documentação da API (Swagger/OpenAPI).

## Como Executar o Projeto

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos para Execução
1.  Clone este repositório:
    ```bash
    git clone [https://github.com/seu-usuario/seu-repositorio.git](https://github.com/seu-usuario/seu-repositorio.git)
    cd seu-repositorio
    ```
2.  Inicie a infraestrutura (MySQL, RabbitMQ e Seq) com o Docker. Abra um terminal na raiz do projeto e execute os dois comandos a seguir:
    ```bash
    # Inicia o MySQL e RabbitMQ
    docker-compose up -d

    # Inicia o Seq em um container separado
    docker run --name seq -d -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
    ```
3.  Aguarde cerca de 1 minuto para os containers iniciarem. Você pode verificar se estão rodando com `docker ps`.

4.  Na primeira execução, é necessário criar os bancos de dados e aplicar as migrations. Abra dois terminais separados:
    ```bash
    # Terminal 1
    cd src/Stock.API
    dotnet ef database update

    # Terminal 2
    cd src/Sales.API
    dotnet ef database update
    ```
5.  Inicie os 4 microserviços. Você precisará de **4 terminais abertos** simultaneamente:
    ```bash
    # Terminal 1
    cd src/Auth.API
    dotnet run

    # Terminal 2
    cd src/Stock.API
    dotnet run

    # Terminal 3
    cd src/Sales.API
    dotnet run

    # Terminal 4
    cd src/ApiGateway
    dotnet run
    ```
6.  Para visualizar os logs em tempo real, acesse `http://localhost:5341` no seu navegador.

## Como Testar a API (Fluxo Básico)

Use uma ferramenta como Postman ou Insomnia. Todas as requisições devem ser feitas para o API Gateway (por padrão, rodando em `http://localhost:5116` ou outra porta informada no log).

1.  **Obter Token:** Faça um `POST` para `/gateway/login` com o corpo:
    ```json
    {
        "username": "admin",
        "password": "password"
    }
    ```
    Copie o token JWT da resposta.

2.  **Criar Produto:** Faça um `POST` para `/gateway/products`.
    * **Header:** `Authorization: Bearer <seu_token_jwt>`
    * **Body:**
        ```json
        {
          "name": "Produto de Teste",
          "description": "Descrição do produto",
          "price": 19.99,
          "quantityInStock": 50
        }
        ```

3.  **Realizar Venda:** Faça um `POST` para `/gateway/orders`.
    * **Header:** `Authorization: Bearer <seu_token_jwt>`
    * **Body:**
        ```json
        {
          "productId": 1,
          "quantity": 5
        }
        ```
4.  **Verificar Estoque:** Faça um `GET` para `/gateway/products/1`.
    * **Header:** `Authorization: Bearer <seu_token_jwt>`
    * Verifique se o `quantityInStock` foi atualizado para `45`.