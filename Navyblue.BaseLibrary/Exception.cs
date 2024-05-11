// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Exception.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:51
// *****************************************************************************************************************
// <copyright file="Exception.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions of <see cref="System.Exception" /> type.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Gets the exception string.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>System.String.</returns>
        public static string GetExceptionString(this Exception exception)
        {
            return CreateExceptionString(exception);
        }

        /// <summary>
        ///     Creates the exception string.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>System.String.</returns>
        private static string CreateExceptionString(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            CreateExceptionString(sb, exception, string.Empty);

            return sb.ToString();
        }

        /// <summary>
        ///     Creates the exception string. If the exception is null.
        ///     The exception string will be String.Empty.
        /// </summary>
        private static void CreateExceptionString(StringBuilder sb, Exception exception, string indent)
        {
            while (true)
            {
                if (exception == null)
                {
                    sb.Append(string.Empty);
                    return;
                }

                if (indent == null)
                {
                    indent = string.Empty;
                }
                else if (indent.Length > 0)
                {
                    sb.AppendFormat("{0}Inner ", indent);
                }

                sb.AppendLine("Exception(s) Found:");
                sb.AppendLine($"{indent}Type: {exception.GetType().FullName}");
                sb.AppendLine($"{indent}Message: {exception.Message}");
                sb.AppendLine($"{indent}Source: {exception.Source}");
                sb.AppendLine($"{indent}Stacktrace:");
                sb.AppendLine($"{exception.StackTrace}");

                switch (exception)
                {
                    case ReflectionTypeLoadException loadException:
                        Exception[] loaderExceptions = loadException.LoaderExceptions;
                        if (loaderExceptions.Length == 0)
                        {
                            sb.AppendLine($"{indent}No LoaderExceptions found.");
                        }
                        else
                        {
                            foreach (Exception e in loaderExceptions)
                                CreateExceptionString(sb, e, indent + "  ");
                        }

                        break;

                    case AggregateException aggregateException:
                        ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
                        if (innerExceptions.Count == 0)
                            sb.AppendLine($"{indent}No InnerExceptions found.");
                        else
                        {
                            foreach (Exception e in innerExceptions)
                                CreateExceptionString(sb, e, indent + "  ");
                        }

                        break;

                    default:
                        if (exception.InnerException != null)
                        {
                            sb.Append(Environment.NewLine);
                            exception = exception.InnerException;
                            indent += "  ";
                            continue;
                        }

                        break;
                }

                break;
            }
        }
    }
}