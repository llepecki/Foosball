using System.Collections.Generic;
using EventFlow.Aggregates.ExecutionResults;

namespace FoosballScore.Domain;

public class ErrorCodeFailedExecutionResult : FailedExecutionResult
{
    public static ErrorCodeFailedExecutionResult GameNotFound { get; } = new ErrorCodeFailedExecutionResult(DomainErrors.GameNotFound, new[] { "Game not found" });

    public static ErrorCodeFailedExecutionResult GameAlreadyExists { get; } = new ErrorCodeFailedExecutionResult(DomainErrors.GameAlreadyExists, new[] { "Game already exists" });

    public static ErrorCodeFailedExecutionResult GameAlreadyFinished { get; } = new ErrorCodeFailedExecutionResult(DomainErrors.GameAlreadyFinished, new[] { "Game already finished" });

    private ErrorCodeFailedExecutionResult(int errorCode, IEnumerable<string> errors) : base(errors)
    {
        ErrorCode = errorCode;
    }

    public int ErrorCode { get; }
}