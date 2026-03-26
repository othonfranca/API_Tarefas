namespace TaskManagerApi.DTOs;

public class DashboardResponse
{
    public int TotalTarefas {get;set;}
    public int TarefasConcluidas {get;set;}
    public int TarefasEmAndamento {get;set;}
    public int TarefasPendentes {get;set;}
}