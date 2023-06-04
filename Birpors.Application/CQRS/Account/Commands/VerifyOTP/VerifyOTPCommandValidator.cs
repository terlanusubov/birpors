using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Application.CQRS.Account.Commands.VerifyOTP
{
    public class VerifyOTPCommandValidator:AbstractValidator<VerifyOTPCommand>
    {
        public VerifyOTPCommandValidator()
        {
            RuleFor(c => c.Model.PhoneNumber).Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$").WithMessage("Telefon nömrəsi şablona uyğun deyil xx-xxx-xx-xx")
                                             .NotNull().WithMessage("Telefon nömrəsi qeyd edilməyib.");

            RuleFor(c => c.Model.Code).NotNull().WithMessage("OTP qeyd edilməyib.");
            RuleFor(c => c.Model.UserDeviceId).NotNull().WithMessage("Cihaz ID qeyd edilməyib.");
            RuleFor(c => c.Model.RoleId).NotNull().WithMessage("Role ID qeyd edilməyib.");


        }
    }
}
