namespace Navyblue.BaseLibrary.Resilience;

public static class Retry
{
    public static async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, int attempts = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        Exception? last = null;
        for (var i = 1; i <= attempts; i++)
        {
            try { return await operation(cancellationToken).ConfigureAwait(false); }
            catch (Exception ex) when (i < attempts) { last = ex; await Task.Delay(delay ?? TimeSpan.FromMilliseconds(100 * i), cancellationToken).ConfigureAwait(false); }
        }
        throw last ?? new InvalidOperationException("Retry failed.");
    }
}
