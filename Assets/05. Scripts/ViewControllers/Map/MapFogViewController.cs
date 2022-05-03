using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
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
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
                OnLevelSelected(null, null);
            });

            //Debug.Log("MapFogViewController: Start");
        }


        private void OnLevelSelected(GraphVertex prevNode, GraphVertex newNode) {
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {

                if (newNode == null)
                { //floor 0 or loaded
                    List<GraphVertex> floor0Nodes = this.GetSystem<IGameMapGenerationSystem>().GetPathNodeAtDepth(this.GetModel<IMapGenerationModel>().PathDepth);
                    floor0Nodes.ForEach(node=>{UpdateMapFogForNode(null, node);});
                    Debug.Log("Count: " +floor0Nodes.Count);
                    List<GraphVertex> allNodes = this.GetSystem<IGameMapGenerationSystem>().GetAllAvailableNodes();
                    if (allNodes.Count > 0) {
                        allNodes.ForEach(node => {
                            if (node.Value. Visited) {
                                node.Neighbours.ForEach(n => UpdateMapFogForNode(node, n));
                            }
                          
                        });
                    }
                }
                else
                {
                    if (newNode.Neighbours.Count > 0) {
                        newNode.Neighbours.ForEach(node => {UpdateMapFogForNode(newNode, node);});
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

        private void UpdateMapFogForNode(GraphVertex fromNode, GraphVertex toNode) {
            if(toNode== null || toNode.Value==null || toNode.Value.LevelObject==null) {return;}

            Vector2 targetObjPos = toNode.Value.LevelObject.gameObject.transform.position;
            if (fromNode == null) {
                Debug.Log(vertices.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    float distance = Vector2.SqrMagnitude(new Vector2( vertices[i].x, vertices[i].y) - targetObjPos);

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
                    float distance = DistancePointLine(vertices[i],
                        fromNode.Value. LevelObject.gameObject.transform.position,
                        toNode.Value.LevelObject.gameObject.transform.position);


                    if (distance < radius * Random.Range(0.8f, 1.2f))
                    {
                        float alpha = Mathf.Min(targetColors[i].a, distance / (radius));
                        targetColors[i].a = alpha;
                    }
                }
            }
           

            
           
            //mesh.colors = colors;
           
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) => Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        /// <summary>
        ///   <para>Project point onto a line.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        public static Vector3 ProjectPointLine(
            Vector3 point,
            Vector3 lineStart,
            Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector3 = lineEnd - lineStart;
            float magnitude = vector3.magnitude;
            Vector3 lhs = vector3;
            if ((double)magnitude > 9.99999997475243E-07)
                lhs /= magnitude;
            float num = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0.0f, magnitude);
            return lineStart + lhs * num;
        }
    }
}
