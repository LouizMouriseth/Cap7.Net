using Core;

namespace Infrastructure.Repositories.UnidadeRepository;

public interface IUnidadeRepository : IGenericRepository<Unidade>
{
    Task<List<Unidade>> ListMoreEfficient(CancellationToken ct);
    Task<List<Unidade>> ListLessEfficient(CancellationToken ct);
}