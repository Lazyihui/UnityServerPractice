using System;
using UnityEngine;


namespace ServerMain {

    public static class OnMessageDomain {

        public static void OnSpawnRole(int connID, SpawnRoleReqMessage req, ServerContext ctx) {
            // 1. 处理 SpawnRole 请求 

            // 2. 构建 SpawnRole 响应 回传给本人

            // 3. 广播 SpawnRole 响应 给所有人
            var clientIDs = ctx.clientIDs;

            for (int i = 0; i < clientIDs.Count; i++) {
                int id = clientIDs[i];
                Debug.Log("广播给 " + id + "存的长度" + clientIDs.Count);
                if (id == connID) {
                    // 只给自己
                    SpawnRoleResMessage res = new SpawnRoleResMessage();
                    res.idSig = req.idSig;
                    res.pos = req.pos;
                    byte[] dataOwner = MessageHelper.ToData(res);

                    ctx.server.Send(id, dataOwner);
                    continue;
                }

                // 广播给其他人
                SpawnRoleBroMessage bro = new SpawnRoleBroMessage();
                bro.idSig = req.idSig;
                bro.pos = req.pos;

                byte[] data = MessageHelper.ToData(bro);

                ctx.server.Send(id, data);
            }

        }
    }
}