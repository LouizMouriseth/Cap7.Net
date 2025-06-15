using System.Linq.Expressions;
using Core.SeedWork;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : GenericModel
{
    protected GenericRepository(Context context)
    {
        Context = context;
        _entity = Context.Set<T>();
    }

    protected readonly Context Context;
    private readonly DbSet<T> _entity;
    
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _entity.AddAsync(entity, cancellationToken);
    }

    public async Task<(List<T> data, int total)> ListAllAsync(
            Expression<Func<T, bool>>? filters = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int page = 1,
            int pageSize = 10,
            bool noTrack = true,
            IEnumerable<Expression<Func<T, object>>>? includes = null,
            CancellationToken ct = default
        )
    {
        var query = (IQueryable<T>)_entity;
            
        if (noTrack)
            query = query.AsNoTracking();
            
        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);
        
        if (filters != null)
            query = query.Where(filters);

        if (orderBy != null)
            query = orderBy(query);
        else
            query = query.OrderBy(e => e.Id);

        int total = await query.CountAsync(ct);
        
        query = query.Skip((page - 1) * pageSize).Take(pageSize);
        
        var data = await query.ToListAsync(ct);
                
        return (data, total);
    }

    public void Update(T entity, CancellationToken ct)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _entity
            .FirstAsync(e => e.Id == id, cancellationToken);

        entity.SetDeletedAt();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _entity.FindAsync(id, ct);
    }
}