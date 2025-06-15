using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Consumo;

public class Create
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(DateTime dataReferencia, float consumoTotal, string tipoFonte, bool eRenovavel, Guid idUnidade)
        {
            DataReferencia = dataReferencia;
            ConsumoTotal = consumoTotal;
            TipoFonte = tipoFonte;
            ERenovavel = eRenovavel;
            IdUnidade = idUnidade;
        }

        public DateTime DataReferencia { get; private set; }
        public float ConsumoTotal { get; private set; }
        public string TipoFonte { get; private set; }
        public bool ERenovavel { get; private set; }
        public Guid IdUnidade { get; private set; }
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.DataReferencia)
                .NotEmpty().WithMessage("Data de referência está em branco");

            RuleFor(request => request.ConsumoTotal)
                .NotEmpty().WithMessage("Consumo total está em branco");

            RuleFor(request => request.TipoFonte)
                .NotEmpty().WithMessage("Tipo de fonte está em branco");

            RuleFor(request => request.ERenovavel)
                .NotEmpty().WithMessage("Campo 'é renovável' está em branco");

            RuleFor(request => request.IdUnidade)
                .NotEmpty().WithMessage("Id da unidade está em branco");
        }
    }
    
    public class ViewModel
    {
        public Guid Id { get; private set; }
        public DateTime DataReferencia { get; private set; }
        public float ConsumoTotal { get; private set; }
        public string TipoFonte { get; private set; }
        public bool ERenovavel { get; private set; }
        public Guid IdUnidade { get; private set; }
        public UnidadeViewModel UnidadeViewModel { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
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
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
    
    internal class Service : IRequestHandler<Request, BaseResponse<ViewModel>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<Request> _validator;

        public Service(IUnitOfWork uow, IValidator<Request> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return new ErrorListResponse<ViewModel>(422, "Um ou mais campos estão incorretos", validationResult.Errors);
            
            var consumo = request.Adapt<Core.Consumo>();
            
            await _uow.ConsumoRepository.AddAsync(consumo, ct);
            await _uow.CommitAsync(ct);

            var data = consumo.Adapt<ViewModel>();

            return new DataResponse<ViewModel>(201, "Consumo criado", data);
        }
    }
}