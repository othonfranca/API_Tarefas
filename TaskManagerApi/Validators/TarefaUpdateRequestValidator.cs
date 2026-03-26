using FluentValidation;
using TaskManagerApi.DTOs;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;

namespace TaskManagerApi.Validators;

public class TarefaUpdateRequestValidator : AbstractValidator<TarefaUpdateRequest>
{
    public TarefaUpdateRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("O status não pode ser vazio!")
            .Must(BeValidStatus)
            .WithMessage("Status inválido. Use: Pendente, EmAndamento ou Concluido.");
    }

    private bool BeValidStatus(string status)
    {
        if(Enum.IsDefined(typeof(StatusTarefa), status))
        {
            return true;
        }

        if (int.TryParse(status, out int intStatus))
        {
            return Enum.IsDefined(typeof(StatusTarefa), intStatus);
        }

        return false;
    }
}