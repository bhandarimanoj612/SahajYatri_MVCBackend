namespace Sahaj_Yatri.Services
{
    public interface IFileService
    {
        Task<string> WriteFile(IFormFile file);
    }
}
