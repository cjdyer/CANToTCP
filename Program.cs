using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace CANToTCP
{
    class Program
    {
        private static NetworkStream ns;
        private static System.Timers.Timer timer;

        static void Main(string[] args)
        {
            // Open CMD -> type "ipconfig" ->
            // Under the header "Wireless LAN adapter WiFi:" ->
            // "IPV4 Address" is the ip address used
            TcpListener server = new TcpListener(IPAddress.Any, 9999);
            server.Start();
            TcpClient client = server.AcceptTcpClient();
            ns = client.GetStream();

            timer = new System.Timers.Timer();
            timer.Interval = 200;
            timer.Elapsed += ReadPGNFile;
            timer.Enabled = true;

            Console.WriteLine("Connected");

            while(true) { Thread.Sleep(200); }
        }

        private static void ReadPGNFile(object source, ElapsedEventArgs e)
        {
            var lines = ReadLines(@"C:\Users\Cory\source\repos\Diagtools\Log_PGNs.txt", Encoding.UTF8);
            string[] info_block = lines[lines.Count - 2].Split("\n");

            //string leftWeightOneString = info_block[1].Replace("\r", "");
            //string rightWeightOneString = info_block[2].Replace("\r", "");
            //string leftWeightTwoString = info_block[3].Replace("\r", "");
            //string rightWeightTwoString = info_block[4].Replace("\r", "");
            string TotalWeightString = info_block[5].Replace("\r", "");
 
            //string leftWeightOne = leftWeightOneString.Split(" ")[5];
            //string rightWeightOne = rightWeightOneString.Split(" ")[5];
            //string leftWeightTwo = leftWeightTwoString.Split(" ")[5];
            //string rightWeightTwo = rightWeightTwoString.Split(" ")[5];
            string TotalWeight = TotalWeightString.Split(" ")[4];

            //Debug.Write("leftWeightOne : ");
            //Debug.Write(leftWeightOne);
            //Debug.Write(" rightWeightOne : ");
            //Debug.Write(rightWeightOne);
            //Debug.Write(" leftWeightTwo : ");
            //Debug.Write(leftWeightTwo);
            //Debug.Write(" rightWeightTwo : ");
            //Debug.WriteLine(rightWeightTwo);

            Debug.WriteLine(TotalWeight);

            byte[] message = Encoding.Default.GetBytes(TotalWeight);
            ns.Write(message, 0, message.Length);
        }

        public static List<string> ReadLines(string path, Encoding encoding)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                List<string> file = reader.ReadToEnd().Split("Bolster Group 1 Weights").ToList();
                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                return file;
            }
        }
    }
}
