﻿using JournalyApiV2.Models.Requests;

namespace JournalyApiV2.Services.BLL;

public interface IMedService
{
    Task PatchMeds(PatchMedsRequest request, Guid owner, Guid deviceId);
    Task ClearMeds(Guid user);
}