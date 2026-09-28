using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_Vault.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioEmail { get; set; } = string.Empty;

        [Required]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public string FormaPago { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        // Relación con el detalle
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}