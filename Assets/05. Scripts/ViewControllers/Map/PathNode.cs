using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.BindableProperty;
using MikroFramework.Pool;
using MikroFramework.Serializer;
using Newtonsoft.Json;
using NHibernate.Util;
using UnityEngine;

public enum LevelType {
    Boss,
    Unknown,
    Nothing,
    Treasure,
    Rest,
    Enemy,
    Elite,
    RestOccupiedByEnemy,
}

[Serializable]
[ES3Serializable]
public class NodeConnectionInfo {
    public int Order;
    public int Depth;
}

[Serializable]
[ES3Serializable]
public class PathNode : IPoolable {

    [JsonIgnore][ES3NonSerializable][NonSerialized]
    private List<PathNode> connectedNodes;
   
    [JsonIgnore]
    [ES3NonSerializable]
    public List<PathNode> ConnectedNodes => connectedNodes;

    [JsonIgnore]
    [NonSerialized]
    [ES3NonSerializable]
    public List<PathNode> ConnectedByNodes;

    [ES3Serializable]
    public List<NodeConnectionInfo> ConnectedNodeInfo { get; set; } = new List<NodeConnectionInfo>();

    [ES3Serializable] public bool Visited = false;

    [ES3NonSerializable]
    public bool IsActiveNode {
        get {
            return ConnectedNodeInfo.Count > 0;
            
        }
    }
    
    [ES3Serializable]
    private int depth = 0;
    public int Depth => depth;
    [ES3Serializable]
    private int order = 0;
    public int Order => order;
    [ES3Serializable]
    public BindableProperty<LevelType> NodeType = new BindableProperty<LevelType>();
    

    [ES3Serializable]
    public Vector3 PositionOnMap;

    [JsonIgnore]
    [ES3NonSerializable]
    [NonSerialized]
    public List<GameObject> ConnectedByLevelObject = new List<GameObject>();

    [JsonIgnore]
    [ES3NonSerializable]
    [NonSerialized]
    public GameObject LevelObject;
    public PathNode() {
        
        connectedNodes = new List<PathNode>();
        NodeType.Value = LevelType.Enemy;
        ConnectedByNodes = new List<PathNode>();
    }

    public PathNode(int depth, int order, LevelType nodeType) : this() {
        this.depth = depth;
        this.order = order;
        this.NodeType.Value = nodeType;
    }

    public PathNode(int depth, int order, LevelType levelType, params PathNode[] nextNodes) : this(depth, order, levelType) {
        AddNodes(nextNodes);
    }

    public void AddNode(PathNode node) {
        connectedNodes.Add(node);
        ConnectedByNodes.Add(node);
        node.ConnectedByNodes.Add(this);
        node.connectedNodes.Add(this);
        ConnectedNodeInfo.Add(new NodeConnectionInfo() {
            Depth = node.depth,
            Order = node.order
        });
    }

    

    public void AddNodes(params PathNode[] nodes) {
        connectedNodes.AddRange(nodes);
        ConnectedByNodes.AddRange(nodes);
        foreach (PathNode node in nodes) {
            node.ConnectedByNodes.Add(this);
            node.connectedNodes.Add(this);
            ConnectedNodeInfo.Add(new NodeConnectionInfo() {
                Depth = node.depth,
                Order = node.order
            });
        }
       
    }

    public void OnRecycled() {
        connectedNodes.Clear();
        ConnectedByNodes.Clear();
        NodeType.ClearListeners();
        Visited = false;
        ConnectedByLevelObject.Clear();
    }

    public bool IsRecycled { get; set; }
    public void RecycleToCache() {
       
        SafeObjectPool<PathNode>.Singleton.Recycle(this);
    }

    public static PathNode Allocate(int depth, int order, LevelType nodeType) {
        PathNode node = SafeObjectPool<PathNode>.Singleton.Allocate();
        node.depth = depth;
        node.order = order;
        node.NodeType.Value = nodeType;
        return node;
    }
}
