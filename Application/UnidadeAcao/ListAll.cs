using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.UnidadeAcao;

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
        public DateTime DataImplantacao { get; private set; }
        public Guid IdUnidade { get; private set; }
        public Guid IdAcao { get; private set; }
        public UnidadeViewModel Unidade { get; private set; }
        public AcaoViewModel Acao { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
    
    #region Sub ViewModels

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

    public class AcaoViewModel
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; }
        public string Categoria { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
    
    #endregion

    internal class Service : IRequestHandler<Request, BaseResponse<ViewModel>>
    {
        private readonly IUnitOfWork _uow;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var (acoes, total) = await _uow.UnidadeAcaoRepository.ListAllAsync(
                page: request.Page,
                pageSize: request.PageSize,
                includes: [
                    e => e.Unidade,
                    e => e.Acao
                ],
                ct: ct
            );

            var data = acoes.Adapt<List<ViewModel>>();
            
            return new PaginatedResponse<ViewModel>(data, request.Page, request.PageSize, total);
        }
    }
}