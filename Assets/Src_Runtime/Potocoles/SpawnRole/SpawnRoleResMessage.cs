// Request(Req): 客户端 发-> 服务端
using System;
using UnityEngine;

namespace ServerMain {

    public struct SpawnRoleResMessage {

        public RoleType roleType;
        public string roleName;
        public Vector2 pos;

    }

}