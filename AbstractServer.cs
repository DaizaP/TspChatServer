using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TspChatServer
{
    abstract class AbstractServer
    {

        protected TcpListener tcpListener = new TcpListener(IPAddress.Any, 55555); // сервер для прослушивания
        protected List<AbstractClientObject> clients = new List<AbstractClientObject>(); // все подключения
        public abstract void RemoveConnection(string id);
        // прослушивание входящих подключений
        public abstract Task ListenAsync(CancellationToken token);

        // трансляция сообщения подключенным клиентам
        public abstract Task BroadcastMessageAsync(string message, string id);
        // отключение всех клиентов
        public abstract Task Disconnect(CancellationToken token);
    }
}
