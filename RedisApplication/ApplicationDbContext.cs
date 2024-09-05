using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace RedisApplication
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        {
        }
    }
}
