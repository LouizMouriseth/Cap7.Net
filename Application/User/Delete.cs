using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using MediatR;

namespace Application.User;

public class Delete
{
    public class Request(Guid id) : IRequest<BaseResponse<Response>>
    {
        public Guid Id { get; private set; } = id;
    }

    public class Response
    {
    }

    internal class Service : IRequestHandler<Request, BaseResponse<Response>>
    {
        private readonly IUnitOfWork _uow;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BaseResponse<Response>> Handle(Request request, CancellationToken ct)
        {
            var user = await _uow.UserRepository.GetByIdAsync(request.Id, ct);
            
            if (user == null)
                return new NoDataResponse<Response>(404, "Usuário não encontrado");

            await _uow.UserRepository.DeleteAsync(request.Id, ct);
            await _uow.CommitAsync(ct);
            
            return new NoDataResponse<Response>("Usuário excluído");
        }
    }
}