using Domain.Enum;

namespace Domain.Model.Upload;

public class UploadResultModel
{
    public string? Id { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public string? FileSizeText { get; set; }
    public int? FileSize { get; set; }
    public string? Url { get; set; }
    public string? AliasName { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; } = false;
    public DocumentType? DocumentType { get; set; } = Enum.DocumentType.File;
    public Exception? ex { get; set; }
}