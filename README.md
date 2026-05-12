# 🎮 Biblioteca de Jogos

Sistema web para gerenciamento de uma biblioteca pessoal de jogos, permitindo
cadastro, empréstimo e controle de jogos entre usuários.

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

> Loginpadrão: `admin@admin.com` / senha definida no seed de `Program.cs`

---

## 📦 Funcionalidades

- [x] Cadastro e autenticação de usuários
- [x] Recuperação de senha por e-mail
- [x] Cadastro de jogos com foto, gênero, console e horas para zerar
- [x] Controle de empréstimos entre usuários
- [x] Solicitações de empréstimo com status de aprovação
- [x] Notificações de solicitações não visualizadas

---

## 🗄️ Estrutura do Banco de Dados

| Tabela               | Descrição                              |
|----------------------|----------------------------------------|
| `Usuarios`           | Usuários do sistema                    |
| `Jogos`              | Acervo de jogos cadastrados            |
| `Consoles`           | Consoles disponíveis para seleção      |
| `Solicitacoes`       | Solicitações de empréstimo             |
| `CodigosRecuperacao` | Códigos para recuperação de senha      |

---

## 👤 Autor

**Gabriel Silva** — [@wGabriel007](https://github.com/wGabriel007)

