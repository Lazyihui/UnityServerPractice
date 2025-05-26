using System.Collections.Generic;
using MyTelepathy;

namespace ServerMain {

    public class RoleRepository {

        Dictionary<int, RoleEntity> all;

        RoleEntity[] temArray;

        public RoleRepository() {
            all = new Dictionary<int, RoleEntity>();
            temArray = new RoleEntity[100];
        }

        public void Add(RoleEntity entity) {
            all.Add(entity.idSig, entity);
        }

        public void Remove(RoleEntity entity) {
            all.Remove(entity.idSig);
        }

        public int TakeAll(out RoleEntity[] array) {
            if (all.Count > temArray.Length) {
                temArray = new RoleEntity[all.Count * 2];
            }
            all.Values.CopyTo(temArray, 0);
            array = temArray;
            return all.Count;
        }

        public bool TryGet(int idSig, out RoleEntity entity) {
            return all.TryGetValue(idSig, out entity);
        }

        public bool TryGetByString(string roleName, out RoleEntity entity) {
            foreach (var kvp in all) {
                if (kvp.Value.roleName == roleName) {
                    entity = kvp.Value;
                    return true;
                }
            }
            entity = null;
            return false;
        }


        public void Clear() {
            all.Clear();
        }
    }
}