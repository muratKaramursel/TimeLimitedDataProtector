using Microsoft.AspNetCore.DataProtection;

internal class Program
{
    private readonly static ITimeLimitedDataProtector _timeLimitedDataProtector = CreateProtector("TEST");

    private static void Main(string[] args)
    {
        string data = ReadData();

        TimeSpan protectedDataLifeTime = TimeSpan.FromSeconds(5);

        string protectedData = _timeLimitedDataProtector.Protect(data, protectedDataLifeTime);

        Timer protectedDataTimer = new(ProtectedDataTimerCallBackFunction, protectedData, TimeSpan.FromSeconds(0), protectedDataLifeTime.Divide(5));

        Console.ReadLine();
    }

    private static string ReadData()
    {
        Console.WriteLine("Please enter the text to be protected...");

        string? data = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(data))
            return ReadData();

        return data;
    }

    private static ITimeLimitedDataProtector CreateProtector(string purpose)
    {
        IDataProtectionProvider dataProtectionProvider = DataProtectionProvider.Create("MyApplication");
        IDataProtector dataProtector = dataProtectionProvider.CreateProtector($"MyApplication.{purpose}");
        ITimeLimitedDataProtector timeLimitedDataProtector = dataProtector.ToTimeLimitedDataProtector();

        return timeLimitedDataProtector;
    }

    private static void ProtectedDataTimerCallBackFunction(object protectedDataObject)
    {
        try
        {
            string protectedData = (string)protectedDataObject;
            string unprotectedData = _timeLimitedDataProtector.Unprotect(protectedData);
            Console.WriteLine($"{DateTime.Now:dd-MM-yyyy HH:mm:ss:fff}{Environment.NewLine}{protectedData}{Environment.NewLine}{unprotectedData}");
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            Console.WriteLine("Protected Data Life Time Finished. Please press \"Enter\" to end application.");
        }
    }
}