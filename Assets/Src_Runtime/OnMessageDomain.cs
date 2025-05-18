using System;
using Codice.Client.Commands.CheckIn;
using Telepathy;
using UnityEngine;
using UnityEngine.UIElements;


namespace ServerMain {

    public static class OnMessageDomain {

        public static void OnSpawnRoleRes(int connID, SpawnRoleReqMessage req, ServerContext ctx) {

            Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0f);
            // 1. 为新玩家生成角色并返回响应
            SpawnRoleResMessage res = new SpawnRoleResMessage {
                pos = randomPos,
                roleName = req.roleName
            };
            byte[] resData = MessageHelper.ToData(res);
            ctx.server.Send(connID, resData);

            // 2. 将新玩家信息存入 userMap（检查重复）
            if (ctx.userMap.ContainsKey(req.roleName)) {
                Debug.LogWarning($"角色名已存在: {req.roleName}");
                return;
            }

            UserEntity userEntity = new UserEntity {
                roleName = req.roleName,
                connID = connID,
                pos = randomPos // 初始化位置
            };
            ctx.AddUserEntity(req.roleName, userEntity);

            // 3. 向新玩家同步所有已存在的角色信息
            foreach (var existingUser in ctx.userMap.Values) {

                if (existingUser.connID != connID) {

                    SpawnRoleBroMessage bro = new SpawnRoleBroMessage {
                        pos = existingUser.pos,
                        roleName = existingUser.roleName
                    };

                    ctx.server.Send(connID, MessageHelper.ToData(bro));

                    Debug.Log($"同步已有角色 {existingUser.idSig} 给新玩家 {connID}" +
                              $": {existingUser.pos}, 角色名: {existingUser.roleName}");
                }

                // 3. 广播新玩家信息给其他所有人（原逻辑） 把新玩家信息广播给其他人 to all 
                var clientIDs = ctx.clientIDs;
                SpawnRoleBroMessage newPlayerBro = new SpawnRoleBroMessage {
                    pos = randomPos,
                    roleName = req.roleName
                };
                byte[] data = MessageHelper.ToData(newPlayerBro);

                for (int i = 0; i < clientIDs.Count; i++) {
                    // id 发给这个id的人
                    int id = clientIDs[i];
                    if (id != connID) { // 不发给自己
                        ctx.server.Send(id, data);
                        Debug.Log($"广播 SpawnRole_Bro 给其他玩家 {id}: {newPlayerBro.pos}");
                    }
                }

            }
        }

    }
}