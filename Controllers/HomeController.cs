using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Vault.Data;

namespace Ecommerce_Vault.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Carga los productos para el "Último Drop" de la portada
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // ACCIÓN NUEVA: Guía de Talles
        public IActionResult Talles()
        {
            return View();
        }

        // ACCIÓN NUEVA: Preguntas Frecuentes
        public IActionResult Preguntas()
        {
            return View();
        }
    }
}