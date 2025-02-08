namespace WebApi.Models;


public class ProjectCollaboration
{
    public required string UserId { get; set; }
    public required int ProjectId { get; set; }
    public required string Permission { get; set; }
}
