using Core;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.EmpresaRepository;

public class EmpresaRepository : GenericRepository<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(Context context) : base(context)
    {
    }

    public async Task<Empresa?> GetByCnpjAsync(string cnpj)
    {
        return await Context.Empresas.FirstOrDefaultAsync(u => u.Cnpj == cnpj);
    }
}