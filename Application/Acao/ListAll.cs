using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Acao;

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
        public string Descricao { get; private set; }
        public string Categoria { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public ICollection<UnidadeAcaoViewModel> UnidadesAcoes { get; private set; }
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

    internal class Service : IRequestHandler<Request, BaseResponse<ViewModel>>
    {
        private readonly IUnitOfWork _uow;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var (acoes, total) = await _uow.AcaoRepository.ListAllAsync(
                page: request.Page,
                pageSize: request.PageSize,
                includes: [ e => e.UnidadesAcoes ],
                ct: ct
            );

            var data = acoes.Adapt<List<ViewModel>>();
            
            return new PaginatedResponse<ViewModel>(data, request.Page, request.PageSize, total);
        }
    }
}