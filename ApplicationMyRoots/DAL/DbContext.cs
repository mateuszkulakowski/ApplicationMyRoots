using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.DAL
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext() : base("MyRootsDatabase") { }

        public DbSet<User> Users { get; set; }

    }
}