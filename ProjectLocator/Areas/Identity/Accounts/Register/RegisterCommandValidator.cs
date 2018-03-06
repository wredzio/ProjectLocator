using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Identity.Accounts.Register
{
    internal class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        internal RegisterCommandValidator()
        {
            RuleFor(register => register.FirstName).NotEmpty();
        }
    }
}
