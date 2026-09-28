using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Vault.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Total { get; set; }

        [Required(ErrorMessage = "La dirección de entrega es obligatoria")]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione una forma de pago")]
        public string FormaPago { get; set; } = string.Empty;

        public string? Observaciones { get; set; }
    }
}