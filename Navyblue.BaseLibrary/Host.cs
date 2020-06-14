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
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Navyblue.BaseLibrary
{
    public static class HostServer
    {
        public static int CurrentManagedThreadId
        {
            get { return Environment.CurrentManagedThreadId; }
        }

        public static string IP
        {
            get { return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(); }
        }

        public static bool Is64BitOperatingSystem
        {
            get { return Environment.Is64BitOperatingSystem; }
        }

        public static bool Is64BitProcess
        {
            get { return Environment.Is64BitProcess; }
        }

        public static string MachineName
        {
            get { return Environment.MachineName; }
        }

        public static string OSVersion
        {
            get { return Environment.OSVersion.VersionString; }
        }

        public static int ProcessorCount
        {
            get { return Environment.ProcessorCount; }
        }

        public static string RuntimeVersion
        {
            get { return Environment.Version.ToString(); }
        }
    }
}