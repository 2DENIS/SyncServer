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
        // длина буфера по приему сообщения
        private static int BUFER_SIZE = 1024;
        // Стандартное приветсвие на клиента
        private static string CLIENT_INCOM_MSG = "Привет клиент!";
        // массив сообщений 
        private static List<string> ARR_MSG= new List<string>() {"Так вот ты какой?!","И что тебе еще нужно?","А какже, согласен!" };
        // фраза для разрыва соединения
        private static string DISCONNECT_MSG = "ДО СВИДАНИЯ";
          
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
                    byte[] serverMessage = Encoding.Unicode.GetBytes(CLIENT_INCOM_MSG);
                    //отправка сообщения
                    requestHandlingSocket.Send(serverMessage);

                    while (true)
                    {
                        //создаем буфер временного хранения сетевых байт - полученных из сети
                        byte[] incomeBuffer = new byte[BUFER_SIZE];

                        int bytesReceived = requestHandlingSocket.Receive(incomeBuffer);

                        // восстановление из байт текст сообщения
                        string clientMessage = Encoding.Unicode.GetString(incomeBuffer, 0, bytesReceived);
                        
                        Console.WriteLine($"Получено сообщение от клиента {clientMessage}");
                       
                        // проверка окончания соединения
                        if (clientMessage.ToUpper().Contains(DISCONNECT_MSG))
                        {
                            break;
                        }

                        // записываем полученное сообщение от клиента , для расширение словаря ответов.
                        ARR_MSG.Add(clientMessage);

                        // Рандом генерация ответа 
                        Random rand = new Random();         
                        
                        int ind_msg =  rand.Next(ARR_MSG.Count);

                        string answer_msg = ARR_MSG[ind_msg];

                        //Буфер для отправки двоичного представления сообщения
                        byte[] requaredMessage = Encoding.Unicode.GetBytes(answer_msg);

                        //отправка сообщения клиенту
                        requestHandlingSocket.Send(requaredMessage);
                        
                    }

                   
                    //
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
