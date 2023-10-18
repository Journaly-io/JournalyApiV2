﻿using JournalyApiV2.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalyApiV2.Data;

public class JournalyDbContext : DbContext
{
    private readonly IConfiguration _config;

    public DbSet<EmotionCategory> EmotionCategories { get; set; }
    
    public JournalyDbContext(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host={_config.GetValue<string>("Database:Host")};Database={_config.GetValue<string>("Database:Database")};Username={_config.GetValue<string>("Database:Username")};Password={_config.GetValue<string>("Database:Password")};");
}