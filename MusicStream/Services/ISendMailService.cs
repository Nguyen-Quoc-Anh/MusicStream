using MusicStream.Models;
using System.Threading.Tasks;

namespace MusicStream.Services
{
    public interface ISendMailService
    {
        Task<bool> SendEmail(MailRequest mailRequest);
    }
}
