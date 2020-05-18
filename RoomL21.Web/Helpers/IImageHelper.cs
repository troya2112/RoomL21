using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RoomL21.Web.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
}
