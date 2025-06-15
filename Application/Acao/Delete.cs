using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using MediatR;

namespace Application.Acao;

public class Delete
{
    public class Request(Guid id) : IRequest<BaseResponse<ViewModel>>
    {
        public Guid Id { get; private set; } = id;
    }

    public class ViewModel
    {
    }

    internal class Service : IRequestHandler<Request, BaseResponse<ViewModel>>
    {
        private readonly IUnitOfWork _uow;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BaseResponse<ViewModel>> Handle(Request request, CancellationToken ct)
        {
            var acao = await _uow.AcaoRepository.GetByIdAsync(request.Id, ct);
            
            if (acao == null)
                return new NoDataResponse<ViewModel>(404, "Ação não existe");

            await _uow.AcaoRepository.DeleteAsync(request.Id, ct);
            await _uow.CommitAsync(ct);
            
        return new NoDataResponse<ViewModel>("Ação excluída");
        }
    }
}