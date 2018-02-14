using Microsoft.EntityFrameworkCore;
using ProjectLocator.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Application.DatabaseContext
{
    public class ApplicationContext : BaseDbContext<ApplicationContext>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }


        public DbSet<Test> Tests { get; set; }
    }

    public class Test : BaseEntity
    {
        public int aa { get; set; }
    }
}
