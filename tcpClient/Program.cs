using System.Net;
using System.Net.Sockets;

namespace tcpClient
{
    internal class Program
    {
        static string[]? address;
        static TcpClient tcpClient = new();
        static NetworkStream? netStream;
        static BinaryReader? binReader; static BinaryWriter? binWriter;
        static Thread? readerThread;
        static void Main(string[] args)
        {
            address = new string[2] { "127.0.0.1", "56620" };
            tcpClient = new();
            tcpClient.Connect(address[0], int.Parse(address[1]));//
            if (tcpClient != null)
            {
                try
                {
                    netStream = tcpClient.GetStream();//获取网络数据流
                    binReader = new BinaryReader(netStream);//数据流读取对象
                    binWriter = new BinaryWriter(netStream);//数据流写入对象
                    string localip = "0.0.0.0";
                    IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());//获取本机ip
                    foreach (IPAddress ip in ips)
                    {
                        if (!ip.IsIPv6SiteLocal) localip = ip.ToString();
                    }
                    binWriter.Write(localip);

                    Console.WriteLine("已连接至服务器({0}:{1})\r\n当前已与服务端建立连接，输入信息按下回车即可发送: ", address[0], address[1]);
                    readerThread = new Thread(Reader);//建立数据流读取线程
                    readerThread.Start();
                    string? input;
                    while (true)
                    {
                        input = Console.ReadLine();
                        if (input != null) binWriter.Write(input);//输入信息后写入数据流
                    }
                }
                catch { Console.WriteLine("发生错误!"); }
            }
        }
        static void Reader()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    Console.WriteLine("服务端信息: " + binReader?.ReadString());
                }
            }
            catch { return; }
        }
    }
}
