namespace Navyblue.BaseLibrary.Idempotency;

public sealed record IdempotencyKey(string Value) { public static IdempotencyKey From(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Idempotency key cannot be empty.", nameof(value)) : new(value.Trim()); }
public enum IdempotencyState { Processing, Succeeded, Failed }
public sealed record IdempotencyRecord(IdempotencyKey Key, IdempotencyState State, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt, string? ResponsePayload = null);
public interface IIdempotencyStore { ValueTask<bool> TryBeginAsync(IdempotencyKey key, TimeSpan ttl, CancellationToken cancellationToken = default); ValueTask CompleteAsync(IdempotencyKey key, string? responsePayload = null, CancellationToken cancellationToken = default); ValueTask FailAsync(IdempotencyKey key, string? reason = null, CancellationToken cancellationToken = default); ValueTask<IdempotencyRecord?> GetAsync(IdempotencyKey key, CancellationToken cancellationToken = default); }
