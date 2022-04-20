using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    [Serializable]
    [ES3Serializable]
    public class MapNode : IEquatable<MapNode> {
        [field: SerializeField]
        [ES3Serializable]
        public BindableProperty<LevelType> LevelType { get; set; } = new BindableProperty<LevelType>();

        [field:SerializeField]
        [ES3Serializable]
        public Vector2Int PointOnMap { get; set; }

        [ES3NonSerializable] public GameObject LevelObject;

        [ES3Serializable] public bool Visited = false;

        public int Depth {
            get {
                return PointOnMap.y;
            }
        }

        public int Order
        {
            get
            {
                return PointOnMap.x;
            }
        }

        public MapNode(){}

        public MapNode(LevelType levelType, Vector2Int pointOnMap) {
            this.LevelType.Value = levelType;
            this.PointOnMap = pointOnMap;
        }

        public MapNode(LevelType levelType, int x,  int y) {
            this.LevelType.Value = levelType;
            this.PointOnMap = new Vector2Int(x, y);
        }

        public static float GetDistance(MapNode node1, MapNode node2) {
            return (node1.PointOnMap - node2.PointOnMap).magnitude;
        }
        public static float GetManhattanCost(MapNode a, MapNode b)
        {
            return Mathf.Abs(a.PointOnMap.x - b.PointOnMap.x) +
                   Mathf.Abs(a.PointOnMap.y - b.PointOnMap.y);
        }
        public static float GetEuclideanCost(MapNode from, MapNode to)
        {
            return GetDistance(from, to);
        }

        public static float GetAngleInRadians(MapNode from, MapNode to) {
            Vector2 temp = new Vector2(to.PointOnMap.x - from.PointOnMap.x, to.PointOnMap.y - from.PointOnMap.y);
            float radians = Mathf.Atan2(temp.y, temp.x);
            return radians;
        }

        public bool Equals(MapNode other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PointOnMap.Equals(other.PointOnMap);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MapNode) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((LevelType != null ? LevelType.GetHashCode() : 0) * 397) ^ PointOnMap.GetHashCode();
            }
        }
    }
}
