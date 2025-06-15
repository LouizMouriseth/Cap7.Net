using Core;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.AcaoRepository;

public class AcaoRepository : GenericRepository<Acao>, IAcaoRepository
{
    public AcaoRepository(Context context) : base(context)
    {
    }
}