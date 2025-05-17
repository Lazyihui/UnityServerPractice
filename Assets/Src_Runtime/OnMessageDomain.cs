using System;
using Codice.Client.Commands.CheckIn;
using Telepathy;
using UnityEngine;
using UnityEngine.UIElements;


namespace ServerMain {

    public static class OnMessageDomain {

        public static void OnSpawnRoleBro(int connID, SpawnRoleReqMessage req, ServerContext ctx) {
            // 1. 处理 SpawnRole 请求 

            // 2. 构建 SpawnRole 响应 回传给本人

            // 3. 广播新玩家信息给其他所有人（原逻辑） 把新玩家信息广播给其他人 to all 
            var clientIDs = ctx.clientIDs;
            SpawnRoleBroMessage newPlayerBro = new SpawnRoleBroMessage {
                idSig = req.idSig,
                pos = req.pos,
                roleName = req.roleName
            };
            byte[] data = MessageHelper.ToData(newPlayerBro);

            for (int i = 0; i < clientIDs.Count; i++) {
                // id 发给这个id的人
                int id = clientIDs[i];
                if (id != connID) { // 不发给自己
                    ctx.server.Send(id, data);
                    Debug.Log($"广播 SpawnRole_Bro 给其他玩家 {id}: {newPlayerBro.idSig} {newPlayerBro.pos}");
                }
            }
        }

        public static void OnSpawnRoleRes(int connID, SpawnRoleReqMessage req, ServerContext ctx) {
            var clientIDs = ctx.clientIDs;
            // 1. 为新玩家生成角色并返回响应 生成自己
            SpawnRoleResMessage res = new SpawnRoleResMessage {
                idSig = req.idSig,
                pos = req.pos,
                roleName = req.roleName
            };
            byte[] resData = MessageHelper.ToData(res);
            ctx.server.Send(connID, resData);


            // 2. 将新玩家信息存入 userMap
            UserEntity userEntity = new UserEntity {
                roleName = req.roleName,
                connID = connID,
                idSig = req.idSig
            };
            ctx.AddUserEntity(req.roleName, userEntity);

            // 3. 向新玩家同步所有已存在的角色信息
            for (int i = 0; i < clientIDs.Count; i++) {
                int existingID = clientIDs[i];
                bool has = ctx.userMap.TryGetValue(req.roleName, out UserEntity existingUser);
                if (existingID != connID && has) {
                    SpawnRoleBroMessage bro = new SpawnRoleBroMessage {
                        idSig = existingUser.idSig,
                        pos = existingUser.pos,
                        roleName = existingUser.roleName
                    };
                    byte[] broData = MessageHelper.ToData(bro);
                    ctx.server.Send(connID, broData); // 发给新玩家

                    Debug.Log($"同步已有角色 {existingUser.idSig} 给新玩家 {connID}");
                }
            }

        }

        public static void OnSpawnRoleRes1(int connID, SpawnRoleReqMessage req, ServerContext ctx) {

            // 1. 为新玩家生成角色并返回响应
            SpawnRoleResMessage res = new SpawnRoleResMessage {
                idSig = req.idSig,
                pos = req.pos,
                roleName = req.roleName
            };
            byte[] resData = MessageHelper.ToData(res);
            ctx.server.Send(connID, resData);
            Debug.Log($"回发 SpawnRole_Res 给新玩家: {req.idSig}");

            // // 2. 向新玩家同步所有已存在的角色信息
            // foreach (int existingID in ctx.clientIDs) {
            //     if (existingID != connID && ctx.roleMap.TryGetValue(existingID, out RoleInfo existingRole)) {
            //         SpawnRoleBroMessage bro = new SpawnRoleBroMessage {
            //             idSig = existingRole.idSig,
            //             pos = existingRole.pos,
            //             roleName = existingRole.roleName
            //         };
            //         byte[] broData = MessageHelper.ToData(bro);
            //         ctx.server.Send(connID, broData); // 发给新玩家
            //         Debug.Log($"同步已有角色 {existingRole.idSig} 给新玩家 {connID}");
            //     }
            // }
        }


    }
}