﻿// <auto-generated />
using System;
using JournalyApiV2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    [DbContext(typeof(JournalyDbContext))]
    [Migration("20231018212510_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("JournalyApiV2.Data.Models.Emotion", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

                    b.Property<Guid>("CategoryUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("CategoryUuid");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean")
                        .HasColumnName("Deleted");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Icon");

                    b.Property<short>("IconTypeId")
                        .HasColumnType("smallint")
                        .HasColumnName("IconTypeId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<short>("Order")
                        .HasColumnType("smallint")
                        .HasColumnName("Order");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uuid")
                        .HasColumnName("Owner");

                    b.HasKey("Uuid");

                    b.HasIndex("CategoryUuid");

                    b.HasIndex("IconTypeId");

                    b.ToTable("Emotion");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.EmotionCategory", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

                    b.Property<bool>("AllowMultiple")
                        .HasColumnType("boolean")
                        .HasColumnName("AllowMultiple");

                    b.Property<bool>("Default")
                        .HasColumnType("boolean")
                        .HasColumnName("Default");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean")
                        .HasColumnName("Deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<short>("Order")
                        .HasColumnType("smallint")
                        .HasColumnName("Order");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uuid")
                        .HasColumnName("Owner");

                    b.HasKey("Uuid");

                    b.ToTable("EmotionCategory");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.IconType", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("IconType");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Emotion", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.EmotionCategory", "EmotionCategory")
                        .WithMany("Emotions")
                        .HasForeignKey("CategoryUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.IconType", "IconType")
                        .WithMany("Emotions")
                        .HasForeignKey("IconTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmotionCategory");

                    b.Navigation("IconType");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.EmotionCategory", b =>
                {
                    b.Navigation("Emotions");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.IconType", b =>
                {
                    b.Navigation("Emotions");
                });
#pragma warning restore 612, 618
        }
    }
}