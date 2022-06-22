using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja.db
{

    public partial class MySQLDb: DbContext
    {
        public static string TablaActivos { get => "gd$activo"; }

        public MySQLDb(): base()
        {
        }

        public DbSet<Activo> Activos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(AppConfig.MySQlConnString);
            }
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DbConfActivo());
            base.OnModelCreating(builder);
        }


    }
}
