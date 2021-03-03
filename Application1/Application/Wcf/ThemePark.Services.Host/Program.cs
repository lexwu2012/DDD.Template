using System;
using Abp;

namespace ThemePark.Services.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = AbpBootstrapper.Create<ThemeParkServicesModule>();
            bootstrapper.Initialize();

            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("started.");
            Console.WriteLine("press any key to exit");
            Console.ReadKey();

            bootstrapper.Dispose();
        }
    }
}
