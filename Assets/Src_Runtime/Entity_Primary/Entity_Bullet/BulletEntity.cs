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
        public BulletEntity() {
            direction = Vector3.up;
        }
    }
}