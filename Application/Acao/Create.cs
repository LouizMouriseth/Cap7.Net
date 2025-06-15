using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Acao;

public class Create
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string descricao, string categoria)
        {
            Descricao = descricao;
            Categoria = categoria;
        }

        public string Descricao { get; private set; }
        public string Categoria { get; private set; }
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Descricao)
                .NotEmpty().WithMessage("Descrição está em branco");

            RuleFor(request => request.Categoria)
                .NotEmpty().WithMessage("Categoria está em branco");
        }
    }
    
    public class ViewModel
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; }
        public string Categoria { get; private set; }
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
            
            var acao = request.Adapt<Core.Acao>();

            await _uow.AcaoRepository.AddAsync(acao, ct);
            await _uow.CommitAsync(ct);

            var data = acao.Adapt<ViewModel>();

            return new DataResponse<ViewModel>(201, "Ação criada", data);
        }
    }
}