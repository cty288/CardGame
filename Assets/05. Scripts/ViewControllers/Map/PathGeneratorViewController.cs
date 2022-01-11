using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MainGame;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace MainGame
{
    public class PathGeneratorViewController : AbstractMikroController<CardGame>, ISingleton
    {
        public static PathGeneratorViewController Singleton {
            get {
                return SingletonProperty<PathGeneratorViewController>.Singleton;
            }
        }

        //public static PathGeneratorViewController Singleton;

        [Range(0f, 10.0f)]
        [SerializeField]
        private float cellXInterval = 1.0f;
        public float CellXInterval => cellXInterval;

        [Range(0f, 20f)]
        [SerializeField]
        private float cellYInterval = 1.0f;
        public float CellYInterval => cellYInterval;

        [SerializeField] private float xOffset = 1;
        [SerializeField] private float yOffset = 2;


        [SerializeField]
        private GameObject levelObjectPrefab;

        [SerializeField]
        private GameObject line;

        [SerializeField]
        private Transform pathParent;

        [SerializeField]
        private Transform connectionParent;

        public Transform startLocation;

        
        private void Awake()
        {
            this.RegisterEvent<OnNewMapGenerated>(OnNewMapGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            
            GameObjectPoolManager.Singleton.CreatePool(levelObjectPrefab, 200, 500).NumObjDestroyPerFrame = 200;
            

            GameObjectPoolManager.Singleton.CreatePool(line, 100, 400);


        }

        private void Update() {
            CheckClick();
        }

        private void CheckClick() {
            Camera cam = Camera.main;

            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D ray = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));

                Collider2D collider = ray.collider;

                if (collider != null && collider.gameObject.CompareTag("MapItem"))
                {
                    LevelObject level = collider.GetComponent<LevelObject>();
                    
                    this.SendCommand<SelectLevelCommand>(SelectLevelCommand.Allocate(level));
                }

            }
        }

        private void OnNewMapGenerated(OnNewMapGenerated e) {
            Debug.Log("Event received");
            CreatePathRoutine(e.NodeTree);
            //Floor0Stage();
        }

        void CreatePathRoutine(List<List<PathNode>> nodeTree)
        {
            float YPos = startLocation.position.y;
            int pathDepth = this.GetModel<IMapGenerationConfigModel>().PathDepth;
            int pathWidth = this.GetModel<IMapGenerationConfigModel>().PathWidth;
            Random random = this.GetSystem<ISeedSystem>().MapRandom;
            for (int i = pathDepth; i >= 0; i--)
            {
                float XPos = startLocation.position.x;

                for (int j = 0; j < pathWidth; j++)
                {
                    if (nodeTree[i][j] != null)
                    {
                        PathNode node = nodeTree[i][j];

                        Vector3 pos;
                        if (node.NodeType == LevelType.Boss)
                        {//add a bit offset
                            pos = new Vector3(XPos + (float)(random.NextDouble() * 2 - 1) * xOffset,
                                (YPos + 1 + ((float)random.NextDouble() * 2 - 1) * yOffset));
                        }
                        else
                        {
                            //add a bit offset
                            pos = new Vector3(XPos + (float)(random.NextDouble() * 2 - 1) * xOffset,
                                (YPos + ((float)random.NextDouble() * 2 - 1) * yOffset));
                        }


                        node.PositionOnMap = pos;

                        GameObject obj = GameObjectPoolManager.Singleton.Allocate(levelObjectPrefab);

                        obj.transform.position = pos;
                        obj.transform.SetParent(pathParent);
                        

                        node.LevelObject = obj;
                        obj.GetComponent<LevelObject>().LevelType = node.NodeType;
                        node.ConnectedNodes.ForEach(connectedNodes => {
                            connectedNodes.ConnectedByLevelObject.Add(obj);
                        });
                        obj.GetComponent<LevelObject>().Node = node;
                     
                    }

                    XPos += cellXInterval;
                }

                YPos += cellYInterval;
            }



            //draw connection
            for (int i = 0; i < nodeTree.Count; i++)
            {
                for (int j = 0; j < nodeTree[i].Count; j++)
                {
                    if (nodeTree[i][j] != null)
                    {
                        if (nodeTree[i][j].ConnectedNodes.Count > 0)
                        {
                            PathNode node = nodeTree[i][j];


                            PathNode[] ToNodes = node.ConnectedNodes.ToArray();


                            foreach (PathNode toNode in ToNodes)
                            {
                                Vector3 fromPos = Vector3.zero;
                                Vector3 toPos = Vector3.zero;

                                if (toNode.Depth != node.Depth)
                                {
                                    if (toNode.Depth > node.Depth)
                                    {
                                        if (node.NodeType == LevelType.Boss)
                                        {
                                            fromPos = new Vector3(node.PositionOnMap.x,
                                                node.PositionOnMap.y - 1f, -0.1f);
                                        }
                                        else
                                        {
                                            fromPos = new Vector3(node.PositionOnMap.x,
                                                node.PositionOnMap.y - 1.4f, -0.1f);
                                        }

                                        toPos = new Vector3(toNode.PositionOnMap.x,
                                            toNode.PositionOnMap.y + 1.4f, -0.1f);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (toNode.Order > node.Order)
                                    {
                                        fromPos = new Vector3(node.PositionOnMap.x + 1.4f,
                                            node.PositionOnMap.y, -0.1f);

                                        toPos = new Vector3(toNode.PositionOnMap.x - 1.4f,
                                            toNode.PositionOnMap.y, -0.1f);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }


                                LineRenderer line = GameObjectPoolManager.Singleton.Allocate(this.line).GetComponent<LineRenderer>();
                                line.SetPositions(new Vector3[] {
                                fromPos,
                                toPos,
                            });
                                line.transform.SetParent(connectionParent);
                            }

                        }
                    }
                }
            }
          
        }

        /*
        private void Floor0Stage() {
          //  yield return new WaitForSeconds(0.1f);
            List<PathNode> level0Nodes = this.GetSystem<IGameMapSystem>()
                .GetPathNodeAtDepth(this.GetModel<IMapGenerationModel>().PathDepth);
              
            foreach (PathNode level0Node in level0Nodes) {
              
                level0Node.LevelObject.GetComponent<LevelObject>().OnPlayerMeet();
            }
        }*/

        public void OnSingletonInit() {
            
        }
    }

}
