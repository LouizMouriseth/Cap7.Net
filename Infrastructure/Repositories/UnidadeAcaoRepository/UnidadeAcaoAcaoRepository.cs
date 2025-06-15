using Infrastructure.Contexts;

namespace Infrastructure.Repositories.UnidadeAcaoRepository;

public class UnidadeAcaoAcaoRepository : GenericRepository<Core.UnidadeAcao>, IUnidadeAcaoRepository
{
    public UnidadeAcaoAcaoRepository(Context context) : base(context)
    {
    }
}