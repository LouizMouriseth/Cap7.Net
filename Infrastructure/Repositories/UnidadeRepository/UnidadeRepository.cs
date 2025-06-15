using Core;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.UnidadeRepository;

public class UnidadeRepository : GenericRepository<Unidade>, IUnidadeRepository
{
    public UnidadeRepository(Context context) : base(context)
    {
    }
}