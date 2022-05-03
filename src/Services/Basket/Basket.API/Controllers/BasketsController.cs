using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        #region Props & Ctor

        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly ILogger<BasketsController> _logger;

        public BasketsController(
            IBasketRepository basketRepository,
            DiscountGrpcService discountGrpcService,
            ILogger<BasketsController> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Queries

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("{userName}", Name = nameof(GetBasket))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> GetBasket([FromRoute] string userName)
            => Ok(await _basketRepository.GetBasket(userName));

        #endregion

        #region Commands

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update([FromBody] ShoppingCart model)
        {
            _logger.LogCritical($"Adding {model.Items.Count} to basket");
            foreach (var item in model.Items)
            {
                _logger.LogCritical("Consulting gRPC to discounts...");
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                _logger.LogCritical($"Discounts to {item.ProductName} is {coupon.Amount}");
                item.Price = coupon.Amount;
            }

            return Ok(await _basketRepository.UpdateBasket(model));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete([FromRoute] string userName)
        {
            // TODO: Comunicate with Discount.Grpc and calculate latest price of products into shopping cart

            await _basketRepository.DeleteBasket(userName);
            return Ok();
        }

        #endregion
    }
}
