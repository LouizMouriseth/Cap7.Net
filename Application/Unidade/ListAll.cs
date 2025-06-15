using Application.SeedWork.Responses;
using Core;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Unidade;

public class ListAll
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        /// <summary>
        /// Página atual, começa em 1
        /// </summary>
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// Número de itens por página
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
    
    public class ViewModel
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Endereco { get; private set; }
        public string Estado { get; private set; }
        public float Area { get; private set; }
        public DateTime InicioOperacao { get; private set; }
        public Guid IdEmpresa { get; private set; }
        public EmpresaViewModel Empresa { get; private set; }
        public ICollection<UnidadeAcaoViewModel> UnidadesAcoes { get; private set; }
        public ICollection<ConsumoViewModel> Consumos { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
    
    #region Sub ViewModels

    public class EmpresaViewModel
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Cnpj { get; private set; }
        public string Segmento { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }

    public class UnidadeAcaoViewModel
    {
        public Guid Id { get; private set; }
        public DateTime DataImplantacao { get; private set; }
        public Guid IdUnidade { get; private set; }
        public Guid IdAcao { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }

    public class ConsumoViewModel
    {
        public Guid Id { get; private set; }
        public DateTime DataReferencia { get; private set; }
        public float ConsumoTotal { get; private set; }
        public string TipoFonte { get; private set; }
        public bool ERenovavel { get; private set; }
        public Guid IdConsumo { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
    
    #endregion

    internal class Service : IRequestHandler<Request, BaseResponse<ViewModel>>
    {
        private readonly IUnitOfWork _uow;
        private readonly TypeAdapterConfig _config;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
            _config = CreateAdapterConfig();
        }
        
        public TypeAdapterConfig CreateAdapterConfig()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<Core.Empresa, ViewModel>()
                .IgnoreNullValues(true);

            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var (unidades, total) = await _uow.UnidadeRepository.ListAllAsync(
                    page: request.Page,
                    pageSize: request.PageSize,
                    includes: [
                        e => e.Empresa,
                        e => e.UnidadesAcoes,
                        e => e.Consumos
                    ],
                    ct: ct
                );

            var data = unidades.Adapt<List<ViewModel>>(_config);
            
            return new PaginatedResponse<ViewModel>(data, request.Page, request.PageSize, total);
        }
    }
}