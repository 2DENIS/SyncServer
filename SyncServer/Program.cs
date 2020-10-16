using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SyncServer
{
    class Program
    {
        private static int PORT_NUMBER = 50000;
        private static string IP_ADDRESS = "192.168.0.199";
        // длина очереди ожидающих запросов
        private static int BACKLOG_LENGHT = 10;
        // Стандартное приветсвие на клиента
        private static string clientWelcomeMsg = "Привет клиент!";
        static void Main(string[] args)
        {
            RunServer();
        }

        private static void RunServer()
        {
            
            IPAddress serverIP = IPAddress.Parse(IP_ADDRESS);
            // Энд поинт - постоянный адресс сервера в сети
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, PORT_NUMBER);

            Socket listenerSocket = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenerSocket.Bind(serverEndPoint);

           

            try
            {
                listenerSocket.Listen(BACKLOG_LENGHT);

                Console.WriteLine($"Сервер запущен по адресу {IP_ADDRESS}:{PORT_NUMBER}");
                //бесконечный цикл ожидания входящих подключений
                while (true)
                {
                    // при подключении клиента переводим его на отдельный сокет
                    Socket requestHandlingSocket = listenerSocket.Accept();

                    //Буфер для отправки двоичного представления сообщения
                    byte[] serverMessage = Encoding.Unicode.GetBytes(clientWelcomeMsg);

                    requestHandlingSocket.Send(serverMessage);

                    //создаем буфер временного хранения сетевых бай - полученных из сети
                    byte[] incomeBuffer = new byte[1024];

                    int bytesReceived = requestHandlingSocket.Receive(incomeBuffer);

                    // восстановление из байт текст сообщения

                    string clientMessage = Encoding.Unicode.GetString(incomeBuffer, 0, bytesReceived);

                    Console.WriteLine($"Получено сообщение от клиента {clientMessage}");

                    requestHandlingSocket.Shutdown(SocketShutdown.Both);

                    requestHandlingSocket.Close();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            finally
            {

            }
        }
    }
}
