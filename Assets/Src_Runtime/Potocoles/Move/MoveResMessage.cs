using System;
using UnityEngine;

namespace ServerMain {
    public struct MoveReqMessage {
        public string roleName;
        public Vector3 targetPos;
        public long timestamp;  // 用于客户端预测
    }
}