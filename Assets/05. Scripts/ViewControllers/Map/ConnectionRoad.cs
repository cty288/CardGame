using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainGame
{
    public class ConnectionRoad : PoolableGameObject, IController {
        [HideInInspector]
        [FormerlySerializedAs("Node1")] public GraphVertex Vertex1;
        [HideInInspector]
        [FormerlySerializedAs("Node2")] public GraphVertex Vertex2;

        private Tween animationTween;
        public override void OnInit() {
            transform.SetParent(Pool.transform);
            this.RegisterEvent<OnTemporaryBlockedUnDirectedEdgeAdded>(OnRoadBlocked)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnTemporaryBlockedUnDirectedEdgeRecovered>(OnRoadRecovered)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            StartCoroutine(LateInit());
        }

        private IEnumerator LateInit() {
            yield return null;
            RefreshMaterial();

        }

        private void RefreshMaterial() {
            if (Vertex1.TemporaryBrokenConnections.Contains(Vertex2.Value) ||
                Vertex2.TemporaryBrokenConnections.Contains(Vertex1.Value)) {
                animationTween = this.GetComponent<MeshRenderer>().material.DOColor(Color.red, 1f)
                    .SetLoops(-1, LoopType.Yoyo);
            }
           
        }

        private void OnRoadRecovered(OnTemporaryBlockedUnDirectedEdgeRecovered e) {
           if ((e.toNode.Equals(Vertex1.Value) && e.fromNode.Equals(Vertex2.Value)) || (e.fromNode.Equals(Vertex1.Value) && 
                    e.toNode .Equals(Vertex2.Value)))
            {
                if (animationTween != null) {
                    animationTween.Kill();
                    animationTween = null;
                }
                
                this.GetComponent<MeshRenderer>().material.DOColor(Color.white, 1f);
            }else if (!Vertex1.TemporaryBrokenConnections.Contains(Vertex2.Value) &&
                      !Vertex2.TemporaryBrokenConnections.Contains(Vertex1.Value)) {
                this.GetComponent<MeshRenderer>().material.DOColor(Color.white, 1f);
            }
         
        }

        private void OnRoadBlocked(OnTemporaryBlockedUnDirectedEdgeAdded e) {
            if ((e.toNode.Equals(Vertex1.Value) && e.fromNode.Equals(Vertex2.Value)) || (e.fromNode.Equals(Vertex1.Value) && 
                    e.toNode .Equals(Vertex2.Value)))
            {
                if (animationTween == null || !animationTween.IsActive()) {
                    animationTween = this.GetComponent<MeshRenderer>().material.DOColor(Color.red, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
                }
            }
        }

        public override void OnRecycled() {
            transform.SetParent(Pool.transform);
            Vertex1 = null;
            Vertex2 = null;
            animationTween = null;
            this.UnRegisterEvent<OnTemporaryBlockedUnDirectedEdgeAdded>(OnRoadBlocked);
            this.UnRegisterEvent<OnTemporaryBlockedUnDirectedEdgeRecovered>(OnRoadRecovered);
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
