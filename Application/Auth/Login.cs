using Application.SeedWork.Responses;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Options;
using Infrastructure.SeedWork.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Auth;

public class Login
{
    public class Request : IRequest<BaseResponse<Response>>
    {
        public Request(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is invalid");
        
            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one upper case letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lower case letter")
                .Matches("\\d").WithMessage("Password must contain at least one number digit")
                .Matches("[^\\w\\d ]").WithMessage("Password must contain at least one special character");
        }
    }

    public class Response
    {
        public Response(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }

    public class Service : IRequestHandler<Request, BaseResponse<Response>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<Request> _validator;
        private readonly AuthTokenOptions _authOptions;

        public Service(IUnitOfWork uow, IValidator<Request> validator, IOptionsSnapshot<AuthTokenOptions> authOptions)
        {
            _uow = uow;
            _validator = validator;
            _authOptions = authOptions.Value;
        }

        public async Task<BaseResponse<Response>> Handle(Request request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return new NoDataResponse<Response>(401, "E-mail ou senha incorretos");
            
            var user = await _uow.UserRepository.GetByEmailAsync(request.Email);
            
            if (user == null)
                return new NoDataResponse<Response>(401, "E-mail ou senha incorretos");

            if (!user.Password.VerifyHash(request.Password))
                return new NoDataResponse<Response>(401, "E-mail ou senha incorretos");
            
            var token = new Response(user.GenerateJwtToken(_authOptions));

            return new DataResponse<Response>(201, "Login successfully", token);
        }
    }
}