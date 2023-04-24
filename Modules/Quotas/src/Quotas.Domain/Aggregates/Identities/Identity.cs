﻿namespace Backbone.Modules.Quotas.Domain.Aggregates.Entities;

public class Identity
{
    public Identity(string address, string tierId)
    {
        Address = address;
        TierId = tierId;
    }

    public string Address { get; }
    
    public string TierId { get; }
}