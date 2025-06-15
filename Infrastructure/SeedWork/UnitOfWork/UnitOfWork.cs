using Infrastructure.Contexts;
using Infrastructure.Repositories.AcaoRepository;
using Infrastructure.Repositories.ConsumoRepository;
using Infrastructure.Repositories.EmpresaRepository;
using Infrastructure.Repositories.UnidadeAcaoRepository;
using Infrastructure.Repositories.UnidadeRepository;
using Infrastructure.Repositories.UserRepository;

namespace Infrastructure.SeedWork.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly Context _context;
    private bool _isDisposed = false;
    
    public IUserRepository UserRepository { get; }
    public IAcaoRepository AcaoRepository { get; }
    public IConsumoRepository ConsumoRepository { get; }
    public IEmpresaRepository EmpresaRepository { get; }
    public IUnidadeRepository UnidadeRepository { get; }
    public IUnidadeAcaoRepository UnidadeAcaoRepository { get; }
    

    public UnitOfWork(Context context, IUserRepository userRepository, IAcaoRepository acaoRepository, IConsumoRepository consumoRepository, IEmpresaRepository empresaRepository, IUnidadeRepository unidadeRepository, IUnidadeAcaoRepository unidadeAcaoRepository)
    {
        _context = context;
        UserRepository = userRepository;
        AcaoRepository = acaoRepository;
        ConsumoRepository = consumoRepository;
        EmpresaRepository = empresaRepository;
        UnidadeRepository = unidadeRepository;
        UnidadeAcaoRepository = unidadeAcaoRepository;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    protected virtual void Dispose(bool idDisposing)
    {
        if (this._isDisposed) return;
        
        if (idDisposing)
            _context.Dispose();
            
        this._isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}