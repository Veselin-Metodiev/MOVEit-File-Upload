using MOVEitFileUpload.ViewModels;

namespace MOVEitFileUpload.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task UploadFile(FileUploadViewModel fileUploadViewModel);

        void ValidateInput(FileUploadViewModel fileUploadViewModel);
    }
}
