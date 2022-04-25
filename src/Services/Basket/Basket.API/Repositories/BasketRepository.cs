using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json;

using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        #region Props & Ctor

        private readonly IDistributedCache _cache;

        public BasketRepository(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        #endregion

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            string basket = await _cache.GetStringAsync(userName);
            if(string.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _cache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return basket;
        }

        public async Task DeleteBasket(string userName)
            => await _cache.RemoveAsync(userName);
    }
}
