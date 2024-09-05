using Microsoft.EntityFrameworkCore;
using RedisApplication.Interface;
using RedisApplication.Models;

namespace RedisApplication.Repository
{
    public class MemberRepository: IMemberRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MemberRepository(ApplicationDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _dbContext
                .Set<Member>()
                .FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

        public async Task<bool> IsEmailUniqueAsync(
            string email,
            CancellationToken cancellationToken = default) =>
            !await _dbContext
                .Set<Member>()
                .AnyAsync(member => member.Email == email, cancellationToken);

        public void Add(Member member) =>
            _dbContext.Set<Member>().Add(member);

        public void Update(Member member) =>
            _dbContext.Set<Member>().Update(member);
    }
}
