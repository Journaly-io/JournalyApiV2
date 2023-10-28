﻿using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName);
    Task<AuthenticationResponse> SignIn(string email, string password);
    Task<AuthenticationResponse> RefreshToken(string refreshToken);
    Task<AuthenticationResponse> ChangeName(string firstName, string lastName, Guid userId, int tokenId);
}