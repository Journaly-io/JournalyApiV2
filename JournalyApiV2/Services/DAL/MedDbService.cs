using JournalyApiV2.Data;
using JournalyApiV2.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace JournalyApiV2.Services.DAL;

public class MedDbService : IMedDbService
{
    private readonly IDbFactory _db;

    public MedDbService(IDbFactory db)
    {
        _db = db;
    }

    public async Task SyncMeds(PatchMedsRequest.MedPatch[] patches, Guid owner, Guid deviceId)
    {
        var tasks = patches.Select(emotionCategory => Task.Run(() => SyncSingleMed(emotionCategory, owner)))
            .ToArray();
        await Task.WhenAll(tasks);
    }

    private async Task SyncSingleMed(PatchMedsRequest.MedPatch patch, Guid owner)
    {
        await using var db = _db.Journaly();
        var dbMed = await db.Medications.SingleOrDefaultAsync(x => x.Uuid == patch.Uuid);
        if (dbMed == null)
        {
            dbMed = new Data.Models.Medication
            {
                Uuid = patch.Uuid,
                Owner = owner
            };
            await db.Medications.AddAsync(dbMed);
        }

        // Do the easy stuff first (the direct properties of the dbMed)
        if (patch.Deleted != null) dbMed.Deleted = patch.Deleted.Value;
        if (patch.Name != null) dbMed.Name = patch.Name;
        if (patch.Notes != null) dbMed.Notes = patch.Notes;
        if (patch.Unit != null) dbMed.Unit = patch.Unit.Value;
        if (patch.DefaultDose != null) dbMed.DefaultDose = patch.DefaultDose.Value;
        
        // Now the hard part - schedules
        
    }
}