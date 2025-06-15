using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.UnidadeAcao;

public class Update
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(DateTime? dataImplantacao, Guid? idUnidade, Guid? idAcao)
        {
            DataImplantacao = dataImplantacao;
            IdUnidade = idUnidade;
            IdAcao = idAcao;
        }

        public Guid Id { get; private set; }
        public DateTime? DataImplantacao { get; private set; }
        public Guid? IdUnidade { get; private set; }
        public Guid? IdAcao { get; private set; }
        
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

            config.NewConfig<Request, Core.UnidadeAcao>()
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

            if (request.IdAcao != null)
            {
                var acaoExiste = await _uow.AcaoRepository.GetByIdAsync(request.IdAcao!.Value, ct);
                if (acaoExiste == null)
                {
                    return new NoDataResponse<ViewModel>(404, "Ação não encontrada");
                }
            }

            var unidadeAcaoAntiga = await _uow.UnidadeAcaoRepository.GetByIdAsync(request.Id, ct);
            
            if (unidadeAcaoAntiga == null)
                return new NoDataResponse<ViewModel>(404, "UnidadeAção não existe");

            request.Adapt(unidadeAcaoAntiga, _config);
            
            _uow.UnidadeAcaoRepository.Update(unidadeAcaoAntiga, ct);
            await _uow.CommitAsync(ct);
            
            var res = unidadeAcaoAntiga.Adapt<ViewModel>();
            
            return new DataResponse<ViewModel>("UnidadeAção atualizada", res);
        }
    }
}