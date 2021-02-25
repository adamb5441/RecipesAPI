using DotNETElasticsearchPOC.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json.Linq;
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

        public async virtual Task<Product[]> SearchProductsPage(string query, int page, int pageSize)
        {
            var response = await _elasticClient.SearchAsync<Product>(s =>
                s.Query(q => q.QueryString(d => d.Query('*' + query + '*')))
                .From((page - 1) * pageSize)
                .Size(pageSize));
            if (!response.IsValid)
            {
                return new Product[] { };
            }

            return response.Documents.ToArray();
        }

        public async virtual Task<Product[]> FuzzySearch(string query)
        {
            var response = await _elasticClient.SearchAsync<Product>(s => s
               .Query(q => q.Bool(b => b
                        .Should(
                            s => s.Match(m => m.Query(query).Field(f => f.Name).Fuzziness(Fuzziness.EditDistance(1)).Boost(3)),
                            s => s.Match(m => m.Query(query).Field(f => f.Description).PrefixLength(4).Fuzziness(Fuzziness.EditDistance(2)))
                        )
                    )
                )
                .From(1)
                .Size(100));
            if (!response.IsValid)
            {
                return new Product[] { };
            }

            return response.Documents.ToArray();
        }
        public async virtual Task<ValueAggregate> GetAverageRating()
        {
            var response = await _elasticClient.SearchAsync<Product>(s => s
                .Query(q => q.QueryString(d => d.Query('*' + "" + '*')))
                .Aggregations(ag => ag
                    .Average("avg_Rating", avg => avg
                        .Field(field => field.Rating)
                    )
                )
            );
            return response.Aggregations.Average("avg_Rating");
        }
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
        }
        public async Task Import(Product[] products)
        {
            //TODO: implement indexing
            var productsLIst = await _elasticClient.IndexManyAsync(products);
        }
        public async Task SaveBulkAsync(Product[] products)
        {
            await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
        }
    }
}
