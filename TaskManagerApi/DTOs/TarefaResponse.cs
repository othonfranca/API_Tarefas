namespace TaskManagerApi.DTOs;

public class TarefaResponse
{
    public int Id {get;set;}
    public string Titulo {get;set;} = string.Empty;
    public string Descricao {get;set;} = string.Empty;
    public string Status {get;set;} = string.Empty;
    public DateTime DataCriacao {get;set;}
    public string NomeColaborador {get;set;} = string.Empty;    
}