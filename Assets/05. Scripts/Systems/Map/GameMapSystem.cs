using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MikroFramework.Architecture;
using MikroFramework.Serializer;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    public interface IGameMapSystem : ISystem {
        void InitializeTree();
        List<List<PathNode>> GeneratePath();

        List<PathNode> GetPathNodeAtDepth(int depth);

        PathNode GetPathNodeAtDepthAndOrder(int depth, int order);

        List<PathNode> GetAllAvailableNodes();
    }

    public class GameMapSystem : AbstractSystem, IGameMapSystem, ICanRegisterAndLoadSavedData, ICanSaveGame {
        private int treasureLevel;

        private List<List<PathNode>> nodeTree;
        private IMapGenerationModel mapGenerationModel;

        public bool IsNodeTreeGenerated {
            get {
                return nodeTree.Count > 0;
            }
        }

        protected override void OnInit() {
            Debug.Log("Game map system init");
            mapGenerationModel = this.GetModel<IMapGenerationModel>();
            nodeTree = this.RegisterAndLoadFromSavedData("map_path", new List<List<PathNode>>());
            if (!IsNodeTreeGenerated) {
                GeneratePath();
            }
            else {
                LoadSavedMap();
                Debug.Log("Load saved map");

            }
            
        }
       

        public void InitializeTree() {
            nodeTree.ForEach(level => {
                if (level != null) {
                    level.ForEach(node => {
                        if (node != null) {
                            node.RecycleToCache();
                        }
                    });
                }
                
            });
            nodeTree.Clear();
            for (int i = 0; i <= mapGenerationModel.PathDepth; i++)
            {
                List<PathNode> currentPathHeight = new List<PathNode>(mapGenerationModel.PathWidth);

                for (int j = 0; j < mapGenerationModel.PathWidth; j++)
                {
                    currentPathHeight.Add(null);
                }

                nodeTree.Add(currentPathHeight);
            }

            int middleHeight = mapGenerationModel.PathDepth / 2;
            treasureLevel = this.GetSystem<ISeedSystem>().MapRandom.Next(middleHeight - 1, middleHeight + 2);
        }

        public List<List<PathNode>> GeneratePath() {
            InitializeTree();
            BuildBossLevel();

            int pathWidth = mapGenerationModel.PathWidth;
            int pathDepth = mapGenerationModel.PathDepth;

            List<PathNode> nodesAtCurrentDepth = new List<PathNode>() {
            nodeTree[0][pathWidth / 2]

            };


            //all nodes (exclude the last line)
            for (int depth = 0; depth < pathDepth; depth++)
            {
                List<PathNode> tempNextLevelNodesRecord = new List<PathNode>();
                int mimimumIndex = 0;
                List<int> connectedIndex = new List<int>();
                //Debug.Log($"Depth: {depth}");

                //all nodes in current depth
                foreach (PathNode currentLevelNode in nodesAtCurrentDepth)
                {
                    if (mimimumIndex > currentLevelNode.Order && nodesAtCurrentDepth.Count >= 2)
                    {
                        continue;
                    }
                    int currentLevelOrder = currentLevelNode.Order; //0-6


                    int connectedNodeNumber;
                    do
                    {
                        connectedNodeNumber = GetConnectedNodeNumber(depth);
                    } while (connectedNodeNumber == 0 && nodesAtCurrentDepth.Count <= 2);



                    //Debug.Log($"Current Node Order: {currentLevelNode.Order}, " +
                       //       $"Connecting {connectedNodeNumber} nodes");

                    //connected order range: order-2 ~ order+2
                    //also make sure lines don't intersect
                    int connectedLevelMinimumBound = Math.Max(mimimumIndex, currentLevelOrder - 3);
                    int connectedLevelMaxmimumBound = Math.Min(pathWidth - 1, currentLevelOrder + 3);

                    connectedNodeNumber = Math.Min(connectedLevelMaxmimumBound - connectedLevelMinimumBound + 1,
                        connectedNodeNumber);

                   // Debug.Log($"Min: {connectedLevelMinimumBound}, Max: {connectedLevelMaxmimumBound}");
                    //connected nodes
                    List<PathNode> connectedNodes = new List<PathNode>(connectedNodeNumber);


                    for (int i = 0; i < connectedNodeNumber; i++)
                    {

                        bool duplicate = false;
                        int connectedNodeIndex;

                        do
                        {
                            duplicate = false;
                            //have more possibility to connect to what others already connected
                            connectedNodeIndex = GenerateConnectedNodeIndex(connectedLevelMinimumBound,
                                connectedLevelMaxmimumBound,
                                tempNextLevelNodesRecord);

                            foreach (PathNode node in connectedNodes)
                            {
                                if (node.Order == connectedNodeIndex)
                                {
                                    duplicate = true;
                                    break;
                                }
                            }
                        } while (duplicate);
                       // Debug.Log($"Connecting: {connectedNodeIndex}");
                        connectedIndex.Add(connectedNodeIndex);
                        PathNode connectedNode = CheckNodeExist(connectedNodeIndex, tempNextLevelNodesRecord);

                        if (connectedNode == null)
                        {

                            connectedNode = CreateNewNode(depth + 1, connectedNodeIndex);

                            tempNextLevelNodesRecord.Add(connectedNode);

                            nodeTree[depth + 1][connectedNodeIndex] = connectedNode;
                        }


                        //two elite & merchant & rest can't be connected

                        currentLevelNode.AddNode(connectedNode);
                        connectedNodes.Add(connectedNode);
                    }

                    mimimumIndex = connectedIndex.FindBiggest();
                    SortNodesList(tempNextLevelNodesRecord);
                }

                nodesAtCurrentDepth.Clear();
                nodesAtCurrentDepth.AddRange(tempNextLevelNodesRecord);
                //sort nodesAtCurrentDepth
                SortNodesList(nodesAtCurrentDepth);
                //connect neighbors

                for (int i = 0; i < nodesAtCurrentDepth.Count; i++)
                {
                    bool connectLeft = false, connectRight = false;

                    if (i - 1 >= 0)
                    {
                        if (Mathf.Abs(nodesAtCurrentDepth[i].Order - nodesAtCurrentDepth[i - 1].Order) <= 3)
                        {
                            connectLeft = this.GetSystem<ISeedSystem>().MapRandom.Next(0, 3) < 2;
                        }

                    }

                    if (i + 1 < nodesAtCurrentDepth.Count)
                    {
                        if (Mathf.Abs(nodesAtCurrentDepth[i].Order - nodesAtCurrentDepth[i + 1].Order) <= 3)
                        {
                            connectRight = this.GetSystem<ISeedSystem>().MapRandom.Next(0, 3) < 2;
                        }


                    }

                    if (connectLeft)
                    {
                        nodesAtCurrentDepth[i].AddNode(nodesAtCurrentDepth[i - 1]);
                    }

                    if (connectRight)
                    {
                        nodesAtCurrentDepth[i].AddNode(nodesAtCurrentDepth[i + 1]);
                    }
                }
            }

             //ES3.Save("map_path",nodeTree);
             
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
                this.SendEvent<OnNewMapGenerated>(new OnNewMapGenerated() {NodeTree = nodeTree});
                Debug.Log("Sent event");
                this.SaveGame();
            });


            Debug.Log("Tree Built");

            return nodeTree;
            // StartCoroutine(CreatePathRoutine());
        }


        private void LoadSavedMap() {
            foreach (List<PathNode> curDepthNodes in nodeTree) {
                foreach (PathNode node in curDepthNodes) {
                    if (node != null) {
                        foreach (NodeConnectionInfo nodeConnectionInfo in node.ConnectedNodeInfo) {
                            PathNode other = nodeTree[nodeConnectionInfo.Depth][nodeConnectionInfo.Order];
                            node.ConnectedByNodes.Add(other);
                            node.ConnectedNodes.Add(other);
                            other.ConnectedByNodes.Add(node);
                            other.ConnectedNodes.Add(node);
                        }
                    }
                }
            }

            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
                this.SendEvent<OnNewMapGenerated>(new OnNewMapGenerated() { NodeTree = nodeTree });
            });
        }

        public List<PathNode> GetPathNodeAtDepth(int depth) {
            List<PathNode> allNodes = nodeTree[depth];

            List<PathNode> retResult = new List<PathNode>();
            foreach (PathNode pathNode in allNodes) {
                if (pathNode != null) {
                    retResult.Add(pathNode);
                }
            }

            return retResult;
        }

        public PathNode GetPathNodeAtDepthAndOrder(int depth, int order) {
            Debug.Log("GetPathNodeAtDepthAndOrder");
            return nodeTree[depth][order];
        }

        public List<PathNode> GetAllAvailableNodes() {
            List<PathNode> allNodes = new List<PathNode>();
            foreach (List<PathNode> nodes in nodeTree) {
                foreach (PathNode node in nodes) {
                    if (node != null) {
                        allNodes.Add(node);
                    }
                }
            }

            return allNodes;
        }

        private void SortNodesList(List<PathNode> nodes) {
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {

                if (nodes[i].Order > nodes[j].Order)
                {
                    PathNode temp = nodes[i];
                    nodes[i] = nodes[j];
                    nodes[j] = temp;
                }
            }
        }
    }
        private int GetConnectedNodeNumber(int depth) {
            int pathDepth = mapGenerationModel.PathDepth;
            Random random = this.GetSystem<ISeedSystem>().MapRandom;

            int distanceFromStart = (pathDepth / 2) - Mathf.Abs(depth - (pathDepth / 2));

            float frac = (float)distanceFromStart / (pathDepth / 2);
            //frac: 0 - 1
            int chance = random.Next(0, 100);

            if (depth >= 1 && depth != pathDepth - 2)
            {

                if (frac >= 0 && frac < 0.3)
                {

                    if (chance >= 0 && chance < 60)
                    {
                        return 1;
                    }
                    else if (chance >= 60 && chance < 85)
                    {
                        return 2;
                    }
                    else if (chance >= 85 && chance < 93)
                    {
                        return 0;
                    }

                    return 2;


                }
                else if (frac >= 0.3 && frac < 0.6)
                {
                    if (chance >= 0 && chance < 55)
                    {
                        return 1;
                    }
                    else if (chance >= 55 && chance < 85)
                    {
                        return 2;
                    }
                    else if (chance >= 85 && chance < 90)
                    {
                        return 0;
                    }

                    return 3;
                }
                else if (frac >= 0.6 && frac < 0.9)
                {

                    if (chance >= 0 && chance < 50)
                    {
                        return 1;
                    }
                    else if (chance >= 50 && chance < 85)
                    {
                        return 2;
                    }
                    else if (chance >= 85 && chance < 88)
                    {
                        return 0;
                    }

                    return 3;
                }

                else
                {
                    if (chance >= 0 && chance < 40)
                    {
                        return 1;
                    }
                    else if (chance >= 50 && chance <= 80)
                    {
                        return 2;
                    }

                    return 3;
                }

            }
            else
            {
                if (depth < 1)
                {
                    return random.Next(4, 7);
                }
                else
                {
                    return 1;
                }

            }

        }

        private int GenerateConnectedNodeIndex(int minimumBound, int maxBound, List<PathNode> existingNodes) {
            Random random = this.GetSystem<ISeedSystem>().MapRandom;
            if (existingNodes.Count > 0 && existingNodes[existingNodes.Count - 1].Order >= minimumBound)
            {
                int ran = random.Next(0, 100);
                if (ran <= 95)
                {
                    return existingNodes[existingNodes.Count - 1].Order;
                }
            }
            return random.Next(minimumBound, maxBound + 1);

        }



        private PathNode CheckNodeExist(int order, List<PathNode> existingNodes)
        {
            foreach (PathNode existingNode in existingNodes)
            {
                if (existingNode.Order == order)
                {
                    return existingNode;
                }
            }

            return null;
        }

        private PathNode CreateNewNode(int depth, int order) {
            Random random = this.GetSystem<ISeedSystem>().MapRandom;
            LevelType type = LevelType.Nothing;

            if (depth == 1)
            {
                type = LevelType.Rest;
            }
            else if (depth == treasureLevel)
            {
                type = LevelType.Treasure;
            }
            else if (depth == mapGenerationModel.PathDepth)
            {
                type = LevelType.Enemy;
            }
            else {
                float lowerBound = 0;

                List<float> possibilities = mapGenerationModel.NormalLevelPossibilityValues;
                int levelChance = random.Next(0, 100);

                for (int i = 0; i < possibilities.Count; i++) {

                    if (levelChance >= lowerBound && levelChance < lowerBound + possibilities[i]) {
                        type = mapGenerationModel.NormalLevelTypes[i];
                        break;
                    }

                    lowerBound += possibilities[i];

                }

            }



            return PathNode.Allocate(depth, order, type);
        }

        void BuildBossLevel() {

            nodeTree[0][mapGenerationModel.PathWidth / 2] = PathNode.Allocate(0, mapGenerationModel.PathWidth / 2,
                LevelType.Boss);
        }

       
    }
}
