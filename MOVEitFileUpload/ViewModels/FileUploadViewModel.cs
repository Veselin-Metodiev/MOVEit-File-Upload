namespace MOVEitFileUpload.ViewModels;

public class FileUploadViewModel
{
    public string Url { get; set; } = null!;

    public IFormFile File { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
