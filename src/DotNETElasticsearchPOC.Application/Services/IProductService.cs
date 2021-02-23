using DotNETElasticsearchPOC.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNETElasticsearchPOC.Application.Services
{
    public interface IProductService
    {
        //Task<IEnumerable<Product>> GetProducts(int count, int skip = 0);
        Task<Product> GetProductById(int id);
        Task DeleteAsync(Product product);
        Task SaveSingleAsync(Product product);
        Task SaveManyAsync(Product[] products);
        Task SaveBulkAsync(Product[] products);
    }
}
