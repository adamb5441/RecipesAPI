using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DotNETElasticsearchPOC.Application.Models;
using DotNETElasticsearchPOC.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;

namespace RecipesAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("SearchPage")]
        public async Task<IActionResult> SearchProductsPage(string query, int page = 1, int pageSize = 5)
        {
            var response = await _productService.SearchProductsPage( query, page, pageSize);

            return Ok(response);
        }
        [HttpGet("SearchProducts")]
        public async Task<IActionResult> SearchProducts(string query)
        {
            var response = await _productService.FuzzySearch(query);

            return Ok(response);
        }
        [HttpGet("AverageRating")]
        public async Task<IActionResult> GetAverageRating()
        {
            var response = await _productService.GetAverageRating();

            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProductById(id);

            return Ok(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var exists = await _productService.GetProductById(id);

            if (exists != null)
            {
                await _productService.SaveSingleAsync(exists);
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var exists = await _productService.GetProductById(id);

            if (exists != null)
            {
                await _productService.DeleteAsync(exists);
                return Ok();
            }

            return NotFound();
        }

        [HttpGet("FakeImport/{count}")]
        public async Task<ActionResult> Import(int count = 0)
        {
            var productFaker = new Faker<Product>()
                   .CustomInstantiator(f => new Product())
                   .RuleFor(p => p.Id, f => f.IndexFaker)
                   .RuleFor(p => p.Ean, f => f.Commerce.Ean13())
                   .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                   .RuleFor(p => p.Description, f => f.Lorem.Sentence(f.Random.Int(5, 20)))
                   .RuleFor(p => p.Brand, f => f.Company.CompanyName())
                   .RuleFor(p => p.Category, f => f.Commerce.Categories(1).First())
                   .RuleFor(p => p.Price, f => f.Commerce.Price(1, 1000, 2, "$"))
                   .RuleFor(p => p.Quantity, f => f.Random.Int(0, 1000))
                   .RuleFor(p => p.Rating, f => f.Random.Float(0, 1));


            var products = productFaker.Generate(count);
            await _productService.SaveManyAsync(products.ToArray());

            return Ok();
        }

    }
}
