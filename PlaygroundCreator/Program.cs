using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Qlik.Engine;
using Qlik.Engine.Communication;
using Qlik.Engine.Extensions;
using Qlik.Sense.Client;

namespace PlaygroundCreator
{
    class Program
    {
        private static Uri _qlikSenseServerUri;

        static void Main(string[] args)
        {
            // Uri to Qlik Sense Server can be given by command-line or use a default value.
            if (args.Length > 0)
                _qlikSenseServerUri = new Uri(args[0]);
            else
                _qlikSenseServerUri = new Uri("http://playground.qlik.com");

            ILocation location = SetupConnection();

            PrintQlikSenseVersionNumber(location);
        }

        private static ILocation SetupConnection()
        {
            ILocation location = Qlik.Engine.Location.FromUri("https://playground.qlik.com");

            location.VirtualProxyPath = "anon";
            location.AsAnonymousUserViaProxy();
            return location;
        }

        private static void PrintQlikSenseVersionNumber(ILocation location)
        {
            try
            {
                using (IHub hub = location.Hub(noVersionCheck: true))
                {
                    Console.WriteLine(hub.ProductVersion());
                    var app = hub.CreateSessionApp().Return;
                    app.SetScript(app.GetCommonOperationsScript());
                    app.DoReload();
                    foreach (var nxFieldDescription in app.GetFieldList().Items)
                    {
                        Console.WriteLine(nxFieldDescription.Name);
                    }
                }
            }
            catch (CommunicationErrorException cex)
            {
                Console.WriteLine("Can not connect to Qlik Sense instance, check that Qlik Sense is running." + Environment.NewLine + cex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error." + Environment.NewLine + ex.Message);
            }
            Console.ReadLine();
        }
    }
}
