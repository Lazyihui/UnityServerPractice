using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unity.VisualScripting;

namespace ServerMain {
    public static class MessageHelper {

        static Dictionary<Type, int> typeIDMap = new Dictionary<Type, int>() {
            { typeof(SpawnRoleReqMessage), MessageConst.SpawnRole_Req },
            { typeof(SpawnRoleResMessage), MessageConst.SpawnRole_Res },
            { typeof(SpawnRoleBroMessage), MessageConst.SpawnRole_Bro },
             { typeof(TestReqMessage), MessageConst.Test_Req },
              {typeof(MoveReqMessage), MessageConst.Move_Req },
            {typeof(MoveResMessage), MessageConst.Move_res },
            {typeof(MoveBroMessage),MessageConst.Move_Bro}
        };


        public static int GetTypeID<T>() {
            Type type = typeof(T);
            if (typeIDMap.ContainsKey(type)) {
                return typeIDMap[type];
            } else {
                Debug.LogError("Type not found in typeIDMap: " + type);
                return -1;
            }
        }

        public static byte[] ToData<T>(T msg) {
            string str = JsonUtility.ToJson(msg);
            int typeID = GetTypeID<T>();

            byte[] msg_header = BitConverter.GetBytes(typeID);
            byte[] msg_data = Encoding.UTF8.GetBytes(str);
            byte[] msg_length = BitConverter.GetBytes(msg_data.Length);

            byte[] data = new byte[msg_header.Length + msg_length.Length + msg_data.Length];// 头部 + 长度 + 数据 4+4+N

            Buffer.BlockCopy(msg_header, 0, data, 0, msg_header.Length);
            Buffer.BlockCopy(msg_length, 0, data, msg_header.Length, msg_length.Length);
            Buffer.BlockCopy(msg_data, 0, data, msg_header.Length + msg_length.Length, msg_data.Length);

            return data;

        }

        public static int ReadHeader(byte[] data) {
            if (data.Length < 4) {
                return -1;
            } else {
                int typeID = BitConverter.ToInt32(data, 0);
                return typeID;
            }
        }

        public static T ReadDate<T>(byte[] data) where T : struct {
            if (data.Length < 4) {
                return default;
            } else {
                int typeID = ReadHeader(data);
                if (typeID != GetTypeID<T>()) {
                    Debug.LogError("TypeID not match: " + typeID + " " + GetTypeID<T>());
                    return default;
                } else {
                    int Length = BitConverter.ToInt32(data, 4);
                    string str = Encoding.UTF8.GetString(data, 8, Length);
                    T msg = JsonUtility.FromJson<T>(str);
                    return msg;
                }
            }

        }
    }
}