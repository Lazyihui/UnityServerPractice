using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using MyTelepathy;
using System.ComponentModel;
using Codice.CM.Common;

namespace ServerMain {

    public static class BulletDomain {

        public static void OnSpawnBulletReq(int connID, SpawnBulletReqMessage req, ServerContext ctx) {
            // 获取发射者
            if (!ctx.userMap.TryGetValue(req.belongName, out UserEntity owner)) {
                Debug.LogWarning($"非法子弹发射请求：角色 {req.belongName} 不存在");
                return;
            }

            BulletEntity bulletEntity = new BulletEntity {
                idSig = ctx.idServer.PickBulletID(),
                rootPos = req.rootPos,
                direction = req.dir, // 添加方向
                pos = req.pos,
                belongIdSig = owner.idSig, // 设置发射者ID
            };

            ctx.bulletRepo.Add(bulletEntity);

            SpawnBulletBroMessage bro = new SpawnBulletBroMessage {
                idSig = bulletEntity.idSig,
                rootPos = req.rootPos,
                dir = req.dir,
            };

            byte[] data = MessageHelper.ToData(bro);
            foreach (int clientID in ctx.clientIDs) {
                ctx.server.Send(clientID, data);
            }
        }
        // 这个应该从配置文件读取
        public const float BULLET_SPEED = 10f;

        public static void MoveAllBullets(ServerContext ctx, float dt) {
            int len = ctx.bulletRepo.TakeAll(out BulletEntity[] bullets);

            for (int i = 0; i < len; i++) {
                BulletEntity bullet = bullets[i];
                // 更新子弹位置
                bullet.pos += bullet.direction * BULLET_SPEED * dt;

                // 广播移动消息
                BulletMoveBroMessage bro = new BulletMoveBroMessage {
                    idSig = bullet.idSig,
                    position = bullet.pos
                };

                // 发送给所有玩家
                byte[] date = MessageHelper.ToData(bro);
                foreach (var clientID in ctx.clientIDs) {
                    ctx.server.Send(clientID, date);
                }

                // 检查子弹是否超出边界
                if (IsOutOfBounds(bullet.pos)) {
                    // 移除子弹

                    ctx.bulletRepo.Remove(bullet);
                    // 发送销毁消息
                    BulletDestoryBroMessage destroyBro = new BulletDestoryBroMessage();
                    destroyBro.Init(bullet.idSig);

                    byte[] destroyData = MessageHelper.ToData(destroyBro);
                    foreach (int clientID in ctx.clientIDs) {
                        ctx.server.Send(clientID, destroyData);
                    }
                }

            }

        }
        static bool IsOutOfBounds(Vector3 position) {
            // 定义边界值
            bool IsOutOfBounds = false;
            if (position.y > 10) {
                IsOutOfBounds = true;
            }

            return IsOutOfBounds;
        }

        public static void OnHitStuff(ServerContext ctx, BulletEntity blt) {

            int len = ctx.stuffRepo.TakeAll(out StuffEntity[] stuffs);

            for (int i = 0; i < len; i++) {
                StuffEntity stuff = stuffs[i];

                float distance = Vector3.Distance(blt.pos, stuff.pos);

                if (distance < 0.5f) { // 假设1.0f是子弹与物体的碰撞距离
                    Debug.Log($"子弹 {blt.idSig} 命中物品 {stuff.idSig}，位置: {stuff.pos}");
                }

            }
        }

    }
}