using AutoMapper;
using ICMarketsTest.Contracts;
using ICMarketsTest.Infrastructure.Persistence.Entities;

namespace ICMarketsTest.Infrastructure.Mapping;

/// <summary>
/// Mapping profile for persistence models.
/// </summary>
public sealed class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<BlockchainSnapshot, BlockchainSnapshotDto>().ReverseMap();
    }
}
