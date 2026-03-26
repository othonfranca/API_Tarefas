using TaskManagerApi.Enums;

namespace TaskManagerApi.Models;

public class Tarefa
{
    public int Id {get;set;}
    public DateTime DataCriacao {get;set;} = DateTime.Now;
    public string Titulo {get;set;} = string.Empty;
    public string Descricao {get;set;} = string.Empty;
    public StatusTarefa Status {get;set;} = StatusTarefa.Pendente;
    public int ColaboradorId {get;set;} // chave estrangeira para Colaborador
    public Colaborador? Colaborador {get;set;} // propriedade de navegação para Colaborador
    public DateTime? DataAtualizacao { get; set; } // O '?' permite nulo no banco
}