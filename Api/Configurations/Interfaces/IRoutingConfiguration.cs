﻿namespace Api.Models.Interfaces;

public interface IRoutingConfiguration
{
     string? CartServiceAddress { get; set; }
     string? ApiServiceAddress { get; set; }
}