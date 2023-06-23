using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfiesh
{
    public unsafe class Socket
    {
        public static List<Client> Pool = new List<Client>();
        public void HandleAllPool()
        {
            while (true)
            {
                foreach (var client in Pool)
                {
                }
                System.Threading.Thread.Sleep(1);
            }
        }
        public Socket()
        {
            new System.Threading.Thread(HandleAllPool).Start();
        }
        public bool AddClient(Client client)
        {
            foreach (Client item in Pool)
            {
                if (item.ip.ToString().CompareTo(client.ip.ToString()) == 0)
                {
                    DateTime now = DateTime.Now;
                    item.timeSinceLastRarp = now;
                    return false;
                }
            }
            Pool.Add(client);
            if (!client.isGateway)
            {
                if (client.isLocalPc)
                {
                    Console.WriteLine("My PC:" + client.name + " IP:" + client.ip.ToString() + " Mac:" + client.mac + "\n");
                }
                else
                {
                    Console.WriteLine("Name:" + client.name + " IP:" + client.ip.ToString() + " Mac:" + client.mac + "\n");
                    Console.Title = "Alive Connectors:" + Pool.Count;
                }
            }
            return true;
        }
        public bool RemoveClient(Client client)
        {
            foreach (Client item in Pool)
            {
                if (item.ip.ToString().CompareTo(client.ip.ToString()) == 0)
                {
                    Pool.Remove(client);
                    Console.Title = "Alive Connectors:" + Pool.Count;
                    return true;
                }
            }
            return false;
        }
        public object SyncRoot = new object();
        public Client GetRouter()
        {
            lock (SyncRoot)
            {
                foreach (var client in Pool)
                {
                    if (client.isGateway)
                    {
                        return client;
                    }
                }
            }
            return null;
        }
        public void Reset()
        {
            lock (SyncRoot)
            {
                foreach (var client in Pool)
                {
                    client.nbPacketReceivedSinceLastReset = 0;
                    client.nbPacketSentSinceLastReset = 0;
                }
            }          
        }
        public Client GetLocalPC()
        {
            lock (SyncRoot)
            {
                foreach (var client in Pool)
                {
                    if (client.isLocalPc)
                    {
                        return client;
                    }
                }
            }
            return null;
        }       
        public Client GetClientFromIP(byte[] IP)
        {
            lock (SyncRoot)
            {
                foreach (var client in Pool)
                {
                    if (areValuesEqual(client.ip.GetAddressBytes(), IP))
                    {
                        return client;
                    }
                }
            }
            return null;
        }
        public Client GetClientFromMAC(byte[] MAC)
        {
            lock (SyncRoot)
            {
                foreach (var client in Pool)
                {
                    if (areValuesEqual(client.mac.GetAddressBytes(), MAC))
                    {
                        return client;
                    }
                }
            }
            return null;
        }
        public static System.Net.IPAddress getIpAddress(string ip)
        {
            string[] array3 = new string[4];
            byte[] array = new byte[4];
            string[] array2 = ip.Split('.', '\u0003');
            int num = 0;
            do
            {
                array[num] = Convert.ToByte(array2[num]);
                num++;
            }
            while (num < 4);
            return new System.Net.IPAddress(array);
        }
        public unsafe static bool areValuesEqual(byte[] obj1, byte[] obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                int num = obj1.Length;
                if ((IntPtr)num != (IntPtr)(void*)obj2.LongLength)
                {
                    return false;
                }
                int num2 = 0;
                int num3 = num;
                if (0 < num3)
                {
                    do
                    {
                        if (obj1[num2] == obj2[num2])
                        {
                            num2++;
                            continue;
                        }
                        return false;
                    }
                    while (num2 < num3);
                }
                return true;
            }
            return false;
        }
    }
}