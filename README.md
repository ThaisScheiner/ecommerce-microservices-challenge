# Desafio de Microserviços: E-commerce de Gestão de Estoque e Vendas

## Descrição do Projeto

Este projeto é uma aplicação back-end desenvolvida com uma arquitetura de microserviços para simular o gerenciamento de estoque e vendas de uma plataforma de e-commerce. A solução foi projetada para ser escalável, robusta e segura, utilizando boas práticas de desenvolvimento e comunicação entre serviços.

O sistema é composto por quatro serviços independentes:
* **Serviço de Autenticação (`Auth.API`):** Responsável por validar credenciais e gerar tokens de acesso JWT.
* **Serviço de Estoque (`Stock.API`):** Gerencia o cadastro de produtos e controla a quantidade disponível em estoque.
* **Serviço de Vendas (`Sales.API`):** Gerencia os pedidos de venda, interagindo com o serviço de estoque para validação.
* **API Gateway:** Atua como um ponto de entrada único (Single Point of Entry) para todas as requisições, roteando-as para o microserviço apropriado.

A comunicação entre os serviços de Vendas e Estoque é realizada de forma assíncrona utilizando **RabbitMQ** para garantir a resiliência e o desacoplamento das operações de atualização de estoque.

## Arquitetura Proposta

A solução segue o padrão de arquitetura de microserviços com um API Gateway como fachada. A comunicação síncrona (validação de estoque) é feita via HTTP, enquanto a comunicação assíncrona (confirmação de venda e baixa de estoque) é feita via mensageria.