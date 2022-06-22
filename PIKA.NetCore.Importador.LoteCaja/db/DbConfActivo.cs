using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja.db
{
    public class DbConfActivo : IEntityTypeConfiguration<Activo>
    {
        public void Configure(EntityTypeBuilder<Activo> builder)
        {
            builder.ToTable(MySQLDb.TablaActivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            //¿Requerida?
            builder.Property(x => x.EntradaClasificacionId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.IDunico).HasMaxLength(LongitudDatos.IDunico);
            builder.Property(x => x.CuadroClasificacionId).HasMaxLength(LongitudDatos.IDunico).IsRequired();
            builder.Property(x => x.Asunto).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.FechaApertura).IsRequired();
            builder.Property(x => x.FechaCierre).IsRequired(false);
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.EsElectronico).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.EnPrestamo).IsRequired();
            builder.Property(x => x.Reservado).IsRequired();
            builder.Property(x => x.Confidencial).IsRequired();
            builder.Property(x => x.Ampliado).IsRequired();
            builder.Property(x => x.ArchivoOrigenId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ArchivoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoArchivoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.FechaRetencionAC).IsRequired(false);
            builder.Property(x => x.FechaRetencionAT).IsRequired(false);
            builder.Property(x => x.TieneContenido).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ElementoId).IsRequired(false);
            builder.Property(x => x.UnidadAdministrativaArchivoId).IsRequired(false);
            builder.Property(x => x.UbicacionCaja).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.UbicacionRack).IsRequired(false).HasMaxLength(100);

            builder.Property(x => x.AlmacenArchivoId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ZonaAlmacenId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ContenedorAlmacenId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);

            builder.HasIndex(i => new { i.UbicacionCaja });
            builder.HasIndex(i => new { i.UbicacionRack });
            builder.HasIndex(i => new { i.UnidadAdministrativaArchivoId });
            builder.HasIndex(i => new { i.ArchivoOrigenId });
            builder.HasIndex(i => new { i.TipoArchivoId });
            builder.HasIndex(i => new { i.EnPrestamo });
            builder.HasIndex(i => new { i.Ampliado });
            builder.HasIndex(i => new { i.FechaRetencionAC });
            builder.HasIndex(i => new { i.FechaRetencionAT });
            builder.HasIndex(i => new { i.FechaCierre });
            builder.HasIndex(i => new { i.FechaApertura });
            builder.HasIndex(i => new { i.EntradaClasificacionId });
            builder.HasIndex(i => new { i.CuadroClasificacionId });
            builder.HasIndex(i => new { i.Eliminada });
            builder.HasIndex(i => new { i.Nombre });
            builder.HasIndex(i => new { i.CodigoElectronico });
            builder.HasIndex(i => new { i.CodigoOptico });
            builder.HasIndex(i => new { i.ArchivoId });
            builder.HasIndex(i => new { i.TieneContenido });

        }
    }
}
