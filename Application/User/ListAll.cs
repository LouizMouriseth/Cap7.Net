using Application.SeedWork.Responses;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.User;

public class ListAll
{
    public class Request : IRequest<BaseResponse<Response>>
    {
        
    }
    
    public class Response
    {
        public string Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
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
            var users = (await _uow.UserRepository.ListAllAsync(ct: ct)).data;

            var data = users.Adapt<List<Response>>();
            
            return new PaginatedResponse<Response>(data);
        }
    }
}