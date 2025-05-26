using Telepathy;
using MyTelepathy;
using UnityEngine;

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

                } else if (typeID == MessageConst.Move_Req) {

                    var req = MessageHelper.ReadDate<MoveReqMessage>(data.Array);
                    // 处理移动请求
                    OnMessageDomain.OnMoveReq(connID, req, ctx);

                } else if (typeID == MessageConst.BulletSpawn_Req) {

                    var req = MessageHelper.ReadDate<SpawnBulletReqMessage>(data.Array);
                    BulletDomain.OnSpawnBulletReq(connID, req, ctx);

                } else if (typeID == MessageConst.BulletDestory_Req) {

                }

            };

            server.OnDisconnected += (connID) => {
                Debug.Log("服务端断开链接 " + connID);
                ctx.clientIDs.Remove(connID);

                // 这里是一个问题 怎么样移除 TODO:5.18
                // ctx.userMap.Remove();
            };

        }

        void Update() {
            var server = ctx.server;

            float dt = Time.deltaTime;
            // 子弹移动
            BulletDomain.MoveAllBullets(ctx, dt);

            // Stuff
            int len = ctx.stuffRepo.TakeAll(out var stuffs);

            for (int i = 0; i < len; i++) {
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
