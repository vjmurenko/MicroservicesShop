﻿namespace CartService.Configurations;

public class RabbitmqConfiguration
{
    public string? Hostname { get; set; }
    public string? VirtualHost { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}