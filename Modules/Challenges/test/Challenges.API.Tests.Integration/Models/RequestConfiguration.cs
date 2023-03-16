using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Challenges.API.Tests.Integration.Models;
public class RequestConfiguration : IHaveCustomMapping
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new AuthenticationParameters();
    public bool Authenticate { get; set; } = false;
    public string ContentType { get; set; } = string.Empty;
    public string AcceptHeader { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<RequestConfiguration, RequestConfiguration>()
            .ForMember(dest => dest.ContentType, opt => opt.Condition((src, dest) => string.IsNullOrEmpty(dest.ContentType)))
            .ForMember(dest => dest.AcceptHeader, opt => opt.Condition((src, dest) => string.IsNullOrEmpty(dest.AcceptHeader)))
            .ForMember(dest => dest.Content, opt => opt.Condition((src, dest) => string.IsNullOrEmpty(dest.Content)));
    }
}
