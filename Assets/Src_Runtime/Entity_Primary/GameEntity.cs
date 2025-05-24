using MyTelepathy;

namespace ServerMain {
    public class GameEntity {

        public float spawnMstTimer;
        public float spawnMstInterval;
        public IDSignature idSig; // 唯一标识符

        public GameEntity() {
            spawnMstTimer = 0;
            spawnMstInterval = 2f;
        }
    }
}