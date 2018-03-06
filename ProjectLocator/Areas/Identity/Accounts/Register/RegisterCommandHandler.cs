using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectLocator.Areas.Identity.DatabaseContext;
using ProjectLocator.Areas.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Identity.Accounts.Register
{
    internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, long>
    {
        private UserManager<ApplicationUser> _userManager;
        IMapper<RegisterCommand, ApplicationUser> _mapper;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IMapper<RegisterCommand, ApplicationUser> mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<long> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var applicationUser = _mapper.Map(request);
            var user = await _userManager.CreateAsync(applicationUser);

            if(user.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, Roles.Locator.ToString());
            }

            return 0;
        }
    }
}
