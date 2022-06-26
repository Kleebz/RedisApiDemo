using Microsoft.EntityFrameworkCore;
using RedisApiDemo.Domain.Models;
using RedisApiDemo.Domain.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using RedisApiDemo.Domain.Helpers;

namespace RedisApiDemo.Domain.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private RedisDemoDbContext _context;
        private IDistributedCache _cache;

        public CustomerRepository(RedisDemoDbContext redisDemoDbContext, IDistributedCache cache) 
        {
            _context = redisDemoDbContext;
            _cache = cache;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync() => await CacheHelper.GetStringAsync("customers", _cache, TimeSpan.FromMinutes(20),
                async () =>
                {
                    await Task.Delay(5000);
                    return await _context.Customers.ToListAsync();
                });

        public async Task<Customer> GetCustomerByIdAsync(int id) => (await GetCustomersAsync()).Where(x => x.CustomerId == id).SingleOrDefault();

        public async Task<Customer> GetCustomerByIdAsync_HashedExample(int id)
        {
            var query = _context.Customers.Where(x => x.CustomerId == id);
            var queryString = query.ToQueryString();
            var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(queryString));

            var customer = await CacheHelper.GetStringAsync(key, _cache, TimeSpan.FromMinutes(20),
                async () =>
                {
                    await Task.Delay(5000);
                    return await query.SingleOrDefaultAsync();
                });

            return customer;
        }
    }
}
