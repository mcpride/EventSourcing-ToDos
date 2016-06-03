using System;
using ToDos.Infrastructure;
using ToDos.Infrastructure.IoC;
using Topshelf;

namespace ToDos.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc = new MefBootstrapper<IToDosService>(typeof (Program).Assembly).Root;
            svc.Start();
            Console.ReadLine();
            svc.Stop();

            //HostFactory.New(
            //    x => x.Service<IToDosService>(sc =>
            //    {
            //        sc.ConstructUsing(() => new MefBootstrapper<IToDosService>(typeof(Program).Assembly).Root);

            //        // the start and stop methods for the service
            //        sc.WhenStarted(s => s.Start());
            //        sc.WhenStopped(s => s.Stop());
            //    })
            //);
        }
    }
}
