namespace API.Entities;

public class UserImageMessage
{
    public int Id { get; set; }
    public string PhotoUrl { get; set; }
    public string PublicId { get; set; }
    public int CityId { get; set; }
    public string City { get; set; }
}
