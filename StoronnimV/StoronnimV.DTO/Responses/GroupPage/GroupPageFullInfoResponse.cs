using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage;

public class GroupPageFullInfoResponse
{
    public GroupPageResponse GroupPage { get; set; }
    public IEnumerable<MemberShortResponse> Members { get; set; }
    
    public GroupPageFullInfoResponse() { }
    
    public GroupPageFullInfoResponse(GroupPageResponse groupPage, IEnumerable<MemberShortResponse> members)
    {
        GroupPage = groupPage;
        Members = members;
    }
}