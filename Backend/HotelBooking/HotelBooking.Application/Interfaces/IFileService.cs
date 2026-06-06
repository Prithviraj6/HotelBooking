using Microsoft.AspNetCore.Http;

namespace HotelBooking.Application.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Saves an uploaded file to the specified subfolder and returns the relative URL.
        /// </summary>
        Task<string> UploadImageAsync(IFormFile file, string subFolder);

        /// <summary>
        /// Deletes a file given its relative URL.
        /// </summary>
        void DeleteImage(string relativeUrl);
    }
}
