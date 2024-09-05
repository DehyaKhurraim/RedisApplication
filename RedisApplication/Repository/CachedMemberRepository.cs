using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RedisApplication.Interface;
using RedisApplication.Models;

namespace RedisApplication.Repository
{
    public class CachedMemberRepository : IMemberRepository
    {
        private readonly IMemberRepository _decorated;
        private readonly IDistributedCache _distributedCache;
        private readonly ApplicationDbContext _context;

        public CachedMemberRepository(IMemberRepository decorated, IDistributedCache distributedCache, ApplicationDbContext context)
        {
            _decorated = decorated;
            _distributedCache = distributedCache;
            _context = context;
        }

        public void Add(Member member)
        {
            _decorated.Add(member);
        }

        public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string key = $"member-{id}";
            string? cachedMemeber = await _distributedCache.GetStringAsync(key, cancellationToken);
            Member? member;

            if (string.IsNullOrEmpty(cachedMemeber))
            {
                member = await _decorated.GetByIdAsync(id,cancellationToken);
                if (member == null)
                  return null;

                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(member), cancellationToken);
                return member;
            }

            member = JsonConvert.DeserializeObject<Member>(
                cachedMemeber,   
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });

            if(member != null)
            {
                _context.Set<Member>().Attach(member);
            }

            return member;
        }

        public Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
        {
            return _decorated.IsEmailUniqueAsync(email, cancellationToken);
        }

        public void Update(Member member)
        {
            _decorated?.Update(member);
        }
    }
}
