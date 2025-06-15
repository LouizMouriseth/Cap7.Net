using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Unidade;

public class Update
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string? nome, string? endereco, string? estado, float? area, DateTime? inicioOperacao, Guid? idEmpresa)
        {
            Nome = nome;
            Endereco = endereco;
            Estado = estado;
            Area = area;
            InicioOperacao = inicioOperacao;
            IdEmpresa = idEmpresa;
        }

        public Guid Id { get; private set; }
        public string? Nome { get; private set; }
        public string? Endereco { get; private set; }
        public string? Estado { get; private set; }
        public float? Area { get; private set; }
        public DateTime? InicioOperacao { get; private set; }
        public Guid? IdEmpresa { get; private set; }
        
        public void SetId(Guid id) => Id = id;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
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

            config.NewConfig<Request, Core.Unidade>()
                .IgnoreNullValues(true);
            
            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return new ErrorListResponse<ViewModel>("Um ou mais campos estão incorretos", validationResult.Errors);

            if (request.IdEmpresa != null)
            {
                var empresaExiste = await _uow.EmpresaRepository.GetByIdAsync(request.IdEmpresa!.Value, ct);
                if (empresaExiste == null)
                {
                    return new NoDataResponse<ViewModel>(404, "Empresa não encontrada");
                }
            }
            
            var unidadeAntiga = await _uow.UnidadeRepository.GetByIdAsync(request.Id, ct);
            if (unidadeAntiga == null)
                return new NoDataResponse<ViewModel>(404, "Já existe uma unidade com esse CNPJ");

            request.Adapt(unidadeAntiga, _config);
            
            _uow.UnidadeRepository.Update(unidadeAntiga, ct);
            await _uow.CommitAsync(ct);
            
            var res = unidadeAntiga.Adapt<ViewModel>(_config);
            
            return new DataResponse<ViewModel>("Unidade atualizada", res);
        }
    }
}