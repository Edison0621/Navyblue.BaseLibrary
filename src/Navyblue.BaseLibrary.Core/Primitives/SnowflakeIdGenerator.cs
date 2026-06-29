namespace Navyblue.BaseLibrary.Primitives;

public interface IIdGenerator<out T>
{
    T NextId();
}

public sealed class SnowflakeIdGenerator : IIdGenerator<long>
{
    private const long Twepoch = 1704067200000L;
    private const int WorkerIdBits = 5;
    private const int DataCenterIdBits = 5;
    private const int SequenceBits = 12;
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDataCenterId = -1L ^ (-1L << DataCenterIdBits);
    private const int WorkerIdShift = SequenceBits;
    private const int DataCenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DataCenterIdBits;
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    private readonly object _lock = new();
    private readonly long _workerId;
    private readonly long _dataCenterId;
    private long _sequence;
    private long _lastTimestamp = -1L;

    public SnowflakeIdGenerator(long workerId = 0, long dataCenterId = 0)
    {
        if (workerId is < 0 or > MaxWorkerId) throw new ArgumentOutOfRangeException(nameof(workerId));
        if (dataCenterId is < 0 or > MaxDataCenterId) throw new ArgumentOutOfRangeException(nameof(dataCenterId));
        _workerId = workerId;
        _dataCenterId = dataCenterId;
    }

    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = CurrentTimeMillis();
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException("System clock moved backwards.");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0) timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            return ((timestamp - Twepoch) << TimestampLeftShift) | (_dataCenterId << DataCenterIdShift) | (_workerId << WorkerIdShift) | _sequence;
        }
    }

    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = CurrentTimeMillis();
        while (timestamp <= lastTimestamp) timestamp = CurrentTimeMillis();
        return timestamp;
    }

    private static long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
