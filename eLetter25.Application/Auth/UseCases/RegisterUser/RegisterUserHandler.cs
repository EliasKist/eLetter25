using eLetter25.Application.Auth.Ports;
using eLetter25.Application.Common.Results;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.RegisterUser;

public sealed class RegisterUserHandler(IUserRegistrationService userRegistrationService)
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResult>>
{
    public async Task<Result<RegisterUserResult>> Handle(RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var userId = await userRegistrationService.RegisterUserAsync(
                request.Email,
                request.Password,
                request.EnableNotifications,
                cancellationToken);
            var result = new RegisterUserResult(userId, "User successfully registered");
            return Result<RegisterUserResult>.Success(result);
        }
        catch (InvalidOperationException ex)
        {
            var errorMessages = ex.Message.Split([", "], StringSplitOptions.RemoveEmptyEntries);
            var errors = errorMessages.Select(Error.Validation);
            return Result<RegisterUserResult>.Failure(errors);
        }
    }
}