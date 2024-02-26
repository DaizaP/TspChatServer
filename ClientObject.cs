using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TspChatServer
{
    internal class ClientObject : AbstractClientObject
    {
        public ClientObject(TcpClient tcpClient, AbstractServer serverObject) : base(tcpClient, serverObject)
        {
        }

        public override void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }

        public override async Task ProcessAsync()
        {
            try
            {

                // получаем имя пользователя
                string? userName = await Reader.ReadLineAsync();
                string? message = $"{userName} вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessageAsync(message, Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        if (message.Contains(Code.succesCode))
                        {
                            Console.WriteLine(message);
                            continue;
                        }
                        message = $"{userName}: {message}";
                        if (message == $"{userName}: Exit")
                        {
                            throw new Exception("User exit");
                        }
                        server.BroadcastMessageAsync(message, Id);
                    }
                    catch (Exception ex)
                    {
                        IsClosed = true;
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        server.BroadcastMessageAsync(message, Id);
                        Close();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
            }
        }
    }
}
