// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : SnowflakeIdGenerator.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="SnowflakeIdGenerator.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Primitives;

/// <summary>
///     The id generator interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IIdGenerator<out T>
{
    /// <summary>
    ///     Nexts the id.
    /// </summary>
    /// <returns>
    ///     A T
    /// </returns>
    T NextId();
}

/// <summary>
///     The snowflake id generator.
/// </summary>
public sealed class SnowflakeIdGenerator : IIdGenerator<long>
{
    /// <summary>
    ///     The data center identifier bits
    /// </summary>
    private const int DATA_CENTER_ID_BITS = 5;

    /// <summary>
    ///     The data center identifier shift
    /// </summary>
    private const int DATA_CENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;

    /// <summary>
    ///     The maximum data center identifier
    /// </summary>
    private const long MAX_DATA_CENTER_ID = -1L ^ (-1L << DATA_CENTER_ID_BITS);

    /// <summary>
    ///     The maximum worker identifier
    /// </summary>
    private const long MAX_WORKER_ID = -1L ^ (-1L << WORKER_ID_BITS);

    /// <summary>
    ///     The sequence bits
    /// </summary>
    private const int SEQUENCE_BITS = 12;

    /// <summary>
    ///     The sequence mask
    /// </summary>
    private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);

    /// <summary>
    ///     The timestamp left shift
    /// </summary>
    private const int TIMESTAMP_LEFT_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS + DATA_CENTER_ID_BITS;

    /// <summary>
    ///     The twepoch
    /// </summary>
    private const long TWEPOCH = 1704067200000L;

    /// <summary>
    ///     The worker identifier bits
    /// </summary>
    private const int WORKER_ID_BITS = 5;

    /// <summary>
    ///     The worker identifier shift
    /// </summary>
    private const int WORKER_ID_SHIFT = SEQUENCE_BITS;

    /// <summary>
    ///     The data center identifier
    /// </summary>
    private readonly long _dataCenterId;

    /// <summary>
    ///     The lock
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    ///     The worker identifier
    /// </summary>
    private readonly long _workerId;

    /// <summary>
    ///     The last timestamp
    /// </summary>
    private long _lastTimestamp = -1L;

    /// <summary>
    ///     The sequence
    /// </summary>
    private long _sequence;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SnowflakeIdGenerator" /> class.
    /// </summary>
    /// <param name="workerId">The worker id.</param>
    /// <param name="dataCenterId">The data center id.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     workerId
    ///     or
    ///     dataCenterId
    /// </exception>
    public SnowflakeIdGenerator(long workerId = 0, long dataCenterId = 0)
    {
        if (workerId is < 0 or > MAX_WORKER_ID) throw new ArgumentOutOfRangeException(nameof(workerId));
        if (dataCenterId is < 0 or > MAX_DATA_CENTER_ID) throw new ArgumentOutOfRangeException(nameof(dataCenterId));
        this._workerId = workerId;
        this._dataCenterId = dataCenterId;
    }

    #region IIdGenerator<long> Members

    /// <summary>
    ///     Nexts the id.
    /// </summary>
    /// <returns>
    ///     A long
    /// </returns>
    /// <exception cref="InvalidOperationException">System clock moved backwards.</exception>
    public long NextId()
    {
        lock (this._lock)
        {
            long timestamp = CurrentTimeMillis();
            if (timestamp < this._lastTimestamp)
            {
                throw new InvalidOperationException("System clock moved backwards.");
            }

            if (this._lastTimestamp == timestamp)
            {
                this._sequence = (this._sequence + 1) & SEQUENCE_MASK;
                if (this._sequence == 0) timestamp = WaitNextMillis(this._lastTimestamp);
            }
            else
            {
                this._sequence = 0;
            }

            this._lastTimestamp = timestamp;
            return ((timestamp - TWEPOCH) << TIMESTAMP_LEFT_SHIFT) | (this._dataCenterId << DATA_CENTER_ID_SHIFT) | (this._workerId << WORKER_ID_SHIFT) | this._sequence;
        }
    }

    #endregion

    /// <summary>
    ///     Currents the time millis.
    /// </summary>
    /// <returns></returns>
    private static long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    ///     Waits the next millis.
    /// </summary>
    /// <param name="lastTimestamp">The last timestamp.</param>
    /// <returns></returns>
    private static long WaitNextMillis(long lastTimestamp)
    {
        long timestamp = CurrentTimeMillis();
        while (timestamp <= lastTimestamp) timestamp = CurrentTimeMillis();
        return timestamp;
    }
}