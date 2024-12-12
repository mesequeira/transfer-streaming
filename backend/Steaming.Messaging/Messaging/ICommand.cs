using MediatR;
using Streaming.Result;

namespace Steaming.Messaging.Messaging;

/// <summary>
/// Interface to represent a command that can be handled as a transaction.
/// </summary>
public interface IBaseTransactionCommand;

/// <summary>
/// A wrapper interpretation of the <see cref="IRequest"/> useful to manage transactional command with a response without value (void).
/// </summary>
public interface ITransactionalCommand : IRequest<Result>, IBaseTransactionCommand;

/// <summary>
/// A wrapper interpretation of the <see cref="IRequest"/> useful to manage transactional command with a response with value.
/// </summary>
/// <typeparam name="TResponse">The type of the response value.</typeparam>
public interface ITransactionalCommand<TResponse>
    : IBaseTransactionCommand,
        IRequest<Result<TResponse>>;
