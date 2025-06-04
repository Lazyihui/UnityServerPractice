using System;
using UnityEngine;
using MyTelepathy;

namespace ServerMain {

    public class BulletEntity {
        public int idSig;
        public Transform rootPos;
        public Vector3 direction;
        public Vector3 pos;

        public int belongIdSig;

        public float lastSendTime;
        public float sendInterval = 0.1f; // 发送间隔 
        public BulletEntity() {
            direction = Vector3.up;
        }
    }
}