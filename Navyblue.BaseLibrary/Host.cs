// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Host.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Host.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Net;
using System.Net.Sockets;

namespace Navyblue.BaseLibrary;

/// <summary>
/// </summary>
public static class HostServer
{
    /// <summary>
    ///     Gets the current managed thread identifier.
    /// </summary>
    /// <value>
    ///     The current managed thread identifier.
    /// </value>
    public static int CurrentManagedThreadId => Environment.CurrentManagedThreadId;

    /// <summary>
    ///     Gets the ip.
    /// </summary>
    /// <value>
    ///     The ip.
    /// </value>
    public static string IP
    {
        get { return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(); }
    }

    /// <summary>
    ///     Gets a value indicating whether [is64 bit operating system].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [is64 bit operating system]; otherwise, <c>false</c>.
    /// </value>
    public static bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;

    /// <summary>
    ///     Gets a value indicating whether [is64 bit process].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [is64 bit process]; otherwise, <c>false</c>.
    /// </value>
    public static bool Is64BitProcess => Environment.Is64BitProcess;

    /// <summary>
    ///     Gets the name of the machine.
    /// </summary>
    /// <value>
    ///     The name of the machine.
    /// </value>
    public static string MachineName => Environment.MachineName;

    /// <summary>
    ///     Gets the os version.
    /// </summary>
    /// <value>
    ///     The os version.
    /// </value>
    public static string OsVersion => Environment.OSVersion.VersionString;

    /// <summary>
    ///     Gets the processor count.
    /// </summary>
    /// <value>
    ///     The processor count.
    /// </value>
    public static int ProcessorCount => Environment.ProcessorCount;

    /// <summary>
    ///     Gets the runtime version.
    /// </summary>
    /// <value>
    ///     The runtime version.
    /// </value>
    public static string RuntimeVersion => Environment.Version.ToString();
}