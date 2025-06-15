using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.UnidadeAcao;

public class Create
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(DateTime dataImplantacao, Guid idUnidade, Guid idAcao)
        {
            DataImplantacao = dataImplantacao;
            IdUnidade = idUnidade;
            IdAcao = idAcao;
        }

        public DateTime DataImplantacao { get; private set; }
        public Guid IdUnidade { get; private set; }
        public Guid IdAcao { get; private set; }
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.DataImplantacao)
                .NotEmpty().WithMessage("Data de implantação está em branco");

            RuleFor(request => request.IdUnidade)
                .NotEmpty().WithMessage("Id da unidade está em branco");

            RuleFor(request => request.IdAcao)
                .NotEmpty().WithMessage("Id da ação está em branco");
        }
    }
    
    public class ViewModel
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
            
            var unidadeAcao = request.Adapt<Core.UnidadeAcao>();

            await _uow.UnidadeAcaoRepository.AddAsync(unidadeAcao, ct);
            await _uow.CommitAsync(ct);

            var data = unidadeAcao.Adapt<ViewModel>();

            return new DataResponse<ViewModel>(201, "UnidadeAção criada", data);
        }
    }
}