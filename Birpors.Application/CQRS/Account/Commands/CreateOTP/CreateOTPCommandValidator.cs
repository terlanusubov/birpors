using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Application.CQRS.Account.Commands.CreateOTP
{
    public  class CreateOTPCommandValidator:AbstractValidator<CreateOTPCommand>
    {
        public CreateOTPCommandValidator()
        {
            RuleFor(c => c.Model.PhoneNumber).Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$").WithMessage("Telefon nömrəsi şablona uyğun deyil xx-xxx-xx-xx")
                                             .NotNull().WithMessage("Telefon nömrəsi qeyd edilməyib.");

        }
    }
}
