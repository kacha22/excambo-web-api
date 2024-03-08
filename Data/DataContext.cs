using Microsoft.EntityFrameworkCore;
using Excambo.Models;
using System;
using NetTopologySuite.Geometries;

namespace Excambo.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<MyGame> Mygame { get; set; }
        //public DbSet<Trade> Trade { get; set; }
    }
}