using System;
using Telepathy;
using UnityEngine;
using MyTelepathy;


namespace ServerMain {

    public static class OnMessageDomain {

        public static void OnSpawnRoleRes(int connID, SpawnRoleReqMessage req, ServerContext ctx) {
            // 这里的req只传了名字;
            Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0f);
            // 2. 将新玩家信息存入 userMap（检查重复）
            if (ctx.userMap.ContainsKey(req.roleName)) {
                Debug.LogWarning($"角色名已存在: {req.roleName}");
                return;
            }
            UserEntity userEntity = new UserEntity();
            userEntity.Init(ctx.idServer.PickRoleID(), req.roleName, connID, randomPos);
            ctx.AddUserEntity(req.roleName, userEntity);

            // 1. 为新玩家生成角色并返回响应
            SpawnRoleResMessage res = new SpawnRoleResMessage();
            res.Init(RoleType.Player, userEntity.idSig, req.roleName, randomPos);

            byte[] resData = MessageHelper.ToData(res);
            ctx.server.Send(connID, resData);

            // 3. 向新玩家同步所有已存在的角色信息
            foreach (var existingUser in ctx.userMap.Values) {

                if (existingUser.connID != connID) {

                    SpawnRoleBroMessage bro = new SpawnRoleBroMessage();
                    bro.Init(RoleType.Player, existingUser.idSig, existingUser.roleName, existingUser.pos);

                    ctx.server.Send(connID, MessageHelper.ToData(bro));
                }
            }

            // 4. 广播新玩家信息给其他所有人把新玩家信息广播给其他人 to all 
            var clientIDs = ctx.clientIDs;
            SpawnRoleBroMessage newPlayerBro = new SpawnRoleBroMessage();
            newPlayerBro.Init(req.roleType, userEntity.idSig, req.roleName, randomPos);
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

        public static void OnRoleDestoryBro(int connID, UserEntity user, ServerContext ctx) {

            // 1. 从 userMap 中移除用户
            if (user == null) {
                Debug.LogWarning($"无法销毁角色：用户信息为空");
                return;
            }
            ctx.userMap.Remove(user.roleName);

            // 2. 广播销毁消息给所有客户端
            RoleDestoryBroMessage bro = new RoleDestoryBroMessage();
            bro.Init(bro.idSig, user.roleName);

            byte[] data = MessageHelper.ToData(bro);
            foreach (int clientID in ctx.clientIDs) {
                ctx.server.Send(clientID, data);
            }

        }

        public static void OnMoveReq(int connID, MoveReqMessage req, ServerContext ctx) {

            bool has = ctx.userMap.TryGetValue(req.roleName, out UserEntity user);

            if (!has) {
                Debug.LogWarning($"非法移动请求：角色 {req.roleName} 不存在");
                return;
            }

            // 2. 更新服务端位置
            user.pos = req.targetPos;
            // 3. 广播移动信息给所有客户端
            MoveBroMessage bro = new MoveBroMessage {
                roleName = req.roleName,
                targetPos = req.targetPos,
                timestamp = req.timestamp
            };
            byte[] data = MessageHelper.ToData(bro);

            foreach (int clientID in ctx.clientIDs) {
                ctx.server.Send(clientID, data);
            }

        }

    }
}