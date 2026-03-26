namespace TaskManagerApi.DTOs;

public class PaginatedResponse<T> // T = Generics (Genéricos) no C#. Ele funciona como um "espaço reservado" ou um coringa para um tipo de dado que você só vai definir quando usar a classe, senão teria que criar um DTO para cada Model
{
    public int PaginaAtual {get;set;}
    public int ItensPorPagina {get;set;}
    public int TotalRegistros {get;set;}
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / ItensPorPagina);
    public List<T> Dados {get;set;} = new();
}