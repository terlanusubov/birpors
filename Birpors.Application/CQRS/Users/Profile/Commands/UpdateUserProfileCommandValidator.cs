using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Application.CQRS.Users.Profile.Commands
{
    public class UpdateUserProfileCommandValidator: AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(c => c.Model.Name).NotEmpty().WithMessage("Ad boş qala bilməz.");
            RuleFor(c => c.Model.Surname).NotEmpty().WithMessage("Soyad boş qala bilməz.");
            RuleFor(c => c.Model.Email).NotEmpty().WithMessage("Email boş qala bilməz.");
        }
    }
}
