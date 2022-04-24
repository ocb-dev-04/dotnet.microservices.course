using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Catalog.API.Entities;
using Catalog.API.Repositories;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        #region Props & Ctor

        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogsController> _logger;

        public CatalogsController(
            IProductRepository productRepository,
            ILogger<CatalogsController> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Queries

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name = nameof(GetProductById))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] string id)
        {
            Product find = await _productRepository.GetProductById(id);
            if (find == null) return NotFound();

            return Ok(find);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
            => Ok(await _productRepository.GetProducts());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("by-name/{name}", Name = nameof(GetProductsByName))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName([FromRoute] string name)
            => Ok(await _productRepository.GetProductsByName(name));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [HttpGet("by-category/{categoryName}", Name = nameof(GetProductsByCategory))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory([FromRoute] string categoryName)
            => Ok(await _productRepository.GetProductsByCategory(categoryName));

        #endregion

        #region Commands

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create([FromBody] Product model)
        {
            try
            {
                await _productRepository.Create(model);
                return CreatedAtRoute(nameof(GetProductById), model.Id, model);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update([FromBody] Product model)
        {
            bool updated = await _productRepository.Update(model);
            return updated ? Ok() : Problem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            bool deleted = await _productRepository.Delete(id);
            return deleted ? Ok() : Problem();
        }

        #endregion
    }
}
