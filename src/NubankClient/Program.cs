using System;
using System.Linq;
using System.Threading.Tasks;

namespace NubankClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var nubankClient = new NubankClient("USER_LOGIN", "USER_PASSWORD");
            var successfulLogin = await nubankClient.LoginAsync();
            if (!successfulLogin) {
                Console.WriteLine(nubankClient.GetQRCodeAsAscii());
                successfulLogin = await nubankClient.LoginWithQrCodeAsync();
            }

            var events = await nubankClient.GetEventsAsync();
            foreach(var e  in events.ToList()) {
                Console.WriteLine($"{e.Title} - {e.Amount} {e.CurrencyAmount}");
            }

            var savings = await nubankClient.GetSavingsAsync();
            foreach(var s  in savings.ToList()) {
                Console.WriteLine($"{s.Title} - {s.Amount} {s.TypeName}");
            }
        }
    }
}
