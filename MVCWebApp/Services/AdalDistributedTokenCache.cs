using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    internal class AdalDistributedTokenCache : TokenCache
    {
        private readonly IDistributedCache _cache;
        private readonly string _userId;

        public AdalDistributedTokenCache(IDistributedCache cache, string userId)
        {
            _cache = cache;
            _userId = userId;
            BeforeAccess = BeforeAccessNotification;
            AfterAccess = AfterAccessNotification;
        }

        private string getCacheKey()
        {
            return $"{_userId}_TokenCache";
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            if (_cache != null)
            {
                Deserialize(_cache.Get(getCacheKey()));
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (HasStateChanged)
            {
                _cache.Set(getCacheKey(), Serialize(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });

                HasStateChanged = false;
            }
        }
    }
}
