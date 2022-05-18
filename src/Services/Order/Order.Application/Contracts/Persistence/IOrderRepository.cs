using System.Threading.Tasks;
using System.Collections.Generic;

using Order.Domain.Entities;

namespace Order.Application.Contracts.Persistence
{
    public interface IOrderRepository : IAsyncRepository<OrderModel>
    {
        Task<IEnumerable<OrderModel>> GetOrderByUserName(string userName);
    }
}
