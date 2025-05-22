using System;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using System.Collections.Concurrent;

namespace ServerMain {

    public class ServerContext {
        public Server server;
        int port = 7777;
        int messageSize = 1024;
        public List<int> clientIDs;
        public Dictionary<string, UserEntity> userMap;
        // public ConcurrentDictionary

        public GameEntity gameEntity;

        // repos
        public RoleRepository roleRepo;

        public ServerContext() {
            clientIDs = new List<int>();
            userMap = new Dictionary<string, UserEntity>();

            gameEntity = new GameEntity();

            // repos
            roleRepo = new RoleRepository();

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