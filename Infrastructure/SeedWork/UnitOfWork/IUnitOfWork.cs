using Infrastructure.Repositories.AcaoRepository;
using Infrastructure.Repositories.ConsumoRepository;
using Infrastructure.Repositories.EmpresaRepository;
using Infrastructure.Repositories.UnidadeAcaoRepository;
using Infrastructure.Repositories.UnidadeRepository;
using Infrastructure.Repositories.UserRepository;

namespace Infrastructure.SeedWork.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }    
    IAcaoRepository AcaoRepository { get; }
    IConsumoRepository ConsumoRepository { get; }
    IEmpresaRepository EmpresaRepository { get; }
    IUnidadeRepository UnidadeRepository { get; }
    IUnidadeAcaoRepository UnidadeAcaoRepository { get; }
    
    void Commit();
    Task CommitAsync(CancellationToken cancellationToken);
}