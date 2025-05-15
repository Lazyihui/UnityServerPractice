using System;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

namespace ServerMain {

    public class ServerContext {
        public Server server;
        int port = 7777;
        int messageSize = 1024;
        public List<int> clientIDs;

        public ServerContext() {
            clientIDs = new List<int>();
            server = new Server(messageSize);
            server.Start(port);
        }

    }
}