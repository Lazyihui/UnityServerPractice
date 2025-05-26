using System.Collections.Generic;
using MyTelepathy;

namespace ServerMain {

    public class StuffRepository {

        Dictionary<int, StuffEntity> all;

        StuffEntity[] temArray;

        public StuffRepository() {
            all = new Dictionary<int, StuffEntity>();
            temArray = new StuffEntity[100];
        }

        public void Add(StuffEntity entity) {
            all.Add(entity.idSig, entity);
        }

        public void Remove(StuffEntity entity) {
            all.Remove(entity.idSig);
        }

        public int TakeAll(out StuffEntity[] array) {
            if (all.Count > temArray.Length) {
                temArray = new StuffEntity[all.Count * 2];
            }
            all.Values.CopyTo(temArray, 0);
            array = temArray;
            return all.Count;
        }

        public bool TryGet(int idSig, out StuffEntity entity) {
            return all.TryGetValue(idSig, out entity);
        }

        public void Clear() {
            all.Clear();
        }
    }
}