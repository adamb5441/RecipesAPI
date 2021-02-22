using DotNETElasticsearchPOC.Application.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNETElasticsearchPOC.Application.Services
{
    public class ProductService : IProductService
    {
        private List<Product> _cache = new List<Product>();

        private readonly IElasticClient _elasticClient;

        public ProductService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public virtual Task<IEnumerable<Product>> GetProducts(int count, int skip = 0)
        {
            var products = _cache
                .Where(p => p.ReleaseDate <= DateTime.UtcNow)
                .Skip(skip)
                .Take(count);

            return Task.FromResult(products);
        }

        public virtual Task<Product> GetProductById(int id)
        {
            var product = _cache
              .Where(p => p.ReleaseDate <= DateTime.UtcNow)
              .FirstOrDefault(p => p.Id == id);

            return Task.FromResult(product);
        }

        public virtual Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            var products = _cache
              .Where(p => p.ReleaseDate <= DateTime.UtcNow)
              .Where(p => p.Category.Contains(category, StringComparison.CurrentCultureIgnoreCase));

            return Task.FromResult(products);
        }

        public virtual Task<IEnumerable<Product>> GetProductsByBrand(string brand)
        {
            var products = _cache
              .Where(p => p.ReleaseDate <= DateTime.UtcNow)
              .Where(p => p.Brand.Contains(brand, StringComparison.CurrentCultureIgnoreCase));

            return Task.FromResult(products);
        }

        public async Task DeleteAsync(Product product)
        {
            await _elasticClient.DeleteAsync<Product>(product);

            if (_cache.Contains(product))
            {
                _cache.Remove(product);
            }
        }

        public async Task SaveSingleAsync(Product product)
        {
            if (_cache.Any(p => p.Id == product.Id))
            {
                await _elasticClient.UpdateAsync<Product>(product, u => u.Doc(product));
            }
            else
            {
                _cache.Add(product);
                await _elasticClient.IndexDocumentAsync<Product>(product);
            }
        }

        public async Task SaveManyAsync(Product[] products)
        {
            _cache.AddRange(products);
            await _elasticClient.IndexManyAsync(products);
        }

        public async Task SaveBulkAsync(Product[] products)
        {
            _cache.AddRange(products);
            await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
        }
    }
}
