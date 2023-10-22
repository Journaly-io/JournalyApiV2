using JournalyApiV2.Data;
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

        if (patch.Deleted != null) dbMed.Deleted = patch.Deleted.Value;
        if (patch.Name != null) dbMed.Name = patch.Name;
        if (patch.Notes != null) dbMed.Notes = patch.Notes;
        if (patch.Unit != null) dbMed.Unit = patch.Unit.Value;
        if (patch.DefaultDose != null) dbMed.DefaultDose = patch.DefaultDose.Value;
        if (patch.From != null) dbMed.FromDate = patch.From.Value;
        if (patch.Until != null) dbMed.UntilDate = patch.Until.Value;
        if (patch.Forever != null) dbMed.Forever = patch.Forever.Value;


        await db.SaveChangesAsync();
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
                MedicationUuid = patch.MedicationUuid.Value,
                ScheduleUuid = patch.ScheduleUuid
            };
            await db.MedicationInstances.AddAsync(dbMedInstance);
        }

        if (patch.Dose != null) dbMedInstance.Dose = patch.Dose.Value;
        dbMedInstance.ScheduledTime = patch.ScheduledTime?.ToUniversalTime() ?? null;
        dbMedInstance.ActualTime = patch.ActualTime?.ToUniversalTime() ?? null;
        if (patch.Deleted != null) dbMedInstance.Deleted = patch.Deleted.Value;
        if (patch.Status != null) dbMedInstance.Status = patch.Status.Value;

        await db.SaveChangesAsync();
    }

    public async Task SyncSchedules(PatchMedsRequest.SchedulePatch[] patches, Guid owner, Guid deviceId)
    {
        var tasks = patches.Select(schedulePatch => Task.Run(() => SyncSingleSchedule(schedulePatch, owner)))
            .ToArray();
        await Task.WhenAll(tasks);
        // Since this device uploaded these records we can mark them synced for that device
        var recordSyncs = patches.Select(x => new RecordSync
        {
            DeviceId = deviceId,
            RecordId = x.Uuid,
            RecordType = Data.Enums.RecordType.Schedule
        });
        await _syncDbService.MarkSynced(recordSyncs.ToArray());    }

    private async Task SyncSingleSchedule(PatchMedsRequest.SchedulePatch patch, Guid owner)
    {
        await using var db = _db.Journaly();
        var dbSchedule = await db.MedSchedules.FindAsync(patch.Uuid);
        
        if (dbSchedule == null)
        {
            if (patch.MedicationUuid == null)
                throw new ArgumentException("MedicationUuid is required to create a new schedule");
            dbSchedule = new MedSchedule
            {
                Uuid = patch.Uuid,
                MedicationUuid = patch.MedicationUuid.Value,
                Owner = owner
            };
            await db.MedSchedules.AddAsync(dbSchedule);
        }

        if (patch.Time != null) dbSchedule.Time = patch.Time.Value;
        if (patch.EveryOtherDay != null) dbSchedule.EveryOtherDay = patch.EveryOtherDay.Value;
        if (patch.Days != null)
        {
            var dbDays = await db.MedScheduleDays.Where(x => x.MedScheduleUuid == patch.Uuid).ToListAsync();
            // Add missing days
            var missing = patch.Days.Where(x => dbDays.All(y => y.DayId != x)).ToList();
            await db.MedScheduleDays.AddRangeAsync(missing.Select(x => new MedScheduleDays
            {
                DayId = x,
                MedScheduleUuid = patch.Uuid
            }));
            // Remove extra days
            var extra = dbDays.Where(x => patch.Days.All(y => y != x.DayId));
            db.MedScheduleDays.RemoveRange(extra);
        }

        await db.SaveChangesAsync();
    }

}