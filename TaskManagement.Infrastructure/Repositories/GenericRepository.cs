using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly TaskManagementContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(TaskManagementContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    protected IQueryable<T> Query()
        => _dbSet.AsNoTracking();

    protected IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        => _dbSet.AsNoTracking().Where(predicate);

    public Task<bool> ExistAsync(Guid id)
        => _dbSet.AsNoTracking().AnyAsync(x => x.Id == id);

    public Task<T?> GetByIdAsync(Guid id)
        => _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

    public async Task<T> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        _context.Entry(entity).Property(p => p.RowVersion).OriginalValue = entity.RowVersion;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
