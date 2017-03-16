namespace ImageCaption.Services
{
    using System.IO;
    using System.Threading.Tasks;

    internal interface ICaptionService
    {
        Task<string> GetCaptionAsync(Stream stream);
        Task<string> GetCaptionAsync(string url);
    }
}