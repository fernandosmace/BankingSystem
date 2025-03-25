# Gerenciamento de Contas Bancárias API

## Visão Geral

Este projeto é uma API RESTful para gerenciamento de contas bancarias, onde o sistema oferece as funcionalidades de criação de contas, desativação, transferencia de valores entre contas e consulta de contas cadastradas.

## Funcionalidades da API

- **GET** `/Accounts/filter?Name={Name}&Document={Document}`: Obtém a lista de contas cadastradas com base no nome do Cliente e no Documento vinculado.
- **POST** `/Accounts`: Cria uma nova Conta informando o nome do Cliente (parcial ou completo) ou o Documento vinculado.
- - O Nome do cliente e o Documento são campos obrigatórios para a criação da conta.
- - Não é possível criar mais de uma conta com o mesmo documento.
- - Ao ser criada, a conta possui o saldo inicial de R$ 1.000,00.
- - A data de criação da conta é registrada automaticamente.
- **POST** `/Accounts/transfer`: Realiza a transferência de valores de uma conta para outra através do ID da conta de origem, o ID da conta de destino e o valor desejado à ser transferido.
- - Não é possível transferir valores de uma conta de origem que não possui no mínimo o valor solicitado na transferência.
- - A conta de origem e a conta de destino devem estar ativas para realizar a transferência.
- **POST** `/Accounts/Deactivate`: Realiza a desativação de uma conta recebendo o Documento como parametro e registra a ação no histórico.


## Preparação e Execução Local

### Pré-requisitos

- [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker e Docker Compose (opcional, para execução em container)

### Download do Repositório 

Realize o download do repositorio via clone utilizando o comando:
   ```bash
   git clone https://github.com/fernandosmace/BankingSystem.git
   cd BankingSystem
   ```

### Formas de Executar a Aplicação

#### 1. Via Docker Compose
Na própria pasta do projeto, execute:
```bash
docker-compose up
```
Isso iniciará automaticamente de forma local os containers SQL Server e API, a qual estará acessível na porta `5000` sem necessidade de nenhuma configuração adicional al

#### 2. Via Linha de Comando
1. Edite o arquivo `appsettings.json` para inserir as credenciais do banco de dados.
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Para executar a API:
   ```bash
   dotnet run
   ```

#### 3. Via Visual Studio
1. Edite as variáveis de ambiente no perfil HTTPS no `launchSettings.json` do projeto da API para inserir as credenciais do banco de dados.
2. Execute o projeto pelo Visual Studio, selecionando o perfil HTTPD e clicando no botão "Start" da IDE.

> *Observação: A aplicação aplica automaticamente as migrações do Entity Framework durante a inicialização, portanto, a execução do comando `dotnet ef database update` não é necessária.*

## Testes Unitários 
Para executar os testes automatizados realize a execucao do comando abaixo via terminal ou utilize o Visual Studio:
```bash
dotnet test
```
