using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    public class MovingEnemy : PoolableGameObject, IController
    {
        public GraphVertex targetMoveVertex;

        [SerializeField] private float moveSpeed = 1f;
        public override void OnInit() {
            StartCoroutine(OnLateInit());
        }

        IEnumerator OnLateInit() {
            yield return null;
            //calculate time
            float time = (Vector2.Distance(targetMoveVertex.Value.LevelObject.transform.position, transform.position) /
                          moveSpeed);
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
