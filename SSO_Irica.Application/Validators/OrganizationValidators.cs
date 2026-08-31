using FluentValidation;
using SSO_Irica.Application.DTOs.Organization.Requests;

namespace SSO_Irica.Application.Validators;

public sealed class CreateOrganizationTypeRequestValidator : AbstractValidator<CreateOrganizationTypeRequest>
{
    public CreateOrganizationTypeRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateOrganizationTypeRequestValidator : AbstractValidator<UpdateOrganizationTypeRequest>
{
    public UpdateOrganizationTypeRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateOrganizationUnitRequestValidator : AbstractValidator<CreateOrganizationUnitRequest>
{
    public CreateOrganizationUnitRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TypeId).GreaterThan(0);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId.HasValue);
    }
}

public sealed class UpdateOrganizationUnitRequestValidator : AbstractValidator<UpdateOrganizationUnitRequest>
{
    public UpdateOrganizationUnitRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TypeId).GreaterThan(0);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId.HasValue);
    }
}

public sealed class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>
{
    public CreatePositionRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrganizationUnitId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class UpdatePositionRequestValidator : AbstractValidator<UpdatePositionRequest>
{
    public UpdatePositionRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrganizationUnitId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class CreateGenderRequestValidator : AbstractValidator<CreateGenderRequest>
{
    public CreateGenderRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateGenderRequestValidator : AbstractValidator<UpdateGenderRequest>
{
    public UpdateGenderRequestValidator()
    {
        RuleFor(x => x.Code).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateUserPositionRequestValidator : AbstractValidator<CreateUserPositionRequest>
{
    public CreateUserPositionRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PositionId).GreaterThan(0);
        RuleFor(x => x.EndDateTime).GreaterThan(x => x.StartDateTime)
            .When(x => x.EndDateTime.HasValue);
    }
}

public sealed class UpdateUserPositionRequestValidator : AbstractValidator<UpdateUserPositionRequest>
{
    public UpdateUserPositionRequestValidator()
    {
        RuleFor(x => x.PositionId).GreaterThan(0);
        RuleFor(x => x.EndDateTime).GreaterThan(x => x.StartDateTime)
            .When(x => x.EndDateTime.HasValue);
    }
}
