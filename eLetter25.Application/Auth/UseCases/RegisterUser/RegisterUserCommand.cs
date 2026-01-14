using eLetter25.Application.Auth.Contracts;
using eLetter25.Application.Common.Results;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.RegisterUser;

public sealed record RegisterUserCommand(RegisterUserRequest Request)
    : IRequest<Result<RegisterUserResult>>;