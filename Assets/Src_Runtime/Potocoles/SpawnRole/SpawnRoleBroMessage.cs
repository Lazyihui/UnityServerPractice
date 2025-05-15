// Request(Req): 客户端 发-> 服务端
using System;
using UnityEngine;

namespace ServerMain {

    public struct SpawnRoleBroMessage {

        public IDSignature idSig;
        
        public Vector2 pos;

    }

}