using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebPractice.Data.Models;

namespace WebPractice.Data.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<category> categories { get; set; }

    public virtual DbSet<expense> expenses { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("categories_pkey");

            entity.Property(e => e.category_name).HasMaxLength(50);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.description).HasMaxLength(50);
        });

        modelBuilder.Entity<expense>(entity =>
        {
            entity.HasKey(e => e.id).HasName("expenses_pkey");

            entity.HasIndex(e => e.category_id, "idx_category_id");

            entity.HasIndex(e => e.expense_date, "idx_expense_date");

            entity.Property(e => e.amount).HasPrecision(10, 2);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.category).WithMany(p => p.expenses)
                .HasForeignKey(d => d.category_id)
                .HasConstraintName("expenses_category_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.expenses)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("expenses_user_id_fkey");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.username, "users_username_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.email).HasMaxLength(50);
            entity.Property(e => e.username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
