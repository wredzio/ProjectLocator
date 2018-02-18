using ProjectLocator.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Identity.DatabaseContext
{
    public class IdentityDbContextFactory : DesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityDbContextFactory()
        {
            _databaseName = "IdentityConnection";
        }
    }
}
