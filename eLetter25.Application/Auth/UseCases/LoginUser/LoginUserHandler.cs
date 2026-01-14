using eLetter25.Application.Auth.Ports;
using eLetter25.Application.Common.Results;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.LoginUser;

public sealed class LoginUserHandler(
    IUserAuthenticationService userAuthenticationService,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginUserCommand, Result<LoginUserResult>>
{
    public async Task<Result<LoginUserResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var credentials = await userAuthenticationService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);
        if (credentials is null)
        {
            return Result<LoginUserResult>.Failure(Error.Unauthorized("Invalid email or password"));
        }

        var (userId, email, roles) = credentials.Value;
        var token = jwtTokenGenerator.GenerateToken(userId, email, roles);
        var result = new LoginUserResult(token);
        return Result<LoginUserResult>.Success(result);
    }
}