
# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

Tecnologias Backend utilizadas:

- C#
- Asp.Net Core
- MySQL
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- dotnet Core SDK (https://www.microsoft.com/net/download/windows dotnet Core 6.0.201 SDK)
- Um editor de código, acoselhamos o Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs v17.6.0 para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySQL (vc pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que lhe oferece o banco Mysql adicionamente)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript basta executar:

`npm install`
`npm run build`

Para Rodar o projeto:

Execute a migration no banco mysql:

`dotnet tool update --global dotnet-ef`
`dotnet tool ef database update`

E após: 

`dotnet run` (ou clique em "play" no editor do vscode)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor (por exemplo, um domínio sem extensão).
 - Está sendo utilizado "vanilla-js" para fazer a requisição para o backend, apesar de já estar configurado o webpack. O ideal seria utilizar algum framework mais moderno como ReactJs ou Blazor.  

# BackEnd

 - Não há validação no backend permitindo que uma requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel (DTO) neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

# Dica

- Este teste não tem "pegadinha", é algo pensado para ser simples. Aconselhamos a ler o código, e inclusive algumas dicas textuais deixadas nos testes unitários. 
- Há um teste unitário que está comentado, que obrigatoriamente tem que passar.
- Diferencial: criar mais testes.

# Entrega

- Enviei o link do seu repositório com o código atualizado.
- O repositório deve estar público para que possamos acessar..
- Modifique Este readme adicionando informações sobre os motivos das mudanças realizadas.

# Modificações:

Durante o desenvolvimento e refatoração do desafio, foram realizadas as seguintes melhorias:

## 1° BackEnd
<img width="1209" height="503" alt="Image" src="https://github.com/user-attachments/assets/c85b3a79-a006-4b78-8f57-2cf43a78171f" />

1. **Migrations**
    - Criação de novas migrations, alinhada com um banco de dados local MySQL e refatoração na connectionstring.
2. **Atualização da pasta Models e impacto**

#### O que foi feito:

- Atualizado o model Domain com valores padrão e [Required] nas propriedades essenciais (Name, Ip, UpdatedAt, Ttl).

- Mantida a chave primária Id e o DbSet<Domain> no DatabaseContext.

#### Impacto:

- Garante integridade e consistência dos dados no banco.

- Facilita testes unitários com dados padrão.

- Permite que DomainService e DomainController retornem informações completas sem erros de valores nulos.
3. **Refatoração do Service (`DomainService`)**
    - Criado **wrapper para o `WhoisClient`** e injeção de `ILookupClient` para permitir mock durante testes unitários.
    - Alterado para retornar objetos do tipo `Domain`, mantendo a lógica de persistência e consulta ao banco de dados.
    - Inclusão de **tratamento de TTL** e atualização de registros antigos no banco de dados.
4. **Criação de DTO (`DomainResultDto`)**
    - Separação entre a entidade do banco (`Domain`) e os dados retornados para o front-end.
    - Melhora na segurança, evitando expor dados desnecessários.
    - Facilita a manipulação dos dados no frontend (Vite + React).
5. **Refatoração do Controller (`DomainController`)**
    - Controller agora recebe `IDomainService` via injeção de dependência.
    - Map para `DomainResultDto` realizado no controller.
    - Permite fácil mockagem e testes unitários.
    
## 2° Frontend
<img width="1085" height="512" alt="Image" src="https://github.com/user-attachments/assets/b8079ddb-dc23-498e-bd27-24cc385c8d1c" />

> OBS: Optei por criar o front usando vite + ReactJS em uma pasta diferente, seguindo as recomendações da documentação do ASP.NET. Caso não seja comum para a equipe usar build tools como o vite, também é possível buildar o frontend dentro da pasta Views. Ajustes como esse são de rápida implementação.
1. Substituição  de vanilla JS por **React + TypeScript**.
    
2. Consulta à API via service dedicado (`domainService.ts`).
    
3. Validação de entrada do usuário (domínios inválidos não são enviados).
    
4. Dados retornados pelo backend são 
exibidos de forma legível no frontend.

> OBS: A escolha do pnpm como gerenciador de pacotes no front foi meramente perfomática, por estar lidando com uma máquina com recursos limitados.
    
## 3° Testes Unitários
<img width="1115" height="284" alt="Image" src="https://github.com/user-attachments/assets/ea315eac-32e8-4bbd-8ca5-821f71156c9d" />
    
> OBS: O testa unitário que estava comentado, não estava funcionando naquele formato. Ele dependia do WhoisClient original, que é uma classe estática e não mockável diretamente.

_Para funcionar, foi preciso:_

- Criar um wrapper/método mockável para o Whois (IWhoisClientWrapper).
- Alterar o DomainService e o DomainController para usar a interface mockável em vez da classe estática.
- Criar o teste usando Mock<IWhoisClientWrapper> retornando um valor fake para o domínio.

Depois dessa refatoração, esse teste passou a funcionar, mas não é mais aquele código comentado, e sim uma versão adaptada usando o service mockado.
    
**Porém mesmo adaptado, ele exerce exatamente a mesma função que o teste original comentado**
    
---    
 - Configuração de **InMemoryDatabase** do Entity Framework para testar interações com o banco sem depender de um banco real.
- Criação de mocks para `IDomainService` e wrappers de Whois/DNS.
    - Cobertura aumentada para cenários:
        - Domínio existente no banco.
        - Domínio inexistente no banco.
        - Domínio inválido.
    - Todos os testes unitários obrigatórios passam, garantindo estabilidade da aplicação.

 ## 5° Extra 
- Criação de um teste unitário específico para domínio inválido, simulando a requisição de um domínio que não existe ou não é válido.
- Habilitação de **CORS** e configuração de `Swagger` para facilitar testes e documentação da API.
- Atualização de pacotes NuGet para resolver conflitos de versões do Entity Framework.
- Ajustes para compatibilidade com **.NET 6**.

---

## Como Rodar

### Backend

1. Configure a ConnectionString no `appsettings.json`.
2. Execute `dotnet build`.
3. Atualize as Migrations `dotnet ef database update`
4. Execute `dotnet run` para iniciar o servidor.

### Frontend

1. Entre na pasta `client`.
2. Execute `pnpm i`.
3. Execute `pnpm dev` para iniciar o frontend.
4. Acesse [http://localhost:5173](http://localhost:5173/) (ou porta indicada pelo Vite).

### Testes

- Execute `dotnet test` na pasta `Desafio.Umbler.Test` para rodar todos os testes unitários.

---
fixed :)
