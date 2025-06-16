using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.Unidade;

public class LessEfficient
{
    public class Request : IRequest<BaseResponse<List<ViewModel>>>
    {
    }
    
    internal class Validator : AbstractValidator<Request>
    {
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
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Core.Consumo UltimoConsumo { get; private set; }
        public void SetUltimoConsumo(Core.Consumo consumo) => UltimoConsumo = consumo;
    }
    
    internal class Service : IRequestHandler<Request, BaseResponse<List<ViewModel>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<Request> _validator;

        public Service(IUnitOfWork uow, IValidator<Request> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<BaseResponse<List<ViewModel>>> Handle(Request request, CancellationToken ct)
        {
            var unidades = await _uow.UnidadeRepository.ListLessEfficient(ct);

            var data = unidades.Adapt<List<ViewModel>>();

            for (var i = 0; i < data.Count; i++)
                data[i].SetUltimoConsumo(unidades[i].Consumos.First());

            return new DataResponse<List<ViewModel>>(201, "Unidades menos eficientes listadas", data);
        }
    }
}