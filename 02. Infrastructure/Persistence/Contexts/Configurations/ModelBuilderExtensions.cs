﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts.Configurations;

public static class ModelBuilderExtensions
{
    public static void ConfigureModels(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("noyankes_TestEf");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07EA876776");

            entity.ToTable("Categories", "General");

            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Images", "General");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ImageName).HasMaxLength(100);

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK__Images__IdProduc__3F466844");
        });

        modelBuilder.Entity<Personel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personel__3214EC07DF398778");

            entity.ToTable("Personel", "Security");

            entity.Property(e => e.FirstName).HasMaxLength(400);
            entity.Property(e => e.LastName).HasMaxLength(400);
            entity.Property(e => e.NationalCode).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", "General");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(400);

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__IdCate__3C69FB99");
        });

        modelBuilder.Entity<RabbitMq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RabbitMQ__3214EC07790FE78F");

            entity.ToTable("RabbitMQ", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(38, 0)");
            entity.Property(e => e.Body).HasMaxLength(4000);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(350);
            entity.Property(e => e.ToEmail).HasMaxLength(350);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Tokens", "Security");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Idconnection).HasMaxLength(200);
            entity.Property(e => e.Ip).HasMaxLength(50);
            entity.Property(e => e.Token1).HasColumnName("Token");

            entity.HasOne(d => d.IdPersonelNavigation).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.IdPersonel)
                .HasConstraintName("FK_Tokens_Personel");
        });

    }
}