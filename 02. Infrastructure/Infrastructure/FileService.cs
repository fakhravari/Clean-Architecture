using Application.Contracts.Infrastructure;
using Domain.Enum;
using Domain.Model.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infrastructure;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static string GetReadableFileSize(long fileSizeInBytes)
    {
        string[] sizeUnits = { "B", "KB", "MB", "GB", "TB" };
        double size = fileSizeInBytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.##} {sizeUnits[unitIndex]}";
    }
    public async Task<UploadResultModel> UploadFile(IFormFile file)
    {
        try
        {
            if (file.Length == 0)
                return new UploadResultModel() { IsError = true, Message = "فایل انتنخابی مشکل دارد" };


            var fileType = file.ContentType;
            var fileSize = int.Parse(file.Length.ToString());
            var AliasName = file.FileName;

            var FileName = (Guid.NewGuid() + Path.GetExtension(file.FileName)).ToLower();

            var ftpAddress = _configuration["FTP:FTPAddress"];
            var ftpUsername = _configuration["FTP:FTPUsername"];
            var ftpPassword = _configuration["FTP:FTPPassword"];

            var request = (FtpWebRequest)WebRequest.Create(new Uri($"{ftpAddress}/Content/Academy/{FileName}"));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;

            using (var requestStream = request.GetRequestStream())
            {
                await file.CopyToAsync(requestStream);
            }

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    return new UploadResultModel() { IsError = true, Message = $"Error uploading file: {response.StatusDescription}" };
                }
            }

            return new UploadResultModel()
            {
                FileType = fileType,
                FileName = FileName,
                FileSize = fileSize,
                FileSizeText = GetReadableFileSize(fileSize),
                Url = $"/Content/Academy/{FileName}",
                Id = Guid.NewGuid().ToString(),
                IsError = false,
                AliasName = AliasName,
                DocumentType = fileType.ToLower().Contains("pdf") ? DocumentType.Pdf : DocumentType.Image,
            };
        }
        catch (Exception e)
        {
            return new UploadResultModel() { IsError = true, ex = e };
        }
    }
    public async Task<UploadResultModel> DeleteFile(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return new UploadResultModel() { IsError = true, Message = "نام فایل مشخص نشده است" };

            var ftpAddress = _configuration["FTP:FTPAddress"];
            var ftpUsername = _configuration["FTP:FTPUsername"];
            var ftpPassword = _configuration["FTP:FTPPassword"];

            var requestUri = new Uri($"{ftpAddress}/Content/Academy/{fileName}");
            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    return new UploadResultModel() { IsError = true, Message = $"خطا در حذف فایل: {response.StatusDescription}" };
                }
            }

            return new UploadResultModel()
            {
                IsError = true,
                Message = "فایل با موفقیت حذف شد"
            };
        }
        catch (Exception ex)
        {
            return new UploadResultModel() { IsError = true, ex = ex };
        }
    }
}