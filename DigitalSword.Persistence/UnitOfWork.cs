using DigitalSword.Application.Abstractions;
using DigitalSword.Persistence.Data;
using DigitalSword.Persistence.Repositories;

namespace DigitalSword.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public UnitOfWork(AppDbContext context) => _context = context;

        public IGenericRepository<T> GetRepository<T>() where T : class => new GenericRepository<T>(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) { _context.Dispose(); }
            _disposed = true;
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    }
}
