using Ecommerce_Vault.Data;
using Ecommerce_Vault.Extensions;
using Ecommerce_Vault.Models;
using Ecommerce_Vault.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_Vault.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Ventas/Checkout
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("CartSession") ?? new List<CartItemViewModel>();

            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Items = cart,
                Total = cart.Sum(x => x.Subtotal),
                DireccionEntrega = "General Alvear 245",
                Telefono = "62700549"
            };

            return View(model);
        }

        // POST: /Ventas/ConfirmarCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("CartSession") ?? new List<CartItemViewModel>();

            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            model.Items = cart;
            model.Total = cart.Sum(x => x.Subtotal);

            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            try
            {
                // 1. Crear el objeto Venta
                var nuevaVenta = new Venta
                {
                    UsuarioEmail = User.Identity?.Name ?? "invitado@vault.com",
                    DireccionEntrega = model.DireccionEntrega,
                    Telefono = model.Telefono,
                    FormaPago = model.FormaPago,
                    Observaciones = model.Observaciones,
                    Total = model.Total,
                    FechaVenta = DateTime.UtcNow
                };

                _context.Ventas.Add(nuevaVenta);
                await _context.SaveChangesAsync(); // Guarda en PostgreSQL (Supabase) y genera el ID

                // 2. Insertar los detalles
                foreach (var item in cart)
                {
                    var detalle = new DetalleVenta
                    {
                        VentaId = nuevaVenta.Id,
                        ProductoId = item.ProductoId,
                        NombreProducto = item.Nombre,
                        PrecioUnitario = item.Precio,
                        Cantidad = item.Cantidad,
                        Subtotal = item.Subtotal
                    };
                    _context.DetalleVentas.Add(detalle);
                }

                await _context.SaveChangesAsync();

                // 3. Limpiar Carrito
                HttpContext.Session.Remove("CartSession");

                return RedirectToAction("Confirmacion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al registrar la compra: " + ex.Message);
                return View("Checkout", model);
            }
        }

        public IActionResult Confirmacion()
        {
            return View();
        }
    }
}