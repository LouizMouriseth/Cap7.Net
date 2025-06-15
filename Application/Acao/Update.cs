using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Acao;

public class Update
{
    public class Request : IRequest<BaseResponse<ViewModel>>
    {
        public Request(string? descricao, string? categoria)
        {
            Descricao = descricao;
            Categoria = categoria;
        }

        public Guid Id { get; private set; }
        public string? Descricao { get; private set; }
        public string? Categoria { get; private set; }
        
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
        public string Descricao { get; private set; }
        public string Categoria { get; private set; }
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

            config.NewConfig<Request, Core.Acao>()
                .IgnoreNullValues(true);

            return config;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return new ErrorListResponse<ViewModel>("Um ou mais campos estão incorretos", validationResult.Errors);

            var acaoAntiga = await _uow.AcaoRepository.GetByIdAsync(request.Id, ct);
            
            if (acaoAntiga == null)
                return new NoDataResponse<ViewModel>(404, "Ação não existe");

            request.Adapt(acaoAntiga, _config);
            
            _uow.AcaoRepository.Update(acaoAntiga, ct);
            await _uow.CommitAsync(ct);
            
            var res = acaoAntiga.Adapt<ViewModel>();
            
            return new DataResponse<ViewModel>("Ação atualizada", res);
        }
    }
}