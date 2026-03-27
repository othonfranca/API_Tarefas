using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Models;

namespace TaskManagerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    public DbSet<Colaborador> Colaboradores { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Lógica para, se deletar um colaborador, deletar tbm as tarefas vinculadas a ele automaticamente
        modelBuilder.Entity<Tarefa>()
            .HasOne(t => t.Colaborador)
            .WithMany(c => c.Tarefas)
            .HasForeignKey(t => t.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Colaborador>().HasData(
            new Colaborador { Id = 1, Nome = "Pam", Email = "pam@example.com", Senha = "senha123" },
            new Colaborador { Id = 2, Nome = "Jim", Email = "jim@example.com", Senha = "senha456" },
            new Colaborador { Id = 3, Nome = "Kevin", Email = "kevin@example.com", Senha = "senha789" },
            new Colaborador { Id = 4, Nome = "Kelly", Email = "kelly@example.com", Senha = "senha012" },
            new Colaborador { Id = 5, Nome = "Dwight", Email = "dwight@example.com", Senha = "senha345" },
            new Colaborador { Id = 6, Nome = "Michael", Email = "michael@example.com", Senha = "senha678" },
            new Colaborador { Id = 7, Nome = "Angela", Email = "angela@example.com", Senha = "senha901" },
            new Colaborador { Id = 8, Nome = "Stanley", Email = "stanley@example.com", Senha = "senha234" },
            new Colaborador { Id = 9, Nome = "Oscar", Email = "oscar@example.com", Senha = "senha567" },
            new Colaborador { Id = 10, Nome = "Phyllis", Email = "phyllis@example.com", Senha = "senha890" }
        );

        modelBuilder.Entity<Tarefa>().HasData(
            // Exemplo com uma data fixa para o pessoal da Dunder Mifflin
            new Tarefa { Id = 1, Titulo = "Alimentar o cachorro", DataCriacao = new DateTime(2026, 3, 25), Descricao = "Descrição da tarefa 1",Status = Enums.StatusTarefa.Pendente, ColaboradorId = 1 },
            new Tarefa { Id = 2, Titulo = "Fazer compras", DataCriacao = new DateTime(2026, 3, 26), Descricao = "Descrição da tarefa 2", Status = Enums.StatusTarefa.EmAndamento, ColaboradorId = 2 },
            new Tarefa { Id = 3, Titulo = "Ir à padaria", DataCriacao = new DateTime(2026, 3, 27), Descricao = "Descrição da tarefa 3", Status = Enums.StatusTarefa.Concluido, ColaboradorId = 3 },
            new Tarefa { Id = 4, Titulo = "Ir à feira", DataCriacao = new DateTime(2026, 3, 28), Descricao = "Descrição da tarefa 4", Status = Enums.StatusTarefa.Pendente, ColaboradorId = 4 },
            new Tarefa { Id = 5, Titulo = "Comprar itens de festa", DataCriacao = new DateTime(2026, 3, 29), Descricao = "Descrição da tarefa 5", Status = Enums.StatusTarefa.EmAndamento, ColaboradorId = 5 },
            new Tarefa { Id = 6, Titulo = "Contratar palhaço", DataCriacao = new DateTime(2026, 3, 30), Descricao = "Descrição da tarefa 6", Status = Enums.StatusTarefa.Concluido, ColaboradorId = 6 },
            new Tarefa { Id = 7, Titulo = "Limpar o escritório", DataCriacao = new DateTime(2026, 3, 31), Descricao = "Descrição da tarefa 7", Status = Enums.StatusTarefa.Pendente, ColaboradorId = 7 },
            new Tarefa { Id = 8, Titulo = "Preparar a festa", DataCriacao = new DateTime(2026, 4, 1), Descricao = "Descrição da tarefa 8", Status = Enums.StatusTarefa.EmAndamento, ColaboradorId = 8 },
            new Tarefa { Id = 9, Titulo = "Preparar comida", DataCriacao = new DateTime(2026, 4, 2), Descricao = "Descrição da tarefa 9", Status = Enums.StatusTarefa.Concluido, ColaboradorId = 1 },
            new Tarefa { Id = 10, Titulo = "Preparar decoração", DataCriacao = new DateTime(2026, 4, 3), Descricao = "Descrição da tarefa 10", Status = Enums.StatusTarefa.Pendente, ColaboradorId = 2 },
            new Tarefa { Id = 11, Titulo = "Comer", DataCriacao = new DateTime(2026, 4, 4), Descricao = "Descrição da tarefa 11", Status = Enums.StatusTarefa.EmAndamento, ColaboradorId = 3},
            new Tarefa { Id = 12, Titulo = "Fofocar", DataCriacao = new DateTime(2026, 4, 5), Descricao = "Descrição da tarefa 12", Status = Enums.StatusTarefa.Concluido, ColaboradorId = 4 }
        );
    }
}