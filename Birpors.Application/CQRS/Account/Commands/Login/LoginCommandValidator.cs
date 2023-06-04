using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Application.CQRS.Account.Commands.Login
{
    public class LoginCommandValidator:AbstractValidator<LoginCommandRequest>
    {
        public LoginCommandValidator()
        {
            RuleFor(c => c.PhoneNumber).Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$").WithMessage("Telefon nömrəsi şablona uyğun deyil xx-xxx-xx-xx")
                                            .NotNull().WithMessage("Telefon nömrəsi qeyd edilməyib.");

            RuleFor(c => c.UserDeviceId).NotNull().WithMessage("Cihaz ID qeyd edilməyib.");
            RuleFor(c => c.SecretKey).NotNull().WithMessage("SecretKey qeyd edilməyib.");
        }
    }
}
