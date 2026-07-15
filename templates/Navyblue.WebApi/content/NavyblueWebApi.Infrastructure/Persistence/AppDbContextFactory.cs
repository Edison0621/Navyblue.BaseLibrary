// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : AppDbContextFactory.cs
// Created          : 2026-07-13  10:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="AppDbContextFactory.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NavyblueWebApi.Infrastructure.Persistence;

/// <summary>
///     Design-time factory for <c>dotnet ef migrations</c>.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    #region IDesignTimeDbContextFactory<AppDbContext> Members

    public AppDbContext CreateDbContext(string[] args)
    {
        string basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "NavyblueWebApi.Web"));
        if (!Directory.Exists(basePath))
            basePath = Directory.GetCurrentDirectory();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("Default")
                                  ?? "Server=127.0.0.1;Port=3306;Database=navyblue_webapi;User=root;Password=root;";

        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));

        return new AppDbContext(optionsBuilder.Options);
    }

    #endregion
}