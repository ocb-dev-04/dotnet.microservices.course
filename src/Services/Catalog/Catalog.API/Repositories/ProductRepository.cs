using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region Props & Ctor
        
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        #endregion

        public async Task<Product> GetProductById(string id)
            => await _context.Products.Find(p => p.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetProducts()
            => await _context.Products.Find(p => true).ToListAsync();

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Where(p => p.Name.ToLower().StartsWith(name.ToLower()));
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task Create(Product product)
            => await _context.Products.InsertOneAsync(product);

        public async Task<bool> Update(Product product)
        {
            ReplaceOneResult updateResult = await _context
                        .Products
                        .ReplaceOneAsync(
                            filter: f => f.Id.Equals(product.Id), 
                            replacement: product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            DeleteResult delete = await _context.Products.DeleteOneAsync(f => f.Id.Equals(id));
            return delete.IsAcknowledged && delete.DeletedCount > 0;
        }
    }
}
