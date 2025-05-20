using System;
using UnityEngine;

namespace ServerMain {
    public class GameEntity {

        public float spawnMstTimer;
        public float spawnMstInterval;

        public GameEntity() {
            spawnMstTimer = 0;
            spawnMstInterval = 2f;
        }
    }
}