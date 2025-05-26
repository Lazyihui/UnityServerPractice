using System;
using MyTelepathy;
using UnityEngine;

namespace ServerMain {

    public static class StuffDomain {

        public static void OnSpawnStuff(ServerContext ctx) {

            // 生成一个物品实体
            StuffEntity entity = new StuffEntity();
            entity.idSig = new IDSignature(EntityType.Stuff, ctx.idServer.PickStuffID());
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-18f, 18f), 12, 0);
            entity.pos = pos;

            ctx.stuffRepo.Add(entity);

            // 发送给所有客户端
            StuffSpawnBroMessage bro = new StuffSpawnBroMessage();
            bro.idSig = entity.idSig;
            bro.pos = pos;

            byte[] date = MessageHelper.ToData(bro);
            foreach (var clientID in ctx.clientIDs) {
                ctx.server.Send(clientID, date);
            }
            // Debug
        }

        // 临时写在这里

        public static void FreeFallingMove(ServerContext ctx, StuffEntity entity, float dt) {

            // 计算速度变化（v = v0 + a*t）
            entity.currentYVelocity -= entity.gravity * dt;
            // 计算位移（y = y0 + v*t）
            entity.pos += new Vector3(0, entity.currentYVelocity * dt, 0);
            // 广播
            StuffMoveBroMessage bro1 = new StuffMoveBroMessage();
            bro1.Init(entity.idSig, entity.pos);
            byte[] data1 = MessageHelper.ToData(bro1);
            foreach (var clientID in ctx.clientIDs) {
                ctx.server.Send(clientID, data1);
            }

            if (IsOutOfBounds(entity)) {
                ctx.stuffRepo.Remove(entity);
                // 广播移除
                StuffDestoryBroMessage bro = new StuffDestoryBroMessage();
                Debug.Log($"物品 {entity.idSig} 超出边界，已移除");
                bro.Init(entity.idSig);

                byte[] data = MessageHelper.ToData(bro);
                foreach (var clientID in ctx.clientIDs) {
                    ctx.server.Send(clientID, data);
                }
            }

        }

        static bool IsOutOfBounds(StuffEntity entity) {
            bool isOut = false;
            var pos = entity.pos;
            if (pos.y < -10) {
                isOut = true;
            }
            return isOut;
        }
    }
}