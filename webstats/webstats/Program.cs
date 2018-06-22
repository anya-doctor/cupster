/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 12.06.2014
 * Time: 20:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Threading;
using Nancy.Hosting.Self;

namespace webstats
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Start web server
            Console.WriteLine("Press enter to terminate server");
            HostConfiguration hc = new HostConfiguration();
            hc.UrlReservations.CreateAutomatically = true;
            var host = new NancyHost(hc, new Uri("http://localhost:8888"));
            host.Start();
            
            //Under mono if you deamonize a process a Console.ReadLine with cause an EOF 
            //so we need to block another way
            if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.ReadKey();
            }

            host.Stop();
            host.Dispose();
            Console.WriteLine("Server terminated");
        }
    }
}