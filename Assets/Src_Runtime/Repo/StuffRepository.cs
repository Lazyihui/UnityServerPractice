using System.Collections.Generic;
using MyTelepathy;

namespace ServerMain {

    public class StuffRepository {

        Dictionary<IDSignature, StuffEntity> all;

        StuffEntity[] temArray;

        public StuffRepository() {
            all = new Dictionary<IDSignature, StuffEntity>();
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

        public bool TryGet(IDSignature idSig, out StuffEntity entity) {
            return all.TryGetValue(idSig, out entity);
        }

        public void Clear() {
            all.Clear();
        }
    }
}