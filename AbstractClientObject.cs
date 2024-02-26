using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TspChatServer
{
    abstract class AbstractClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }
        protected internal bool IsClosed { get; protected set; } = false;

        protected TcpClient client;
        protected AbstractServer server;

        public AbstractClientObject(TcpClient tcpClient, AbstractServer serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public abstract Task ProcessAsync();
        // закрытие подключения
        public abstract void Close();
    }
}
