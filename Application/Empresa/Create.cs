using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Empresa;

public class Create
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string nome, string cnpj, string segmento)
        {
            Nome = nome;
            Cnpj = cnpj;
            Segmento = segmento;
        }

        public string Nome { get; private set; }
        public string Cnpj { get; private set; }
        public string Segmento { get; private set; }

        public void SetCnpj(string cnpj) => Cnpj = cnpj;
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Nome)
                .NotEmpty().WithMessage("Nome está em branco");

            RuleFor(request => request.Segmento)
                .NotEmpty().WithMessage("Segmento está em branco");

            RuleFor(request => request.Cnpj)
                .NotEmpty().WithMessage("CNPJ está em branco")
                .Matches("^(\\d{2}\\.\\d{3}\\.\\d{3}\\/\\d{4}-\\d{2}|\\d{14})$").WithMessage("Formato do CNPJ inválido")
                .Must(ValidationExtension.IsValidCnpj).WithMessage("CNPJ inválido");
        }
    }
    
    public class ViewModel
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Cnpj { get; private set; }
        public string Segmento { get; private set; }
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
            
            request.SetCnpj(request.Cnpj.RemoveNonNumericCharacters());

            var empresa = request.Adapt<Core.Empresa>();

            var empresaExiste = await _uow.EmpresaRepository.GetByCnpjAsync(empresa.Cnpj);
            if (empresaExiste != null)
                return new NoDataResponse<ViewModel>(404, "Já existe uma empresa com esse CNPJ");

            await _uow.EmpresaRepository.AddAsync(empresa, ct);
            await _uow.CommitAsync(ct);

            var data = empresa.Adapt<ViewModel>();

            return new DataResponse<ViewModel>(201, "Empresa criada", data);
        }
    }
}