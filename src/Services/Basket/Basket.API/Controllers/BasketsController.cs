using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public BasketsController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
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
            => Ok(await _basketRepository.UpdateBasket(model));

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
            await _basketRepository.DeleteBasket(userName);
            return Ok();
        }

        #endregion
    }
}
