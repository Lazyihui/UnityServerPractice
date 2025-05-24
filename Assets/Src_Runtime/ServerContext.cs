using System;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using MyTelepathy;

namespace ServerMain {

    public class ServerContext {
        public Server server;
        int port = 7777;
        int messageSize = 1024;
        public List<int> clientIDs;
        public Dictionary<string, UserEntity> userMap;
        // public ConcurrentDictionary

        public GameEntity gameEntity;
        public IDServer idServer;
        // repos
        public RoleRepository roleRepo;
        public BulletRepository bulletRepo;
        public ServerContext() {
            clientIDs = new List<int>();
            userMap = new Dictionary<string, UserEntity>();

            gameEntity = new GameEntity();
            idServer = new IDServer();

            // repos
            roleRepo = new RoleRepository();
            bulletRepo = new BulletRepository();

            server = new Server(messageSize);
            server.Start(port);
        }

        public void AddUserEntity(string userName, UserEntity entity) {
            if (!userMap.ContainsKey(userName)) {
                userMap.Add(userName, entity);
            } else {
                Debug.LogWarning("UserEntity already exists: " + userName);
            }
        }

    }
}