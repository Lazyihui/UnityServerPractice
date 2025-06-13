using MyTelepathy;
using UnityEngine;

namespace ServerMain {

    public class UserEntity {

        public string roleName;
        public int connID;

        public int idSig; // 唯一标识符

        public Vector3 pos;

        public UserEntity() {

        }

        public void Init(int idSig, string roleName, int connID, Vector3 pos) {
            this.idSig = idSig;
            this.roleName = roleName;
            this.connID = connID;
            this.pos = pos;
        }

    }
}