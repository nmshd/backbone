namespace Backbone.Modules.Quotas.Domain.Entities;

public class Identity
{
    public Identity(string address)
    {
        Address = address;        
    }
    
    public string Address { get; set; }
    
}
