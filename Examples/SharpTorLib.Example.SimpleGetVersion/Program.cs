using System;
using System.Net;
using SharpTorLib.Control;
using SharpTorLib.Control.Commands;
using SharpTorLib.Control.Exceptions;
using SharpTorLib.Control.Replies;
using SharpTorLib.Examples.Common;

namespace SharpTorLib.Example.SimpleGetVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TorControlClient(IPAddress.Loopback, 9051);

            try
            {
                Console.WriteLine("Connecting...");
                client.Connect(Settings.TorPassword);
                Console.WriteLine("Connected!");
            }
            catch (ConnectFailedException ex)
            {
                Console.WriteLine("Unable to connect. {0}", ex.Reply.Arguments);
                Console.WriteLine("Did you change the value of 'Settings.TorPassword' ?"); 
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured while connecting: {0}", ex.Message);
                return;
            }

            Console.WriteLine("Receiving Tor version...");
            TorReply[] replies = client.Send(new TorCommand("GETINFO", "version"));

            var getInfoReply = new GetInfoTorReply(replies);

            if (getInfoReply.IsOK)
            {
                Console.WriteLine("Tor {0}", getInfoReply.Value);
            }
            else
            {
                Console.WriteLine("Unable to get the version.");
            }

            client.Close();

            Console.ReadLine();
        }
    }
}
