using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Dapper;
using Npgsql;

using Discount.GRPC.Entities;

namespace Discount.GRPC.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly string _conectionString;

        public DiscountRepository(IConfiguration configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _conectionString = config.GetValue<string>("DatabaseSettings:ConnectionString");
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_conectionString);
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            if (coupon == null) return new Coupon() { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_conectionString);

            var affected =
                await connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return affected > 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_conectionString);

            var affected = await connection.ExecuteAsync
                    ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return affected > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_conectionString);

            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            return affected > 0;
        }

    }
}
