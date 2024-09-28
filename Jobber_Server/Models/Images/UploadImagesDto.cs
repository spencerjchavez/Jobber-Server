
public record UploadImagesDto
{
    public ICollection<IFormFile> Images { get; set; } = new List<IFormFile>();
}