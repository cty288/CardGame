using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    public class MovingEnemy : PoolableGameObject, IController
    {
        public GraphVertex targetMoveVertex;

        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float maxMoveTime = 5f;
        public override void OnInit() {
            StartCoroutine(OnLateInit());
        }

        IEnumerator OnLateInit() {
            yield return null;
            //calculate time

            float time = (Vector2.Distance(targetMoveVertex.Value.LevelObject.transform.position, transform.position) /
                          moveSpeed);
            time = Mathf.Min(maxMoveTime, time);
            this.GetSystem<IMapAnimationControlSystem>().AddUnblockableAsyncAnimation(DelayAction.Allocate(time));

            transform.DOMove(targetMoveVertex.Value.LevelObject.transform.position, time).OnComplete(
                ()=> {
                    targetMoveVertex.Value.LevelObject.GetComponent<LevelObject>().UpdateNodeSprite();
                    RecycleToCache();
                });
        }

        public override void OnRecycled() {
            targetMoveVertex = null;
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
