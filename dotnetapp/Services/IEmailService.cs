
using System.Threading.Tasks;

namespace dotnetapp.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string token);
    }
}
