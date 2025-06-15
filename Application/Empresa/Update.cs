using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Empresa;

public class Update
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string? nome, string? cnpj, string? segmento)
        {
            Nome = nome;
            Cnpj = cnpj;
            Segmento = segmento;
        }

        public Guid Id { get; private set; }
        public string? Nome { get; private set; }
        public string? Cnpj { get; private set; }
        public string? Segmento { get; private set; }
        
        public void SetId(Guid id) => Id = id;
        public void SetCnpj(string cnpj) => Cnpj = cnpj;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Cnpj)
                .NotEmpty().WithMessage("CNPJ está em branco")
                .Matches("^(\\d{2}\\.\\d{3}\\.\\d{3}\\/\\d{4}-\\d{2}|\\d{14})$").WithMessage("Formato do CNPJ inválido")
                .Must(ValidationExtension.IsValidCnpj).WithMessage("CNPJ inválido")
                .When(x => !string.IsNullOrEmpty(x.Cnpj));
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
        private readonly TypeAdapterConfig _config;

        public Service(IUnitOfWork uow, IValidator<Request> validator)
        {
            _uow = uow;
            _validator = validator;
            _config = CreateAdapterConfig();
        }

        public TypeAdapterConfig CreateAdapterConfig()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<Request, Core.Empresa>()
                .IgnoreNullValues(true)
                .Map(dest => dest.Cnpj, 
                    src => src.Cnpj != null ? 
                        FormatterExtension.RemoveNonNumericCharacters(src.Cnpj) : 
                        null);
            config.NewConfig<ViewModel, Core.Empresa>()
                .IgnoreNullValues(true)
                .Map(dest => dest.Cnpj, src => FormatterExtension.FormatToCnpj(src.Cnpj));

            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return new ErrorListResponse<ViewModel>("Um ou mais campos estão incorretos", validationResult.Errors);

            if (request.Cnpj != null)
            {
                var empresaExiste = await _uow.EmpresaRepository.GetByCnpjAsync(request.Cnpj!.RemoveNonNumericCharacters());
                if (empresaExiste != null)
                    return new NoDataResponse<ViewModel>(400, "Já existe uma empresa com esse CNPJ");
            }

            var empresaAntiga = await _uow.EmpresaRepository.GetByIdAsync(request.Id, ct);
            if (empresaAntiga == null)
                return new NoDataResponse<ViewModel>(404, "Já existe uma empresa com esse CNPJ");
            
            if (request.Cnpj != null)
                request.SetCnpj(request.Cnpj!.RemoveNonNumericCharacters());

            request.Adapt(empresaAntiga, _config);
            
            _uow.EmpresaRepository.Update(empresaAntiga, ct);
            await _uow.CommitAsync(ct);
            
            var res = empresaAntiga.Adapt<ViewModel>(_config);
            
            return new DataResponse<ViewModel>("Empresa atualizada", res);
        }
    }
}