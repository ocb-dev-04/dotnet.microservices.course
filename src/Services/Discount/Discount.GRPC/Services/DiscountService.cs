using AutoMapper;
using Discount.Grpc.Protos;
using Discount.GRPC.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.GRPC.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        #region Props & Ctor

        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(
            IDiscountRepository repository,
            ILogger<DiscountService> logger,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            _logger.LogCritical($"Get discount to product name => {request.ProductName}");

            var coupon = await _repository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, nameof(coupon)));
            }

            string asString = System.Text.Json.JsonSerializer.Serialize(coupon);
            _logger.LogCritical($"discount to product name => {request.ProductName} is {asString}");

            return _mapper.Map<CouponModel>(coupon);
        }
    }
}
