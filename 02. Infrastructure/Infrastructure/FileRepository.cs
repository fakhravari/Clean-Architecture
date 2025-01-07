using System.Net;
using Application.Contracts.Infrastructure;
using Domain.Enum;
using Domain.Model.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class FileRepository : IFileRepository
{
    private readonly string _ftpAddress;
    private readonly NetworkCredential _ftpCredentials;

    public FileRepository(IConfiguration configuration)
    {
        _ftpAddress = configuration["FTP:FTPAddress"];
        _ftpCredentials = new NetworkCredential(configuration["FTP:FTPUsername"], configuration["FTP:FTPPassword"]);
    }

    public async Task<UploadResultModel> UploadFile(IFormFile file)
    {
        if (file.Length == 0) return new UploadResultModel { IsError = true, Message = "فایل انتخابی مشکل دارد" };

        var fileType = file.ContentType;
        var fileSize = file.Length;
        var aliasName = file.FileName;
        var fileName = (Guid.NewGuid() + Path.GetExtension(file.FileName)).ToLower();

        var request = ConfigureFtpRequest(fileName, WebRequestMethods.Ftp.UploadFile);

        try
        {
            await using (var requestStream = request.GetRequestStream())
            {
                await file.CopyToAsync(requestStream);
            }

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.ClosingData)
                    return new UploadResultModel
                        { IsError = true, Message = $"خطا در آپلود فایل: {response.StatusDescription}" };
            }

            return new UploadResultModel
            {
                FileType = fileType,
                FileName = fileName,
                FileSize = (int)fileSize,
                FileSizeText = GetReadableFileSize(fileSize),
                Url = $"/Content/Academy/{fileName}",
                Id = Guid.NewGuid().ToString(),
                IsError = false,
                AliasName = aliasName,
                DocumentType = fileType.ToLower().Contains("pdf") ? DocumentType.Pdf : DocumentType.Image
            };
        }
        catch (Exception e)
        {
            return new UploadResultModel { IsError = true, ex = e };
        }
    }

    public async Task<UploadResultModel> DeleteFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return new UploadResultModel { IsError = true, Message = "نام فایل مشخص نشده است" };

        var request = ConfigureFtpRequest(fileName, WebRequestMethods.Ftp.DeleteFile);

        try
        {
            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.FileActionOK)
                    return new UploadResultModel
                        { IsError = true, Message = $"خطا در حذف فایل: {response.StatusDescription}" };
            }

            return new UploadResultModel
            {
                IsError = false,
                Message = "فایل با موفقیت حذف شد"
            };
        }
        catch (Exception ex)
        {
            return new UploadResultModel { IsError = true, ex = ex };
        }
    }

    private static string GetReadableFileSize(long fileSizeInBytes)
    {
        string[] sizeUnits = { "B", "KB", "MB", "GB", "TB" };
        double size = fileSizeInBytes;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.##} {sizeUnits[unitIndex]}";
    }

    private FtpWebRequest ConfigureFtpRequest(string path, string method)
    {
        var request = (FtpWebRequest)WebRequest.Create(new Uri($"{_ftpAddress}/Content/Academy/{path}"));
        request.Method = method;
        request.Credentials = _ftpCredentials;
        request.UseBinary = true;
        request.UsePassive = true;
        request.KeepAlive = false;
        return request;
    }
}