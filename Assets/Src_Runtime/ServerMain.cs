using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;


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
            server.Send(connID, data);
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
