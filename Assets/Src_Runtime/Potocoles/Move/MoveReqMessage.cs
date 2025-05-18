using System;
using UnityEngine;

namespace ServerMain {
    public struct MoveResMessage {
        public string roleName;
        public Vector3 targetPos;
        public long timestamp;  // 用于客户端预测
    }
}