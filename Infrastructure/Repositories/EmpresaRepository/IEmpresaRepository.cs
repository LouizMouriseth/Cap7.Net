using Core;

namespace Infrastructure.Repositories.EmpresaRepository;

public interface IEmpresaRepository : IGenericRepository<Empresa>
{
    Task<Empresa?> GetByCnpjAsync(string cnpj);
}