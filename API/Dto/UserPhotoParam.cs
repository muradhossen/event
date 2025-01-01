using Microsoft.AspNetCore.Http;

namespace API.Dto;

public class UserPhotoParam
{
    public int Id { get; set; }
    public IFormFile Image { get; set; }
}
