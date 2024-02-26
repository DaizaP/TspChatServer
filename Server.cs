using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TspChatServer
{
    internal class Server : AbstractServer
    {
        public override async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in clients)
            {
                if (client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message); //передача данных
                    await client.Writer.FlushAsync();
                }
                if (client.Id == id)
                {
                    if (!client.IsClosed)
                    {
                        await client.Writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.succesCode} Message sent"); //передача данных
                        await client.Writer.FlushAsync();
                    }
                }
            }
        }

        public override async Task Disconnect(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    foreach (var client in clients)
                    {
                        await client.Writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.shutdownServerCode} Server shutdown"); //передача данных
                        await client.Writer.FlushAsync();
                        client.Close(); //отключение клиента
                    }
                    tcpListener.Stop(); //остановка сервера
                    Console.WriteLine("Работа сервера остановлена");
                    break;
                }
            }
        }

        public override async Task ListenAsync(CancellationToken token)
        {
            Task disconnectTask = new Task(() => Disconnect(token));
            disconnectTask.Start();
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);
                    Task t = new Task(() => clientObject.ProcessAsync()); // добавляем фоновый поток
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect(token);
            }
        }

        public override void RemoveConnection(string id)
        {
            AbstractClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null) clients.Remove(client);
            client?.Close();
        }
    }
}
