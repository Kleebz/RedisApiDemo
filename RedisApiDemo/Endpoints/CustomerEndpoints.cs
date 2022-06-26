using RedisApiDemo.Domain.Repository.Interface;

namespace RedisApiDemo.Api.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            app.MapGet("api/customers", async (ICustomerRepository customerRepository) =>
            {
                return await customerRepository.GetCustomersAsync();
            });

            app.MapGet("api/customer", async (ICustomerRepository customerRepository, int customerId) =>
            {
                return await customerRepository.GetCustomerByIdAsync(customerId);
            });

            app.MapGet("api/customer_hashexample", async (ICustomerRepository customerRepository, int customerId) =>
            {
                return await customerRepository.GetCustomerByIdAsync_HashedExample(customerId);
            });

        }
    }
}
