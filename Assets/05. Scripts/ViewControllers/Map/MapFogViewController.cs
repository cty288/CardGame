using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainGame
{
    public class MapFogViewController : AbstractMikroController<CardGame>
    {
        [SerializeField] private float radius = 5;
        public float Radius => radius;

        private Mesh mesh;
        private Vector3[] vertices;
        public Vector3[] Vertices => vertices;
        private Color[] targetColors;
        private Color[] colors;
        public Color[] Colors => colors;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;
            colors = new Color[vertices.Length];
            targetColors = new Color[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.black;
                targetColors[i] = Color.black;
                vertices[i] = transform.TransformPoint(vertices[i]);
            }

            mesh.colors = colors;
        }

        private void Start()
        {
            this.GetModel<IMapStateModel>().CurNode.RegisterOnValueChaned(OnLevelSelected)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            OnLevelSelected(null,null);
        }


        private void OnLevelSelected(PathNode prevNode, PathNode newNode) {
            this.GetSystem<ITimeSystem>().AddDelayTask(0.06f, () => {
                if (newNode == null)
                { //floor 0 or loaded
                    List<PathNode> floor0Nodes = this.GetSystem<IGameMapSystem>().GetPathNodeAtDepth(this.GetModel<IMapGenerationModel>().PathDepth);
                    floor0Nodes.ForEach(node=>{UpdateMapFogForNode(null, node);});

                    List<PathNode> allNodes = this.GetSystem<IGameMapSystem>().GetAllAvailableNodes();
                    if (allNodes.Count > 0) {
                        allNodes.ForEach(node => {
                            if (node.Visited) {
                                node.ConnectedByNodes.ForEach(n => UpdateMapFogForNode(node, n));
                            }
                          
                        });
                    }
                }
                else
                {
                    if (newNode.ConnectedByNodes.Count > 0) {
                        newNode.ConnectedByNodes.ForEach(node => {UpdateMapFogForNode(newNode, node);});
                    }
                }
              
            });
            
        }

        private void Update() {
            for (int i = 0; i < targetColors.Length; i++) {
                colors[i].a = Mathf.Lerp(colors[i].a, targetColors[i].a, 2 * Time.deltaTime);
            }

            mesh.colors = colors;
        }

        private void UpdateMapFogForNode(PathNode fromNode, PathNode toNode) {
            Vector3 targetObjPos = toNode.LevelObject.gameObject.transform.position;
            if (fromNode == null) {
                for (int i = 0; i < vertices.Length; i++)
                {
                    float distance = Vector3.SqrMagnitude(vertices[i] - targetObjPos);

                    if (distance < radius * radius * Random.Range(0.5f, 1.5f))
                    {
                        float alpha = Mathf.Min(targetColors[i].a, distance / (radius * radius));
                        //DOTween.To(() => myValue, x => myValue = x, int / double / Vector3... finalValue, float time)
                        targetColors[i].a = alpha;
                        // DOTween.To(()=>colors[i].a, x=>colors[i].a = x, alpha, 0.3f);
                        
                        //mesh.colors[i] = colors[i];
                    }
                }
            }
            else {
                for (int i = 0; i < vertices.Length; i++) {
                    float distance = HandleUtility.DistancePointLine(vertices[i],
                        fromNode.LevelObject.gameObject.transform.position,
                        toNode.LevelObject.gameObject.transform.position);


                    if (distance < radius * Random.Range(0.8f, 1.2f))
                    {
                        float alpha = Mathf.Min(targetColors[i].a, distance / (radius));
                        targetColors[i].a = alpha;
                    }
                }
            }
           

            
           
            //mesh.colors = colors;
           
        }
    }
}
