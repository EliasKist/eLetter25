using eLetter25.Application.Auth.Contracts;
using eLetter25.Application.Common.Results;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.LoginUser;

public sealed record LoginUserCommand(LoginUserRequest Request)
    : IRequest<Result<LoginUserResult>>;