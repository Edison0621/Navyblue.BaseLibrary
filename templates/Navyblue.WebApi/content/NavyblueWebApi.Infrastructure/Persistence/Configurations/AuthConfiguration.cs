// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : AuthConfiguration.cs
// Created          : 2026-07-13  10:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="AuthConfiguration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Infrastructure.Persistence.Configurations;

public sealed class AuthConfiguration : IEntityTypeConfiguration<Auth>
{
    #region IEntityTypeConfiguration<Auth> Members

    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        builder.ToTable("auths");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.Login).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Login).IsUnique();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Salt).HasMaxLength(256).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.PasswordChangedAt);
    }

    #endregion
}