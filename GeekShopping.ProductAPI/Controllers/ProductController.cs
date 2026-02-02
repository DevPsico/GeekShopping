using GeekShopping.ProductAPI.Data.Dto;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductController(IProductRepository repository)
    {
        _repository = repository;
    }
    



    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _repository.FindAll();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(long id)
    {
        var product = await _repository.FindById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto dto)
    {
        if (dto == null) return BadRequest();
        var product = await _repository.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] ProductDto dto)
    {
        if (dto == null || dto.Id != id)
            return BadRequest("ID do corpo e da URL devem ser iguais.");

        var updatedProduct = await _repository.Update(dto);
        if (updatedProduct == null)
            return NotFound("Produto não encontrado.");

        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var result = await _repository.Delete(id);
        if (!result) return NotFound();
        return NoContent();
    }
}