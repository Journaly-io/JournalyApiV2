﻿using JournalyApiV2.Models.Requests;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/journal")]
[Authorize]
public class JournalController : JournalyControllerBase
{
    private readonly IJournalService _journalService;
    private readonly IResourceAccessHelper _resourceAccessHelper;

    public JournalController(IJournalService journalService, IResourceAccessHelper resourceAccessHelper)
    {
        _journalService = journalService;
        _resourceAccessHelper = resourceAccessHelper;
    }

    [HttpPatch]
    public async Task<IActionResult> PatchJournal([FromBody] PatchJournalRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(errors);
        }
        
        try
        {
            await Task.WhenAll(
                Task.Run(() =>
                    _resourceAccessHelper.ValidateCategoryAccess(GetUserId(),
                        request.Categories.Select(x => x.Uuid).ToArray())),
                Task.Run(() =>
                    _resourceAccessHelper.ValidateEmotionAccess(GetUserId(),
                        request.Emotions.Select(x => x.Uuid).ToArray())),
                Task.Run(() =>
                    _resourceAccessHelper.ValidateActivityAccess(GetUserId(),
                        request.Activities.Select(x => x.Uuid).ToArray())),
                Task.Run(() =>
                    _resourceAccessHelper.ValidateJournalEntryAccess(GetUserId(),
                        request.JournalEntries.Select(x => x.Uuid).ToArray()))
            );
        }
        catch (IResourceAccessHelper.NoAccessException)
        {
            return StatusCode(403);
        }

        try
        {
            await _journalService.PatchJournal(request, GetUserId(), GetDeviceId());
        }
        catch (ArgumentException ex)
        {
            return StatusCode(400, ex.Message);
        }
        return StatusCode(204);
    }
}