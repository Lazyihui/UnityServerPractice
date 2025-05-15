using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using Unity.VisualScripting;

namespace ServerMain {

    public class ServerMain : MonoBehaviour {

        Server server;
        int port = 7777;
        int messageSize = 1024;
        bool isTearDown = false;

        void Start() {
            Application.runInBackground = true; // 允许后台运行

            server = new Server(messageSize);
            server.Start(port);
            Debug.Log("服务器启动成功: " + port);


            server.OnConnected += (connID, str) => {
                Debug.Log("服务端链接 " + connID);
            };

            server.OnData += (connID, data) => {
                Debug.Log("服务端接收数据 " + connID + " " + System.Text.Encoding.UTF8.GetString(data));
                int typeID = MessageHelper.ReadHeader(data.Array);
                // 1.会接收到信息
                Debug.Log("服务端接收消息类型: " + typeID);
                if (typeID == MessageConst.SpawnRole_Req) {
                    Debug.Log("收到信息");
                    var req = MessageHelper.ReadDate<SpawnRoleReqMessage>(data.Array);

                    Debug.Log("服务端接收 SpawnRole_Res: " + req.idSig + " " + req.pos);
                } else if (typeID == 1) {

                } else {
                    Debug.LogError("未知的消息类型: " + typeID);
                }

                // 2.回发信息
            };

            server.OnDisconnected += (connID) => {
                Debug.Log("服务端断开链接 " + connID);
            };


        }

        void Update() {
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
