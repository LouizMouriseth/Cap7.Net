using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Consumo;

public class Update
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(DateTime? dataReferencia, float? consumoTotal, string? tipoFonte, bool? eRenovavel, Guid? idUnidade)
        {
            DataReferencia = dataReferencia;
            ConsumoTotal = consumoTotal;
            TipoFonte = tipoFonte;
            ERenovavel = eRenovavel;
            IdUnidade = idUnidade;
        }

        public Guid Id { get; private set; }
        public DateTime? DataReferencia { get; private set; }
        public float? ConsumoTotal { get; private set; }
        public string? TipoFonte { get; private set; }
        public bool? ERenovavel { get; private set; }
        public Guid? IdUnidade { get; private set; }
        
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
        public DateTime DataReferencia { get; private set; }
        public float ConsumoTotal { get; private set; }
        public string TipoFonte { get; private set; }
        public bool ERenovavel { get; private set; }
        public Guid IdUnidade { get; private set; }
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

            config.NewConfig<Request, Core.Consumo>()
                .IgnoreNullValues(true);
            
            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return new ErrorListResponse<ViewModel>("Um ou mais campos estão incorretos", validationResult.Errors);
            
            if (request.IdUnidade != null)
            {
                var unidadeExiste = await _uow.UnidadeRepository.GetByIdAsync(request.IdUnidade!.Value, ct);
                if (unidadeExiste == null)
                {
                    return new NoDataResponse<ViewModel>(404, "Unidade não encontrada");
                }
            }

            var consumoAntiga = await _uow.ConsumoRepository.GetByIdAsync(request.Id, ct);
            if (consumoAntiga == null)
                return new NoDataResponse<ViewModel>(404, "Já existe um consumo com esse CNPJ");

            request.Adapt(consumoAntiga, _config);
            
            _uow.ConsumoRepository.Update(consumoAntiga, ct);
            await _uow.CommitAsync(ct);
            
            var res = consumoAntiga.Adapt<ViewModel>(_config);
            
            return new DataResponse<ViewModel>("Consumo atualizado", res);
        }
    }
}