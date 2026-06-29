namespace Navyblue.BaseLibrary.Idempotency;

public enum IdempotencyDecision
{
    Started,
    Replayed,
    Conflict
}

public sealed record IdempotencyResult(IdempotencyDecision Decision, string? ResponsePayload = null)
{
    public bool ShouldExecute => Decision == IdempotencyDecision.Started;
}

public interface IIdempotencyKeyProvider
{
    ValueTask<IdempotencyKey?> GetKeyAsync(CancellationToken cancellationToken = default);
}
