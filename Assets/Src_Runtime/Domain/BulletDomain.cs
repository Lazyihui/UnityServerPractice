using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using MyTelepathy;

namespace ServerMain {

    public static class BulletDomain {

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
                    iDSignature = bullet.idSig,
                    position = bullet.pos
                };

                // 发送给所有玩家
                byte[] date = MessageHelper.ToData(bro);
                foreach (var clientID in ctx.clientIDs) {
                    ctx.server.Send(clientID, date);
                }

                // 检查是否出界
                // 检查子弹是否超出边界
                if (IsOutOfBounds(bullet.pos)) {
                    // 移除子弹
                    ctx.bulletRepo.Remove(bullet);
                    // 发送销毁消息
                    BulletDestoryBroMessage destroyBro = new BulletDestoryBroMessage {
                        iDSignature = bullet.idSig,
                    };

                    byte[] destroyData = MessageHelper.ToData(destroyBro);
                    foreach (int clientID in ctx.clientIDs) {
                        ctx.server.Send(clientID, destroyData);
                    }
                }

            }

        }
        static bool IsOutOfBounds(Vector3 position) {
            // 定义边界值
            float boundary = 20f;
            return position.x < -boundary || position.x > boundary ||
                   position.y < -boundary || position.y > boundary;
        }

    }
}