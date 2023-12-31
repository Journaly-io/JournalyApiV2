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
    [Migration("20231022163536_11")]
    partial class _10
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("JournalyApiV2.Data.Models.Activity", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

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

                    b.HasIndex("IconTypeId");

                    b.ToTable("Activity");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.ActivityEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ActivityUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("ActivityUuid");

                    b.Property<Guid>("JournalEntryUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("JournalEntryUuid");

                    b.HasKey("Id");

                    b.HasIndex("ActivityUuid");

                    b.HasIndex("JournalEntryUuid");

                    b.ToTable("ActivityEntry");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Day", b =>
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

                    b.ToTable("Day");
                });

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

            modelBuilder.Entity("JournalyApiV2.Data.Models.EmotionEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("EmotionUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("EmotionUuid");

                    b.Property<Guid>("JournalEntryUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("JournalEntryUuid");

                    b.HasKey("Id");

                    b.HasIndex("EmotionUuid");

                    b.HasIndex("JournalEntryUuid");

                    b.ToTable("EmotionEntry");
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

            modelBuilder.Entity("JournalyApiV2.Data.Models.JournalEntry", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Body");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean")
                        .HasColumnName("Deleted");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uuid")
                        .HasColumnName("Owner");

                    b.HasKey("Uuid");

                    b.ToTable("JournalEntry");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.JournalEntryCategoryValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("CategoryUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("CategoryUuid");

                    b.Property<Guid>("JournalEntryUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("JournalEntryUuid");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("Value");

                    b.HasKey("Id");

                    b.HasIndex("CategoryUuid");

                    b.HasIndex("JournalEntryUuid");

                    b.ToTable("JournalEntryCategoryValue");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("EveryOtherDay")
                        .HasColumnType("boolean")
                        .HasColumnName("EveryOtherDay");

                    b.Property<Guid>("MedicationUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("MedicationUuid");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time without time zone")
                        .HasColumnName("Time");

                    b.HasKey("Id");

                    b.HasIndex("MedicationUuid");

                    b.ToTable("MedSchedule");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedScheduleDays", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<short>("DayId")
                        .HasColumnType("smallint")
                        .HasColumnName("Day");

                    b.Property<int>("MedScheduleId")
                        .HasColumnType("integer")
                        .HasColumnName("MedScheduleId");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.HasIndex("MedScheduleId");

                    b.ToTable("MedScheduleDays");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedStatus", b =>
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

                    b.ToTable("MedStatus");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedUnit", b =>
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

                    b.ToTable("MedUnit");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Medication", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

                    b.Property<short>("DefaultDose")
                        .HasColumnType("smallint")
                        .HasColumnName("DefaultDose");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean")
                        .HasColumnName("Deleted");

                    b.Property<bool>("Forever")
                        .HasColumnType("boolean")
                        .HasColumnName("Forever");

                    b.Property<DateOnly>("FromDate")
                        .HasColumnType("date")
                        .HasColumnName("From");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Notes");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uuid")
                        .HasColumnName("Owner");

                    b.Property<short>("Unit")
                        .HasColumnType("smallint")
                        .HasColumnName("Unit");

                    b.Property<DateOnly>("UntilDate")
                        .HasColumnType("date")
                        .HasColumnName("Until");

                    b.HasKey("Uuid");

                    b.HasIndex("Unit");

                    b.ToTable("Medication");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedicationInstance", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Uuid");

                    b.Property<DateTime?>("ActualTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("ActualTime");

                    b.Property<short>("Dose")
                        .HasColumnType("smallint")
                        .HasColumnName("Dose");

                    b.Property<short>("MedStatus")
                        .HasColumnType("smallint");

                    b.Property<Guid>("MedicationUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("MedicationUuid");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ScheduleUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("ScheduleUuid");

                    b.Property<DateTime?>("ScheduledTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("ScheduledTime");

                    b.HasKey("Uuid");

                    b.HasIndex("MedStatus");

                    b.HasIndex("MedicationUuid");

                    b.HasIndex("ScheduleId");

                    b.ToTable("MedicationInstance");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.RecordType", b =>
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

                    b.ToTable("RecordType");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.SyncedRecords", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid")
                        .HasColumnName("DeviceId");

                    b.Property<bool>("IsVoid")
                        .HasColumnType("boolean")
                        .HasColumnName("IsVoid");

                    b.Property<Guid>("RecordId")
                        .HasColumnType("uuid")
                        .HasColumnName("RecordId");

                    b.Property<int>("RecordType")
                        .HasColumnType("integer")
                        .HasColumnName("RecordTypeId");

                    b.Property<short?>("RecordTypeId")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("RecordTypeId");

                    b.ToTable("SyncedRecords", t =>
                        {
                            t.Property("RecordTypeId")
                                .HasColumnName("RecordTypeId");
                        });
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Activity", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.IconType", "IconType")
                        .WithMany("Activities")
                        .HasForeignKey("IconTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IconType");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.ActivityEntry", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.Activity", "Activity")
                        .WithMany("ActivityEntries")
                        .HasForeignKey("ActivityUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.JournalEntry", "JournalEntry")
                        .WithMany("ActivityEntries")
                        .HasForeignKey("JournalEntryUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("JournalEntry");
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

            modelBuilder.Entity("JournalyApiV2.Data.Models.EmotionEntry", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.Emotion", "Emotion")
                        .WithMany("EmotionEntries")
                        .HasForeignKey("EmotionUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.JournalEntry", "JournalEntry")
                        .WithMany("EmotionEntries")
                        .HasForeignKey("JournalEntryUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Emotion");

                    b.Navigation("JournalEntry");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.JournalEntryCategoryValue", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.EmotionCategory", "EmotionCategory")
                        .WithMany("JournalEntryCategoryValues")
                        .HasForeignKey("CategoryUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.JournalEntry", "JournalEntry")
                        .WithMany("JournalEntryCategoryValues")
                        .HasForeignKey("JournalEntryUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmotionCategory");

                    b.Navigation("JournalEntry");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedSchedule", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.Medication", "Medication")
                        .WithMany("MedSchedules")
                        .HasForeignKey("MedicationUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medication");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedScheduleDays", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.Day", "Day")
                        .WithMany("MedScheduleDays")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.MedSchedule", "MedSchedule")
                        .WithMany("Days")
                        .HasForeignKey("MedScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");

                    b.Navigation("MedSchedule");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Medication", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.MedUnit", "MedUnit")
                        .WithMany("Medications")
                        .HasForeignKey("Unit")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MedUnit");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedicationInstance", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.MedStatus", "Status")
                        .WithMany()
                        .HasForeignKey("MedStatus")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.Medication", "Medication")
                        .WithMany("Instances")
                        .HasForeignKey("MedicationUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JournalyApiV2.Data.Models.MedSchedule", "Schedule")
                        .WithMany("Instances")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medication");

                    b.Navigation("Schedule");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.SyncedRecords", b =>
                {
                    b.HasOne("JournalyApiV2.Data.Models.RecordType", null)
                        .WithMany("SyncedRecords")
                        .HasForeignKey("RecordTypeId");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Activity", b =>
                {
                    b.Navigation("ActivityEntries");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Day", b =>
                {
                    b.Navigation("MedScheduleDays");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Emotion", b =>
                {
                    b.Navigation("EmotionEntries");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.EmotionCategory", b =>
                {
                    b.Navigation("Emotions");

                    b.Navigation("JournalEntryCategoryValues");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.IconType", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Emotions");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.JournalEntry", b =>
                {
                    b.Navigation("ActivityEntries");

                    b.Navigation("EmotionEntries");

                    b.Navigation("JournalEntryCategoryValues");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedSchedule", b =>
                {
                    b.Navigation("Days");

                    b.Navigation("Instances");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.MedUnit", b =>
                {
                    b.Navigation("Medications");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.Medication", b =>
                {
                    b.Navigation("Instances");

                    b.Navigation("MedSchedules");
                });

            modelBuilder.Entity("JournalyApiV2.Data.Models.RecordType", b =>
                {
                    b.Navigation("SyncedRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
