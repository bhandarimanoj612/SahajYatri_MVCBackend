using Sahaj_Yatri.Models;

namespace Sahaj_Yatri.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
