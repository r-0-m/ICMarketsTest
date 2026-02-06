using AutoMapper;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Core.Commands;
using ICMarketsTest.Core.Queries;

namespace ICMarketsTest.Api.Mapping;

/// <summary>
/// Mapping profile for API request models.
/// </summary>
public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<SyncBlockchainRequest, SyncBlockchainCommand>();
        CreateMap<GetBlockchainSnapshotsRequest, GetSnapshotsQuery>();
    }
}
