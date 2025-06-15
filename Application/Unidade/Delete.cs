using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using MediatR;

namespace Application.Unidade;

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
            var unidade = await _uow.UnidadeRepository.GetByIdAsync(request.Id, ct);
            
            if (unidade == null)
                return new NoDataResponse<ViewModel>(404, "Unidade não existe");

            await _uow.UnidadeRepository.DeleteAsync(request.Id, ct);
            await _uow.CommitAsync(ct);
            
        return new NoDataResponse<ViewModel>("Unidade excluída");
        }
    }
}