// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Host.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:52
// *****************************************************************************************************************
// <copyright file="Host.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Navyblue.BaseLibrary
{
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
}