﻿using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace JournalyApiV2.Services.DAL;

public class MedDbService : IMedDbService
{
    private readonly IDbFactory _db;
    private readonly ISyncDbService _syncDbService;

    public MedDbService(IDbFactory db, ISyncDbService syncDbService)
    {
        _db = db;
        _syncDbService = syncDbService;
    }

    public async Task SyncMeds(PatchMedsRequest.MedPatch[] patches, Guid owner, Guid deviceId)
    {
        var tasks = patches.Select(medPatch => Task.Run(() => SyncSingleMed(medPatch, owner)))
            .ToArray();
        await Task.WhenAll(tasks);
        // Since this device uploaded these records we can mark them synced for that device
        var recordSyncs = patches.Select(x => new RecordSync
        {
            DeviceId = deviceId,
            RecordId = x.Uuid,
            RecordType = Data.Enums.RecordType.Med
        });
        await _syncDbService.MarkSynced(recordSyncs.ToArray());
    }

    private async Task SyncSingleMed(PatchMedsRequest.MedPatch patch, Guid owner)
    {
        await using var db = _db.Journaly();
        var transaction = await db.Database.BeginTransactionAsync();
        var dbMed = await db.Medications.Include(x => x.MedSchedules).ThenInclude(x => x.Days)
            .SingleOrDefaultAsync(x => x.Uuid == patch.Uuid);
        if (dbMed == null)
        {
            dbMed = new Data.Models.Medication
            {
                Uuid = patch.Uuid,
                Owner = owner,
                MedSchedules = new List<MedSchedule>()
            };
            await db.Medications.AddAsync(dbMed);
        }

        // Do the easy stuff first (the direct properties of the dbMed)
        if (patch.Deleted != null) dbMed.Deleted = patch.Deleted.Value;
        if (patch.Name != null) dbMed.Name = patch.Name;
        if (patch.Notes != null) dbMed.Notes = patch.Notes;
        if (patch.Unit != null) dbMed.Unit = patch.Unit.Value;
        if (patch.DefaultDose != null) dbMed.DefaultDose = patch.DefaultDose.Value;
        if (patch.From != null) dbMed.FromDate = patch.From.Value;
        if (patch.Until != null) dbMed.UntilDate = patch.Until.Value;
        if (patch.Forever != null) dbMed.Forever = patch.Forever.Value;

        var accountedFor = new List<int>();
        /* Now the hard part - schedules
        / How this will work is we will go through every schedule and find any that are unchanged and add them to the list above.
        / Any that are changed will be recreated and also added to the list above.
        / Then, anything not in the list above will be removed.
        / The result will be recreating any changed items and leaving alone any that have not changed, and removing any that are removed
        */
        foreach (var schedule in patch.Schedules)
        {
            var existing = dbMed.MedSchedules.SingleOrDefault(x =>
                x.EveryOtherDay == schedule.EveryOtherDay && x.Time == schedule.Time &&
                x.Days.Select(x => x.DayId).ToArray() == schedule.Days);
            if (existing != null) // This schedule is unchanged - don't do anything
            {
                accountedFor.Add(existing.Id);
                continue;
            }

            // This schedule is new or changed - Add a new one
            var result = await db.MedSchedules.AddAsync(new MedSchedule
            {
                MedicationUuid = patch.Uuid,
                Time = schedule.Time
            });
            await db.SaveChangesAsync(); // save so the schedule gets an ID
            accountedFor.Add(result.Entity.Id);
            // Add the days
            await db.MedScheduleDays.AddRangeAsync(schedule.Days.Select(x => new MedScheduleDays
            {
                MedScheduleId = result.Entity.Id,
                DayId = x
            }));
            await db.SaveChangesAsync();
        }

        // Finally, remove any schedules that are not accounted for
        var toRemove = dbMed.MedSchedules.Where(x => !accountedFor.Contains(x.Id)).ToArray();
        db.MedSchedules.RemoveRange(toRemove);
        // Now we can actually save and commit
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task SyncMedInstances(PatchMedsRequest.MedInstancePatch[] patches, Guid owner, Guid deviceId)
    {
        var tasks = patches.Select(medInstancePatch => Task.Run(() => SyncSingleMedInstance(medInstancePatch, owner)))
            .ToArray();
        await Task.WhenAll(tasks);
        // Since this device uploaded these records we can mark them synced for that device
        var recordSyncs = patches.Select(x => new RecordSync
        {
            DeviceId = deviceId,
            RecordId = x.Uuid,
            RecordType = Data.Enums.RecordType.MedInstance
        });
        await _syncDbService.MarkSynced(recordSyncs.ToArray());
    }

    public async Task SyncSingleMedInstance(PatchMedsRequest.MedInstancePatch patch, Guid owner)
    {
        await using var db = _db.Journaly();
        var dbMedInstance = await db.MedicationInstances.FindAsync(patch.Uuid);
        if (dbMedInstance == null)
        {
            if (patch.MedicationUuid == null)
                throw new ArgumentException("Medication UUID is required to create a new instance");
            dbMedInstance = new Data.Models.MedicationInstance
            {
                Uuid = patch.Uuid,
                Owner = owner,
                MedicationUuid = patch.MedicationUuid.Value
            };
            await db.MedicationInstances.AddAsync(dbMedInstance);
        }

        if (patch.Dose != null) dbMedInstance.Dose = patch.Dose.Value;
        dbMedInstance.ScheduledTime = patch.ScheduledTime;
        dbMedInstance.ActualTime = patch.ActualTime;
        if (patch.Status != null) dbMedInstance.Status = patch.Status.Value;
        
        await db.SaveChangesAsync();
    }

}