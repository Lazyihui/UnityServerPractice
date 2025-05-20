using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Differences;
using Unity.VisualScripting;
using UnityEngine;

namespace ServerMain {

    public class RoleEntiy {

        public string roleName;

        public int idSig; // 唯一标识符

        // TODO:这里有问题 生成一个新的角色时要刷新这个位置；
        public Vector3 pos;

        public void SetPos(Vector3 newPos) {
            pos = newPos;
        }
    }
}