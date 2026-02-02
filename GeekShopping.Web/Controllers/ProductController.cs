using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeekShopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        // Construtor que injeta o serviço de produtos
        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        // ============================
        // 📦 LISTAGEM DE PRODUTOS
        // ============================
        // Exibe todos os produtos na tela inicial

        public async Task<IActionResult> ProductIndex(string categoria)
        {
            var products = await _productService.FindAllProducts();

            // Preenche ViewBag com os nomes da enum
            var categorias = Enum.GetNames(typeof(CategoriaProduto)).ToList();
            ViewBag.Categorias = categorias;
            ViewBag.CategoriaSelecionada = categoria;

            // Aplica o filtro, se houver
            if (!string.IsNullOrEmpty(categoria) && Enum.TryParse<CategoriaProduto>(categoria, out var categoriaEnum))
            {
                products = products.Where(p => p.Category == categoriaEnum).ToList();
            }

            return View(products);
        }


        // ============================
        // 🆕 CRIAÇÃO DE PRODUTO
        // ============================

        // GET: Exibe o formulário para criar um novo produto

        [HttpGet]
        public IActionResult ProductCreate()
        {
            ViewBag.Categories = Enum.GetValues(typeof(CategoriaProduto))
                          .Cast<CategoriaProduto>()
                          .Select(c => new SelectListItem
                          {
                              Text = c.ToString(),
                              Value = ((int)c).ToString()
                          }).ToList();

            return View();
        }


        // POST: Recebe os dados do formulário e cria o produto
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _productService.CreateProduct(model);
                    if (result != null)
                    {
                        return RedirectToAction(nameof(ProductIndex));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao salvar: {ex.Message}");
                }
            }

            ViewBag.Categories = Enum.GetValues(typeof(CategoriaProduto))
                                  .Cast<CategoriaProduto>()
                                  .Select(c => new SelectListItem
                                  {
                                      Text = c.ToString(),
                                      Value = ((int)c).ToString()
                                  }).ToList();

            return View(model);
        }

        // ============================
        // ✏️ EDIÇÃO DE PRODUTO
        // ============================

        // GET: Carrega os dados do produto para edição
        [HttpGet]
        public async Task<IActionResult> ProductEdit(int id)
        {
            var product = await _productService.FindByIdProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            // Adiciona o ViewBag com as categorias, igual ao ProductCreate
            ViewBag.Categories = Enum.GetValues(typeof(CategoriaProduto))
                              .Cast<CategoriaProduto>()
                              .Select(c => new SelectListItem
                              {
                                  Text = c.ToString(),
                                  Value = ((int)c).ToString()
                              }).ToList();

            return View(product);

        }

        // POST: Recebe os dados editados e atualiza o produto
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.UpdateProduct(model);
                if (result != null)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

        // ============================
        // 🗑️ EXCLUSÃO DE PRODUTO
        // ============================

        // GET: Carrega os dados do produto para confirmação de exclusão
        [HttpGet]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var product = await _productService.FindByIdProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Confirma e executa a exclusão do produto
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductModel model)
        {
            var result = await _productService.DeleteProduct(model.Id);
            if (result)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
            return View(model);
        }
    }
}