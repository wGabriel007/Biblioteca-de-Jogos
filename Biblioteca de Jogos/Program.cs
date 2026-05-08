using Biblioteca_de_Jogos.Data;
using Biblioteca_de_Jogos.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Variáveis de ambiente DB
var host = Environment.GetEnvironmentVariable("DB_HOST");
var db   = Environment.GetEnvironmentVariable("DB_NAME");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";


// Variáveis de ambiente SMTP
builder.Configuration["Email:Remetente"] = Environment.GetEnvironmentVariable("EMAIL_REMETENTE");
builder.Configuration["Email:SenhaApp"] = Environment.GetEnvironmentVariable("EMAIL_SENHA_APP");
builder.Configuration["Email:SmtpHost"] = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST");
builder.Configuration["Email:SmtpPort"] = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT");

// registro de AddDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSession();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddScoped<Biblioteca_de_Jogos.Services.IEmailService,
                            Biblioteca_de_Jogos.Services.EmailService>();

var app = builder.Build();

// HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Loguin}/{id?}");
    app.MapDefaultControllerRoute();

// Seed automático do admin ao iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var adminJaExiste = await context.Usuarios
        .AnyAsync(u => u.str_Nome == "admin");

    if (!adminJaExiste)
    {
        context.Usuarios.Add(new Usuario
        {
            str_Nome    = "admin",
            str_Senha   = BCrypt.Net.BCrypt.HashPassword("admin123"),
            bool_Admin = true
        });

        await context.SaveChangesAsync();

    }
}

app.Run();
