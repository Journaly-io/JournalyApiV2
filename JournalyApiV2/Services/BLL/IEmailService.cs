﻿using JournalyApiV2.Models;

namespace JournalyApiV2.Services.BLL;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string firstName, string lastName, EmailVerification codes);
    Task SendAccountRecoveryEmailAsync(string toEmail, string firstName, string lastName, EmailVerification codes);
}