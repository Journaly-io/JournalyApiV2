﻿namespace JournalyApiV2.Models.Requests;

public class CryptographicKey
{
    public string DEK { get; set; }
    public string Salt { get; set; }
    public int Type { get; set; }
}