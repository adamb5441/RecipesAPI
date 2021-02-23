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

        private readonly IElasticClient _elasticClient;

        public ProductService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        //public virtual Task<IEnumerable<Product>> GetProducts(int count, int skip = 0)
        //{
        //    var products = _cache
        //        .Where(p => p.ReleaseDate <= DateTime.UtcNow)
        //        .Skip(skip)
        //        .Take(count);

        //    return Task.FromResult(products);
        //}

        public async virtual Task<Product> GetProductById(int id)
        {
            var response = await _elasticClient.GetAsync<Product>(id, idx => idx.Index("products"));
            return response.Source;
        }

        public async Task DeleteAsync(Product product)
        {
            await _elasticClient.DeleteAsync<Product>(product);

        }

        public async Task SaveSingleAsync(Product product)
        {

                await _elasticClient.IndexDocumentAsync<Product>(product);
        }

        public async Task SaveManyAsync(Product[] products)
        {
            var productsLIst = await _elasticClient.IndexManyAsync(products);
            Console.WriteLine("done");
        }

        public async Task SaveBulkAsync(Product[] products)
        {
            await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
        }
    }
}
