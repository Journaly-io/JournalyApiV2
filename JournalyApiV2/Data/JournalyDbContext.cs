﻿using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Activity = JournalyApiV2.Data.Models.Activity;
using Emotion = JournalyApiV2.Data.Models.Emotion;
using EmotionCategory = JournalyApiV2.Data.Models.EmotionCategory;
using JournalEntry = JournalyApiV2.Data.Models.JournalEntry;
using Medication = JournalyApiV2.Data.Models.Medication;
using MedStatus = JournalyApiV2.Data.Enums.MedStatus;
using RecordType = JournalyApiV2.Data.Enums.RecordType;

namespace JournalyApiV2.Data;

public class JournalyDbContext : IdentityDbContext<JournalyUser>
{
    private readonly IConfiguration _config;

    public DbSet<EmotionCategory> EmotionCategories { get; set; }
    public DbSet<Emotion> Emotions { get; set; }
    public DbSet<IconType> IconType { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<ActivityEntry> ActivityEntries { get; set; }
    public DbSet<JournalEntryCategoryValue> JournalEntryCategoryValues { get; set; }
    public DbSet<EmotionEntry> EmotionEntries { get; set; }
    public DbSet<SyncedRecords> SyncedRecords { get; set; }
    public DbSet<MedUnit> MedUnits { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedSchedule> MedSchedules { get; set; }
    public DbSet<MedScheduleDays> MedScheduleDays { get; set; }
    public DbSet<Day> Days { get; set; }
    public DbSet<Data.Models.RecordType> RecordTypes { get; set; }
    public DbSet<MedicationInstance> MedicationInstances { get; set; }
    public DbSet<Data.Models.MedStatus> MedStatuses { get; set; }
    public DbSet<Data.Models.UserToken> UserTokenStore { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
    public DbSet<EncryptedDEKType> EncryptedDekTypes { get; set; }
    public DbSet<EncryptedDEK> EncryptedDeks { get; set; }
    public DbSet<AccountRecoveryToken> AccountRecoveryTokens { get; set; }
    
    public JournalyDbContext(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host={_config.GetValue<string>("Database:Host")};Database={_config.GetValue<string>("Database:Database")};Username={_config.GetValue<string>("Database:Username")};Password={_config.GetValue<string>("Database:Password")};");
}