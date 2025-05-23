using System;

namespace ServerMain {

    public class IDServer {
        public int roleID;
        public int bulletID;

        public IDServer() {
            roleID = 0;
        }

        public int PickRoleID() {
            return ++roleID;
        }
        public int PickBulletID() {
            return ++bulletID;
        }
    }
}