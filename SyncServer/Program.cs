using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;

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
        private static List<string> ARR_MSG= new List<string>() /*{"Так вот ты какой?!","И что тебе еще нужно?","А какже, согласен!" }*/;
        // фраза для разрыва соединения
        private static string DISCONNECT_MSG = "ДО СВИДАНИЯ";
          
        static void Main(string[] args)
        {
            // CreateDictionary("Dictionary.txt");
            GetDictionary("Dictionary.txt");
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
                    byte[] serverMessage = Encoding.UTF8.GetBytes(CLIENT_INCOM_MSG);
                    //отправка сообщения
                    requestHandlingSocket.Send(serverMessage);

                    while (true)
                    {
                        //создаем буфер временного хранения сетевых байт - полученных из сети
                        byte[] incomeBuffer = new byte[BUFER_SIZE];

                        int bytesReceived = requestHandlingSocket.Receive(incomeBuffer);

                        // восстановление из байт текст сообщения
                        string clientMessage = Encoding.UTF8.GetString(incomeBuffer, 0, bytesReceived);
                        
                        Console.WriteLine($"Получено сообщение от клиента {clientMessage}");

                        // переводим в верхний регистр
                        string check_msg = clientMessage.ToUpper();

                        // проверка окончания соединения
                        if (check_msg.Contains(DISCONNECT_MSG))
                        {
                            break;
                        }

                        // записываем полученное сообщение от клиента , для расширение словаря ответов с проверкой на дублирование фразы.
                        foreach (var item in ARR_MSG)
                        {
                            string itemstr = item.ToUpper();
                            if (!itemstr.Equals(check_msg))
                            {
                                ARR_MSG.Add(clientMessage);
                                break;
                            } 
                        }                                       

                        // Рандом генерация ответа 
                        Random rand = new Random();         
                        
                        int ind_msg =  rand.Next(ARR_MSG.Count);

                        string answer_msg = ARR_MSG[ind_msg];

                        //Буфер для отправки двоичного представления сообщения
                        byte[] requaredMessage = Encoding.UTF8.GetBytes(answer_msg);

                        //отправка сообщения клиенту
                        requestHandlingSocket.Send(requaredMessage);
                        
                    }
                   
                    //закрытие сокета
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
                // обновляем коллекцию слов
               UpdateDictionary("Dictionary.txt");
            }
        }

        private static void GetDictionary(string file)
        {
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string Read_Str = sr.ReadLine();
                        ARR_MSG.Add(Read_Str);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка считывания файла {ex}");
            }
        }

        private static void UpdateDictionary(string file)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file, false,
               Encoding.GetEncoding("utf-8")))
                {
                    foreach (var item in ARR_MSG)
                    {
                        sw.WriteLine();
                        sw.Write(item);
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка добавления в файл {ex}");
            }
               
        }

        private static void CreateDictionary(string file)
        {
            List<string> ARR_MSG2 = new List<string>() {"Так вот ты какой?!","И что тебе еще нужно?","А какже, согласен!" };

            try
            {
                using (StreamWriter sw = new StreamWriter(file, false,
               Encoding.GetEncoding("utf-8")))
                {
                    foreach (var item in ARR_MSG2)
                    {
                        sw.WriteLine();
                        sw.Write(item);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка создания и записи в файл {ex}");
            }
           
        }

    }
}
