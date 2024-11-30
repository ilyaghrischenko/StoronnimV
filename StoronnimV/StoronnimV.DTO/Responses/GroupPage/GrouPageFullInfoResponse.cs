using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage;

public class GrouPageFullInfoResponse
{
    public GroupPageResponse GroupPage { get; set; }
    public IEnumerable<MemberShortResponse> Members { get; set; }
    
    public GrouPageFullInfoResponse() { }
    
    public GrouPageFullInfoResponse(GroupPageResponse groupPage, IEnumerable<MemberShortResponse> members)
    {
        GroupPage = groupPage;
        Members = members;
    }
}