using Core;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.UnidadeRepository;

public class UnidadeRepository : GenericRepository<Unidade>, IUnidadeRepository
{
    public UnidadeRepository(Context context) : base(context)
    {
    }

    public async Task<List<Unidade>> ListMoreEfficient(CancellationToken ct)
    {
        return await Context.Unidades
            .Select(u => new
            {
                Unidade = u,
                UltimoConsumo = u.Consumos
                    .OrderByDescending(c => c.DataReferencia)
                    .FirstOrDefault()
            })
            .Where(x => x.UltimoConsumo != null)
            .Select(x => new
            {
                Unidade = x.Unidade,
                ConsumoPorArea = x.UltimoConsumo.ConsumoTotal / x.Unidade.Area
            })
            .OrderBy(x => x.ConsumoPorArea)
            .Take(3)
            .Select(x => x.Unidade)
            .ToListAsync(ct);
    }
    
    public async Task<List<Unidade>> ListLessEfficient(CancellationToken ct)
    {
        return await Context.Unidades
            .Select(u => new
            {
                Unidade = u,
                UltimoConsumo = u.Consumos
                    .OrderByDescending(c => c.DataReferencia)
                    .FirstOrDefault()
            })
            .Where(x => x.UltimoConsumo != null)
            .Select(x => new
            {
                Unidade = x.Unidade,
                ConsumoPorArea = x.UltimoConsumo.ConsumoTotal / x.Unidade.Area
            })
            .OrderByDescending(x => x.ConsumoPorArea)
            .Take(3)
            .Select(x => x.Unidade)
            .ToListAsync(ct);
    }
}