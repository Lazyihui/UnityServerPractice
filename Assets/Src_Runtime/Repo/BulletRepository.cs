using System.Collections.Generic;
using MyTelepathy;

namespace ServerMain {

    public class BulletRepository {

        Dictionary<IDSignature, BulletEntity> all;

        BulletEntity[] temArray;

        public BulletRepository() {
            all = new Dictionary<IDSignature, BulletEntity>();
            temArray = new BulletEntity[100];
        }

        public void Add(BulletEntity entity) {
            all.Add(entity.idSig, entity);
        }

        public void Remove(BulletEntity entity) {
            all.Remove(entity.idSig);
        }

        public int TakeAll(out BulletEntity[] array) {
            if (all.Count > temArray.Length) {
                temArray = new BulletEntity[all.Count * 2];
            }
            all.Values.CopyTo(temArray, 0);
            array = temArray;
            return all.Count;
        }

        public bool TryGet(IDSignature idSig, out BulletEntity entity) {
            return all.TryGetValue(idSig, out entity);
        }

        public void Clear() {
            all.Clear();
        }
    }
}