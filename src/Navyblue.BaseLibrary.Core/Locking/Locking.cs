namespace Navyblue.BaseLibrary.Locking;

public interface IDistributedLock : IAsyncDisposable { string Name { get; } string Token { get; } bool IsAcquired { get; } }
public interface IDistributedLockProvider { ValueTask<IDistributedLock?> TryAcquireAsync(string name, TimeSpan expiry, TimeSpan waitTime, CancellationToken cancellationToken = default); }
public sealed class DistributedLockHandle(string name, string token, Func<ValueTask> release) : IDistributedLock { public string Name { get; } = name; public string Token { get; } = token; public bool IsAcquired { get; private set; } = true; public async ValueTask DisposeAsync() { if (!IsAcquired) return; IsAcquired = false; await release().ConfigureAwait(false); } }
