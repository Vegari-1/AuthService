using AuthService.Repository.Interface;

namespace AuthService.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> Save(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}

