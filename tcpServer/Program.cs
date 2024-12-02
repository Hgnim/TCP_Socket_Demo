using System.Net;
using System.Net.Sockets;

namespace tcpServer
{
    internal class Program
    {
        static string[]? address;
        static TcpClient tcpClient=new();//tcp客户端对象
        static NetworkStream? netStream;
        static BinaryReader? binReader;static BinaryWriter? binWriter;
        static TcpListener? tcpListener;
        static Thread? readerThread;
        static void Main(string[] args)
        {
            address = new string[2] { "127.0.0.1", "56620" };//定义开放地址
            tcpListener = new(Dns.GetHostAddresses(address[0])[0], int.Parse(address[1]));//定义监听套接字
            tcpListener.Start();//启动监听
            Console.WriteLine("服务端已启动，正在侦听({0}:{1})...", address[0], address[1]);

            try
            {
                tcpClient = tcpListener.AcceptTcpClient();//侦听到客户端后建立连接
                netStream = tcpClient.GetStream();//获取网络数据流
                binReader = new BinaryReader(netStream);//数据流读取对象
                binWriter = new BinaryWriter(netStream);//数据流写入对象

                Console.WriteLine("侦听到客户端连接！客户端IP:" + binReader.ReadString() +"\r\n当前已与客户端建立连接，输入信息按下回车即可发送:");           
                readerThread = new Thread(Reader);//建立数据流读取线程
                readerThread.Start();
                string? input;
                while (true)
                {
                    input = Console.ReadLine();
                    if (input != null) binWriter.Write(input);//输入信息后写入数据流
                }
            }
            catch
            {
                Console.WriteLine("发生错误!");
            }
        }

      static void Reader()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    Console.WriteLine("客户端信息: "+binReader?.ReadString());//侦听客户端信息并输出
                }
            }
            catch { return; }
        }
    }
}
