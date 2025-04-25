using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Utilities;

public class ImageUpload(string directoryPath)
{
    //private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
    private readonly string _uploadFolderPath = Path.Combine("wwwroot", directoryPath);
   
    public void CreateDirectory()
    {
        if (!Directory.Exists(_uploadFolderPath))
        {
            Directory.CreateDirectory(_uploadFolderPath);
        }
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new BadRequestException($"file {Path.GetFileName(file.FileName)} length is 0.");
        }
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (Array.IndexOf(allowedExtensions, extension) < 0)
        {
            throw new BadRequestException($"Invalid file type for {Path.GetFileName(file.FileName)}.");
        }
        
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_uploadFolderPath, fileName);
        
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    public void RemoveImage(string fileName)
    {
        
        if (string.IsNullOrEmpty(fileName))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
            //throw new BadRequestException("File name is required.");
        }
        
        var filePath = Path.Combine(_uploadFolderPath, fileName);

        if (!File.Exists(filePath))
        {
            throw new NotFoundException($"File {fileName} not found.");
        }

        File.Delete(filePath);
    }
}