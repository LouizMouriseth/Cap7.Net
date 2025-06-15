using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Unidade;

public class Create
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string nome, string endereco, string estado, float area, DateTime inicioOperacao, Guid idEmpresa)
        {
            Nome = nome;
            Endereco = endereco;
            Estado = estado;
            Area = area;
            InicioOperacao = inicioOperacao;
            IdEmpresa = idEmpresa;
        }

        public string Nome { get; private set; }
        public string Endereco { get; private set; }
        public string Estado { get; private set; }
        public float Area { get; private set; }
        public DateTime InicioOperacao { get; private set; }
        public Guid IdEmpresa { get; private set; }
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Nome)
                .NotEmpty().WithMessage("Nome está em branco");

            RuleFor(request => request.Endereco)
                .NotEmpty().WithMessage("Endereco está em branco");

            RuleFor(request => request.Estado)
                .NotEmpty().WithMessage("Estado está em branco");

            RuleFor(request => request.Area)
                .NotEmpty().WithMessage("Area está em branco");

            RuleFor(request => request.InicioOperacao)
                .NotEmpty().WithMessage("Inicio da operação está em branco");
        }
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
            
            var unidade = request.Adapt<Core.Unidade>();
            
            await _uow.UnidadeRepository.AddAsync(unidade, ct);
            await _uow.CommitAsync(ct);

            var data = unidade.Adapt<ViewModel>();

            return new DataResponse<ViewModel>(201, "Unidade criada", data);
        }
    }
}