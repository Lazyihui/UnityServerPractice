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
            Debug.Log($"Spawned stuff: {entity.idSig} at {pos}");
        }
    }
}