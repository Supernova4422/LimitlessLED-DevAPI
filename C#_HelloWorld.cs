
//Example for the RGBW set of milight bulbs. 
//This is not 'official' documentation, and was written by Benjamin Krajancic
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        string ip = "10.0.0.65";
        const int port = 8899;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);

        byte[] off_command = new byte[] {0x41, 0x00, 0x55 };
        byte[] on_command  = new byte[]  {0x42, 0x00, 0x55 };

        UdpClient udpClient = new UdpClient();
        udpClient.Send(on_command, on_command.Length, endpoint);
        const int one_second = 1000;
        System.Threading.Thread.Sleep(one_second * 3);
        udpClient.Send(off_command, off_command.Length, endpoint);
    }
}
