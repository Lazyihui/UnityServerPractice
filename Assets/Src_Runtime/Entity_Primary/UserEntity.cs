using MyTelepathy;
using UnityEngine;

namespace ServerMain {

    public class UserEntity {

        public string roleName;
        public int connID;

        public IDSignature idSig; // 唯一标识符

        public Vector3 pos;

    }
}