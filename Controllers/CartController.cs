using Ecommerce_Vault.Data; // Ajusta a tu namespace de DbContext
using Ecommerce_Vault.Extensions;
using Ecommerce_Vault.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce_Vault.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<CartItemViewModel> GetCart()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("CartSession") ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            HttpContext.Session.SetObjectAsJson("CartSession", cart);
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productoId, int cantidad = 1)
        {
            var producto = _context.Products.FirstOrDefault(p => p.Id == productoId);
            if (producto == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductoId == productoId);

            if (item != null)
            {
                item.Cantidad += cantidad;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Name, // Revisa si tu propiedad en C# es Name o Nombre
                    Precio = producto.Price,
                    Cantidad = cantidad,
                    StockDisponible = producto.Stock,
                    ImagenUrl = producto.ImageUrl
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productoId, int cantidad)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductoId == productoId);

            if (item != null)
            {
                if (cantidad > 0)
                {
                    item.Cantidad = cantidad;
                }
                else
                {
                    cart.Remove(item);
                }
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int productoId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.ProductoId == productoId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/ClearCart
        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("CartSession");
            return RedirectToAction("Index");
        }
    }
}