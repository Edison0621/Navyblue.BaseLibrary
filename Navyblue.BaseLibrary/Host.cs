// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Host.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="Host.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Navyblue.BaseLibrary;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class HostServer
{
    public static int CurrentManagedThreadId => Environment.CurrentManagedThreadId;

    public static string IP
    {
        get { return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(); }
    }

    public static bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;

    public static bool Is64BitProcess => Environment.Is64BitProcess;

    public static string MachineName => Environment.MachineName;

    public static string OSVersion => Environment.OSVersion.VersionString;

    public static int ProcessorCount => Environment.ProcessorCount;

    public static string RuntimeVersion => Environment.Version.ToString();
}