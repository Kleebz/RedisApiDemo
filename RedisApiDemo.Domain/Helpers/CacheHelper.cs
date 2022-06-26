using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisApiDemo.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisApiDemo.Domain.Helpers
{
    public static class CacheHelper
    {
        public static async Task<T> GetStringAsync<T>(string key, IDistributedCache cache, TimeSpan expiration, Func<Task<T>> dataSourceQuery)
        {
            T t;

            var cachedT = await cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cachedT))
            {
                t = JsonConvert.DeserializeObject<T>(cachedT);
            }
            else
            {
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()//more options: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.distributedcacheentryoptions?view=dotnet-plat-ext-6.0
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };
                t = await dataSourceQuery();
                await cache.SetStringAsync(key, JsonConvert.SerializeObject(t), options);
            }

            return t;
        }
    }
}
