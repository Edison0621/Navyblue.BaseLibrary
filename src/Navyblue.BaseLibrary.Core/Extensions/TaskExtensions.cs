namespace Navyblue.BaseLibrary.Extensions;

public static class TaskExtensions
{
    public static async Task<T> WaitAsync<T>(this Task<T> task, TimeSpan timeout, string? timeoutMessage = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            return await task.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!task.IsCompleted)
        {
            throw new TimeoutException(timeoutMessage ?? $"The operation timed out after {timeout}.");
        }
    }

    public static async Task WithTimeout(this Task task, TimeSpan timeout, string? timeoutMessage = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            await task.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!task.IsCompleted)
        {
            throw new TimeoutException(timeoutMessage ?? $"The operation timed out after {timeout}.");
        }
    }

    public static async Task IgnoreExceptionAsync(this Task task, Action<Exception>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
        }
    }

    public static async Task<IReadOnlyList<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
