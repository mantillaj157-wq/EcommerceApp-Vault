using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Vault.Data;
using Ecommerce_Vault.Models;

namespace Ecommerce_Vault.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string? category, string? searchString)
        {
            var query = _context.Products.AsQueryable();

            // Filtrar por categoría simple (Partes de Arriba / Partes de Abajo)
            if (!string.IsNullOrEmpty(category))
            {
                if (category.Equals("Abajo", StringComparison.OrdinalIgnoreCase) || category.Equals("Partes de Abajo", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Category == "Partes de Abajo" || p.Category == "Abajo");
                }
                else if (category.Equals("Arriba", StringComparison.OrdinalIgnoreCase) || category.Equals("Partes de Arriba", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Category == "Partes de Arriba" || p.Category == "Arriba");
                }
                else if (category.Equals("Ofertas", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Category == "Ofertas");
                }
            }

            // Filtrar por búsqueda si se envió desde la barra superior
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = searchString;

            return View(await query.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // 1. Verificar si el ID enviado desde la URL es válido
            if (id == null)
            {
                return NotFound();
            }

            // 2. Consultar a la base de datos por el producto específico
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            // 3. Si no existe, responder error 404
            if (product == null)
            {
                return NotFound();
            }

            // 4. Enviar el producto encontrado a la vista Details.cshtml
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,Category,ImageUrl")] Product product, string? additionalImages)
        {
            if (ModelState.IsValid)
            {
                // Si ingresaste imágenes adicionales, las concatenamos a la propiedad ImageUrl separadas por coma
                if (!string.IsNullOrWhiteSpace(additionalImages))
                {
                    product.ImageUrl = $"{product.ImageUrl},{additionalImages}";
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}