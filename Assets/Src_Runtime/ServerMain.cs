using Telepathy;
using MyTelepathy;
using UnityEngine;

namespace ServerMain {

    public class ServerMain : MonoBehaviour {
        ServerContext ctx;

        bool isTearDown = false;

        // 广播间隔
        private float lastBroadcastTime;
        public float broadcastInterval = 0.033f; // ~30次/秒

        void Start() {
            Debug.Log("ServerMain Start");

            Application.runInBackground = true; // 允许后台运行
            ctx = new ServerContext();
            var server = ctx.server;

            server.OnConnected += (connID, str) => {
                Debug.Log("服务端连接成功 " + connID + " " + str);
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
                    var req = MessageHelper.ReadDate<BulletDestoryReqMessage>(data.Array);

                } else if (typeID == MessageConst.StuffDestory_Req) {

                    var req = MessageHelper.ReadDate<StuffDestoryReqMessage>(data.Array);

                }

            };

            server.OnDisconnected += (connID) => {
                Debug.Log("服务端断开链接 " + connID);
                ctx.clientIDs.Remove(connID);

                // 这里是一个问题 怎么样移除 TODO:5.18
                // ctx.userMap.Remove();

            };

        }
        // TODO:再创建消息体合并发送消息
        void Update() {
            var server = ctx.server;

            float dt = Time.deltaTime;
            BulletDomain.MoveAllBullets(ctx, dt);

            // 每隔一段时间广播一次
            lastBroadcastTime += dt;
            if (lastBroadcastTime >= broadcastInterval) {
                lastBroadcastTime = 0f;
                // 广播所有玩家信息
                BulletDomain.Tick(ctx, dt);
                StuffDomain.Tick(ctx, dt);
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
                server.Tick(10000); // 每秒30帧
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
