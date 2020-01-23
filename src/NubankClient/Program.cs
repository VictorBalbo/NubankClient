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
            var sucessfulLogin = await nubankClient.LoginAsync();
            if (!sucessfulLogin) {
                Console.WriteLine(nubankClient.GetQRCodeAsAscii());
                sucessfulLogin = await nubankClient.LoginWithQrCodeAsync();
            }

            var events = await nubankClient.GetEventsAsync();
            foreach(var e  in events.ToList()) {
                Console.WriteLine($"{e.Title} - {e.Amount} {e.CurrencyAmount}");
            }
        }
    }
}
