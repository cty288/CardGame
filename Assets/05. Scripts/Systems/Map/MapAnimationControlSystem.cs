using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public interface IMapAnimationControlSystem: ISystem {
        public void AddUnblockableAsyncAnimation(MikroAction animation);
        public bool IsBlockable { get; }
    }
    public class MapAnimationControlSystem : AbstractSystem, IMapAnimationControlSystem {
        private Spawn unblockableAnimationSpawn;
        protected override void OnInit() {
            ResetMapAnimation();
        }
        private void ResetMapAnimation()
        {
            if (unblockableAnimationSpawn != null)
            {
                unblockableAnimationSpawn.RecycleToCache();
            }
            unblockableAnimationSpawn = null;

        }
        public void AddUnblockableAsyncAnimation(MikroAction animation) {
            bool spawnReady = !(unblockableAnimationSpawn == null || unblockableAnimationSpawn.Finished.Value || unblockableAnimationSpawn.IsRecycled);
            if (!spawnReady)
            {
                if (unblockableAnimationSpawn != null)
                {
                    unblockableAnimationSpawn.RecycleToCache();
                }

                unblockableAnimationSpawn = Spawn.Allocate();

            }
            unblockableAnimationSpawn.AddAction(animation);

            if (!spawnReady)
            {
                unblockableAnimationSpawn.Execute();
            }
        }

        public bool IsBlockable {
            get {
                if (unblockableAnimationSpawn == null || unblockableAnimationSpawn.Finished.Value ||
                    unblockableAnimationSpawn.IsRecycled) {
                    return false;
                }

                return !unblockableAnimationSpawn.Finished;
            }
        }
    }
}
