using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorDemo
{
    public class Program
    {
        private static Task _hostTask;

        public static void Main(string[] args)
        {
            Task.Delay(TimeSpan.FromSeconds(1))
                .ContinueWith(_ => Task.Run(() => OpenInBrowser("https://localhost:5001/")));

            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static void OpenInBrowser(string url)
        {
            var process = Process.Start("open", url);
            process.WaitForExit();
            Console.WriteLine("open exited with {0}", process.ExitCode);
        }
    }
}