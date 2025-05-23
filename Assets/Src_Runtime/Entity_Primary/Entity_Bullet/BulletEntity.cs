using System;
using UnityEngine;

namespace ServerMain {

    public class BulletEntity {
        public IDSignature idSig;
        public Transform rootPos;
        public Vector3 direction;
        public Vector3 pos;

        public IDSignature belongIdSig;
        public BulletEntity() {
            direction = Vector3.up;
        }
    }
}