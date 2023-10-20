using JournalyApiV2.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalyApiV2.Data;

public class JournalyDbContext : DbContext
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
    
    public JournalyDbContext(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host={_config.GetValue<string>("Database:Host")};Database={_config.GetValue<string>("Database:Database")};Username={_config.GetValue<string>("Database:Username")};Password={_config.GetValue<string>("Database:Password")};Include Error Detail=true;");
}