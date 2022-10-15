using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.ItSelf;

namespace InterviewDemo
{
    class Demo
    {
        static async Task Main(string[] args)
        {
            var host = new ServerHost(new ControllersHandler(typeof(Demo).Assembly));
            await host.Start();
        }
    }
}
