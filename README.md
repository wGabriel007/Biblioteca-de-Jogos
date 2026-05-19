# 🎮 Biblioteca de Jogos

Sistema web para gerenciamento de uma biblioteca pessoal de jogos e periféricos, permitindo cadastro, empréstimo, venda e controle entre usuários.

---

## 🚀 Tecnologias

- [.NET 8](https://dotnet.microsoft.com/)
- [ASP.NET Core MVC](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [PostgreSQL](https://www.postgresql.org/)
- [Npgsql](https://www.npgsql.org/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) — hash de senhas

---

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

---

## 🔧 Configuração

1. **Clone o repositório**

2. **Execute a aplicação**

O sistema criará automaticamente o usuário **admin** na primeira execução.

> Login padrão: `admin@admin.com` / senha definida no seed de `Program.cs`

---

## 📦 Funcionalidades

- [x] Cadastro e autenticação de usuários
- [x] Recuperação de senha por e-mail
- [x] Edição de perfil de usuário
- [x] Cadastro de jogos com foto, gênero, console e horas para zerar
- [x] Data de adição automática nos jogos
- [x] Controle de empréstimos de jogos entre usuários
- [x] Solicitações de empréstimo com status de aprovação/rejeição
- [x] Exibição da data de empréstimo nos cards
- [x] Avaliação de jogos com estrelas e comentário
- [x] Cadastro de periféricos com foto, estado e disponibilidade
- [x] Solicitação de empréstimo e venda de periféricos
- [x] Notificações unificadas de jogos e periféricos
- [x] Badge de notificações não visualizadas
- [x] Filtros por nome, console, gênero, disponibilidade e usuário
- [x] Abas separadas: Meus Jogos, Comunidade e Periféricos
- [x] Contadores de jogos por aba
- [x] Controle de permissões (dono do item ou admin)
- [x] Painel administrativo com badge de admin

---

## 🗄️ Estrutura do Banco de Dados

| Tabela                    | Descrição                                         |
|---------------------------|---------------------------------------------------|
| `Usuarios`                | Usuários do sistema                               |
| `Jogos`                   | Acervo de jogos cadastrados                       |
| `Consoles`                | Consoles disponíveis para seleção                 |
| `Solicitacoes`            | Solicitações de empréstimo de jogos               |
| `Perifericos`             | Periféricos cadastrados pelos usuários            |
| `SolicitacoesPeriferico`  | Solicitações de empréstimo e venda de periféricos |
| `Avaliacoes`              | Avaliações de jogos com estrelas e comentário     |
| `CodigosRecuperacao`      | Códigos para recuperação de senha                 |

---

## 👤 Autor

**Gabriel Silva** — [@wGabriel007](https://github.com/wGabriel007)

