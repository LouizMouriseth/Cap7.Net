using Core;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.ConsumoRepository;

public class ConsumoRepository : GenericRepository<Consumo>, IConsumoRepository
{
    public ConsumoRepository(Context context) : base(context)
    {
    }
}