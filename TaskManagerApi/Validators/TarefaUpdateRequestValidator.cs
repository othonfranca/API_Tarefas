using FluentValidation;
using TaskManagerApi.DTOs;
using TaskManagerApi.Enums;

namespace TaskManagerApi.Validators;

public class TarefaUpdateRequestValidator : AbstractValidator<TarefaUpdateRequest>
{
    public TarefaUpdateRequestValidator()
    {
        RuleFor( x => x.Status)
            .NotEmpty().WithMessage("O status não pode ser vazio")
            .Must(status =>
            {
                return Enum.TryParse<StatusTarefa>(status, true, out _);
            })
            .WithMessage("Status inválido. Use um dos status possíveis");
    }
}