# ApiBlog

Este projeto foi desenvolvido como parte do módulo "Fundamentos do ASP.NET 6" da formação do Balta.io. Trata-se de uma API para gerenciamento de um blog, permitindo operações CRUD em categorias, autenticação de usuários, upload e serviço de imagens, paginação de dados e outras funcionalidades relacionadas.

## Funcionalidades

- **Gerenciamento de Categorias**: Criação, leitura, atualização e exclusão de categorias do blog.
- **Autenticação e Autorização**: Implementação de autenticação JWT para segurança das rotas.
- **Upload e Serviço de Imagens**: Upload de imagens e disponibilização para exibição no blog.
- **Cache em Memória**: Utilização de caching para otimizar o desempenho em operações de leitura.
- **Envio de Emails**: Serviço para envio de emails através de configuração SMTP.
- **Documentação com Swagger**: Integração do Swagger para documentação e testes dos endpoints da API.
- **Paginação de Dados**: Implementação de paginação para otimizar a recuperação de grandes volumes de registros.

## Tecnologias Utilizadas

- [ASP.NET 6](https://docs.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0): Framework principal para construção da API.
- [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/): ORM para manipulação do banco de dados.
- [JWT (JSON Web Tokens)](https://jwt.io/): Para autenticação e autorização.
- [Swagger](https://swagger.io/): Para documentação interativa da API.
- [In-Memory Caching](https://docs.microsoft.com/pt-br/aspnet/core/performance/caching/memory?view=aspnetcore-6.0): Para cache em memória.
- [SMTP](https://pt.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol): Protocolo para envio de emails.
- **Upload de Arquivos**: Implementado para permitir o upload e exibição de imagens associadas aos posts do blog.
- **Paginação com EF Core**: Implementação para otimizar a recuperação de registros.

## Pré-requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) para o banco de dados.
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/downloads/) ou [Visual Studio Code](https://code.visualstudio.com/) como ambiente de desenvolvimento.

## Configuração do Projeto

1. **Clone o repositório**:
    ```bash
    git clone https://github.com/ronaldo-dsantos/Blog.git
    ```
2. **Navegue até o diretório do projeto**:
    ```bash
    cd Blog
    ```
3. **Configure o arquivo `appsettings.json`**:
    - Defina a string de conexão com o banco de dados.
    - Configure a chave JWT (`JwtKey`).
    - Configure as credenciais SMTP para envio de emails.

4. **Execute as migrações do banco de dados**:
    ```bash
    dotnet ef database update
    ```

5. **Inicie a aplicação**:
    ```bash
    dotnet run
    ```

## Uso

Após iniciar a aplicação, você pode acessar a documentação interativa da API através do Swagger:

```bash
https://localhost:{porta}/swagger
```
Substitua `{porta}` pela porta configurada (geralmente 5001 para HTTPS).

## Licença

Este projeto está licenciado sob a licença MIT. Consulte o arquivo LICENSE para mais informações.