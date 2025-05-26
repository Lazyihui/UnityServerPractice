using System;
using MyTelepathy;
using UnityEngine;

namespace ServerMain {

    public class StuffEntity {
        public float gravity = 4.8f; // 重力加速度
        public float initialYVelocity = 0f; // 初始Y轴速度
        public float currentYVelocity;
        public IDSignature idSig;
        public Vector3 pos;

        public StuffEntity() {
            currentYVelocity = initialYVelocity;
        }
    }
}