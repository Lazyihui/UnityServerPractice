using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using Unity.VisualScripting;

namespace ServerMain {

    public class ServerMain : MonoBehaviour {
        ServerContext ctx;

        bool isTearDown = false;

        void Start() {
            Application.runInBackground = true; // 允许后台运行
            ctx = new ServerContext();
            var server = ctx.server;

            server.OnConnected += (connID, str) => {
                Debug.Log("服务端链接 " + connID + "这个是什么 " + str);
                ctx.clientIDs.Add(connID);

            };

            server.OnData += (connID, data) => {
                int typeID = MessageHelper.ReadHeader(data.Array);

                // 1.会接收到信息
                if (typeID == MessageConst.SpawnRole_Req) {
                    var req = MessageHelper.ReadDate<SpawnRoleReqMessage>(data.Array);
                    // 广播回传
                    Debug.Log("服务端接收 SpawnRole_Res: " + req.idSig + " " + req.pos);
                    
                    OnMessageDomain.OnSpawnRole(connID, req, ctx);

                } else if (typeID == MessageConst.Test_Res) {
                    var req = MessageHelper.ReadDate<TestReqMessage>(data.Array);

                    // 广播回传
                    Debug.Log("服务端接收 Test_Res:" + " " + req.pos);


                } else {
                    Debug.LogError("未知的消息类型: " + typeID);
                }

                // 2.回发信息
            };

            server.OnDisconnected += (connID) => {
                Debug.Log("服务端断开链接 " + connID);
                ctx.clientIDs.Remove(connID);
            };

        }

        void Update() {
            var server = ctx.server;

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
