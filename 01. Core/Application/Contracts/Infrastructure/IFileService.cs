using Domain.Model.Upload;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Infrastructure;

public interface IFileService
{
    Task<UploadResultModel> UploadFile(IFormFile file);
    Task<UploadResultModel> DeleteFile(string fileName);
}

