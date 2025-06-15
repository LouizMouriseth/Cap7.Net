using Application.SeedWork.Responses;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Empresa;

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
        public string Cnpj { get; private set; }
        public string Segmento { get; private set; }
        public ICollection<UnidadeViewModel> Unidades { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }

    public class UnidadeViewModel
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Endereco { get; private set; }
        public string Estado { get; private set; }
        public float Area { get; private set; }
        public DateTime InicioOperacao { get; private set; }
        public Guid IdEmpresa { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }

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
                .IgnoreNullValues(true)
                .Map(dest => dest.Cnpj, src => FormatterExtension.FormatToCnpj(src.Cnpj));

            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var (empresas, total) = await _uow.EmpresaRepository.ListAllAsync(
                page: request.Page,
                pageSize: request.PageSize,
                includes: [ e => e.Unidades ],
                ct: ct
            );

            var data = empresas.Adapt<List<ViewModel>>(_config);
            
            return new PaginatedResponse<ViewModel>(data, request.Page, request.PageSize, total);
        }
    }
}