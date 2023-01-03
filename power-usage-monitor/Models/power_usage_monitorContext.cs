using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace power_usage_monitor.Models
{
    public partial class power_usage_monitorContext : DbContext
    {
        public power_usage_monitorContext()
        {
        }

        public power_usage_monitorContext(DbContextOptions<power_usage_monitorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Abnormal> Abnormals { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Device> Devices { get; set; } = null!;
        public virtual DbSet<Statistic> Statistics { get; set; } = null!;
        public virtual DbSet<Usage> Usages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Abnormal>(entity =>
            {
                entity.ToTable("Abnormal");

                entity.Property(e => e.AbnormalId).HasColumnName("Abnormal_ID");

                entity.Property(e => e.AbnormalTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Abnormal_Time");

                entity.Property(e => e.AbnormalUsage).HasColumnName("Abnormal_Usage");

                entity.Property(e => e.DeviceId).HasColumnName("Device_ID");

                entity.Property(e => e.NoticedUser)
                    .HasMaxLength(50)
                    .HasColumnName("Noticed_User");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Abnormals)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_Abnormal_Device");

                entity.HasOne(d => d.NoticedUserNavigation)
                    .WithMany(p => p.Abnormals)
                    .HasForeignKey(d => d.NoticedUser)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Abnormal_User");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("Category_ID");

                entity.Property(e => e.CategoryAvgPower).HasColumnName("Category_Avg_Power");

                entity.Property(e => e.DeviceCategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("Device_Category_Name");

                entity.Property(e => e.EngneryName)
                    .HasMaxLength(50)
                    .HasColumnName("Engnery_Name");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.DeviceId)
                    .ValueGeneratedNever()
                    .HasColumnName("Device_ID");

                entity.Property(e => e.CategoryId).HasColumnName("Category_ID");

                entity.Property(e => e.DeviceName)
                    .HasMaxLength(50)
                    .HasColumnName("Device_Name");

                entity.Property(e => e.StandbyTime).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UseTime).HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Device_Category");
            });

            modelBuilder.Entity<Statistic>(entity =>
            {
                entity.HasKey(e => new { e.DeviceId, e.Period });

                entity.Property(e => e.DeviceId).HasColumnName("Device_ID");

                entity.Property(e => e.Period).HasMaxLength(50);

                entity.Property(e => e.TotalUsage).HasColumnName("Total_Usage");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Statistics)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_Statistics_Device");
            });

            modelBuilder.Entity<Usage>(entity =>
            {
                entity.HasKey(e => new { e.Time, e.DeviceId });

                entity.ToTable("Usage");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.DeviceId).HasColumnName("Device_ID");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Usages)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_Usage_Device");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserName);

                entity.ToTable("User");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("User_Name");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(50)
                    .HasColumnName("User_Email");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
