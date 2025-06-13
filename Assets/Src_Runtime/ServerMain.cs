using Telepathy;
using MyTelepathy;
using UnityEngine;

// TODO: 认为可以修改的点 但是还没修改， 同一个玩家进入的时候 可以根据名字来判断之前有没有 如果有idsig就不需要重新生成

namespace ServerMain {
    public class ServerMain : MonoBehaviour {
        ServerContext ctx;

        bool isTearDown = false;

        void Start() {

            Application.runInBackground = true; // 允许后台运行
            ctx = new ServerContext();
            var server = ctx.server;

            server.OnConnected += (connID, str) => {
                ctx.clientIDs.Add(connID);
            };

            server.OnData += (connID, data) => {
                int typeID = MessageHelper.ReadHeader(data.Array);

                // 1.会接收到信息
                if (typeID == MessageConst.SpawnRole_Req) {
                    var req = MessageHelper.ReadDate<SpawnRoleReqMessage>(data.Array);
                    // 回发给自己生成自己 生成场上的角色
                    OnMessageDomain.OnSpawnRoleRes(connID, req, ctx);
                    Debug.Log("SpawnRole_Req: " + req.roleName + " connID: " + connID);

                } else if (typeID == MessageConst.Move_Req) {

                    var req = MessageHelper.ReadDate<MoveReqMessage>(data.Array);
                    // 处理移动请求
                    OnMessageDomain.OnMoveReq(connID, req, ctx);

                } else if (typeID == MessageConst.BulletSpawn_Req) {

                    var req = MessageHelper.ReadDate<SpawnBulletReqMessage>(data.Array);
                    BulletDomain.OnSpawnBulletReq(connID, req, ctx);

                } else if (typeID == MessageConst.BulletDestory_Req) {
                    var req = MessageHelper.ReadDate<BulletDestoryReqMessage>(data.Array);

                } else if (typeID == MessageConst.StuffDestory_Req) {

                    var req = MessageHelper.ReadDate<StuffDestoryReqMessage>(data.Array);

                }

            };

            server.OnDisconnected += (connID) => {
                Debug.Log("服务端断开链接 " + connID);
                ctx.clientIDs.Remove(connID);
                Debug.Log($"当前在线用户数: {ctx.clientIDs.Count}");

                // string userNameToRemove = null;
                // foreach (var User in ctx.userMap) {
                //     if (User.Value.connID == connID) {
                //         userNameToRemove = User.Key;
                //         break;
                //     }
                // }

                UserEntity destoryUser = null;
                foreach (var user in ctx.userMap) {
                    if (user.Value.connID == connID) {
                        destoryUser = user.Value;
                        break;
                    }
                }

                if( destoryUser != null) {
                    Debug.Log($"已移除用户: {destoryUser.roleName}");
                    // 广播销毁信息
                    OnMessageDomain.OnRoleDestoryBro(connID, destoryUser, ctx);
                }

                // if (userNameToRemove != null) {
                //     ctx.userMap.Remove(userNameToRemove);
                //     Debug.Log($"已移除用户: {userNameToRemove}");
                //     // 广播销毁信息
                //     OnMessageDomain.OnRoleDestoryReq(connID, ctx);
                // }
            };


        }

        void Update() {
            var server = ctx.server;

            float dt = Time.deltaTime;
            // 子弹移动
            BulletDomain.MoveAllBullets(ctx, dt);
            // Bullet 
            int lenbullet = ctx.bulletRepo.TakeAll(out var bullets);
            for (int i = 0; i < lenbullet; i++) {
                var blt = bullets[i];
                BulletDomain.OnHitStuff(ctx, blt);
            }


            // Stuff
            int lenStuff = ctx.stuffRepo.TakeAll(out var stuffs);
            for (int i = 0; i < lenStuff; i++) {
                var stuff = stuffs[i];
                // 物品自由落体
                StuffDomain.FreeFallingMove(ctx, stuff, dt);
            }

            if (ctx.clientIDs.Count > 0) {
                var game = ctx.gameEntity;
                game.spawnMstTimer += dt;
                if (game.spawnMstTimer >= game.spawnMstInterval) {
                    game.spawnMstTimer = 0;
                    // Debug.Log("生成怪物");
                    StuffDomain.OnSpawnStuff(ctx);
                }
            }


            if (server != null) {
                server.Tick(100); // 每秒30帧
            }

        }

        void OnDestroy() {
            TearDown();
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void TearDown() {
            var server = ctx.server;
            if (isTearDown) {
                return;
            }

            isTearDown = true;
            if (server != null) {
                server.Stop();
            }
        }
    }
}
