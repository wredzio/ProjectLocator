using ProjectLocator.Areas.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Identity.Accounts.Register
{
    internal interface IMapper<T1, T2>
    {
        T1 Map(T2 mapTo);
        T2 Map(T1 mapTo);
    }

    internal class RegisterCommandApplicationUserMapper : IMapper<RegisterCommand, ApplicationUser>
    {
        public RegisterCommand Map(ApplicationUser mapFrom)
        {
            return new RegisterCommand();
        }

        public ApplicationUser Map(RegisterCommand mapTFrom)
        {
            return new ApplicationUser();
        }
    }
}
