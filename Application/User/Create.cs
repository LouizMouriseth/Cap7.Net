using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.SeedWork.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.User;

public class Create
{
    public class Request : IRequest<BaseResponse<Response>>
    {
        public Request(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
    }
    
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Username)
                .NotEmpty().WithMessage("Username é necessário");
        
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email é necessário")
                .EmailAddress().WithMessage("Email é inválido");
        
            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Senha é necessária")
                .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres")
                .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
                .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
                .Matches("\\d").WithMessage("Senha deve conter pelo menos um dígito numérico")
                .Matches("[^\\w\\d ]").WithMessage("Senha deve conter pelo menos um caractere especial");
        }
    }
    
    public class Response
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
    }
    
    internal class Service : IRequestHandler<Request, BaseResponse<Response>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<Request> _validator;

        public Service(IUnitOfWork uow, IValidator<Request> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<BaseResponse<Response>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return new ErrorListResponse<Response>(422, "Uma ou main entradas são inválidas", validationResult.Errors);
            
            var user = request.Adapt<Core.User>();

            user.SetPassword(user.Password.HashPassword());

            await _uow.UserRepository.AddAsync(user, ct);
            await _uow.CommitAsync(ct);

            var data = user.Adapt<Response>();

            return new DataResponse<Response>(201, "Usuário criado", data);
        }
    }
}