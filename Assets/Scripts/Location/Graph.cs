using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    public List<Node> nodes = new List<Node>();
    private int count = 1;
    private GraphData graphData;
    private Dictionary<int, int> layerWidths = new Dictionary<int, int>();
    private int[,] graphMatrix;

    public Graph(GraphData graphData)
    {
        this.graphData = graphData;
        graphMatrix = new int[graphData.layers, graphData.maxWidth];
        AddNode(new Node(this, 1, graphData.startLayer));
        for(int i = 0; i < 100; ++i)
        {
            if (count >= graphData.nodes) break;
            EnterNode(nodes[0]);
        }

        ValidateGraph();

        //foreach (Node node in nodes) Debug.Log(node);
        //for (int i = 0; i < graphData.layers; ++i)
        //{
        //    string r = "";
        //    for (int j = 0; j < graphData.maxWidth; ++j) r += graphMatrix[i, j] + " ";
        //    Debug.Log(r);
        //}
    }

    public void ValidateGraph() {
        Node bossNode = nodes[0];
        foreach(Node node in nodes)
        {
            for (int i = 0; i < graphData.nodeTypes.Count; ++i) if (UnityEngine.Random.Range(0f, 1f) < graphData.nodeTypeChances[i]) node.type = graphData.nodeTypes[i];
            if(bossNode.depth < node.depth) bossNode = node;
        }
        bossNode.type = NodeType.BOSS;
    }

    public Node GetNodeById(int id)
    {
        foreach (Node node in nodes) if (node.id == id) return node;
        return null;
    }

    public void EnterNode(Node node, int depth=0)
    {
        List<Node> newNodes = new List<Node>();
        try
        {
            if(node.neighbours.right == null)
            if (nodes.Count < graphData.nodes && UnityEngine.Random.Range(0f, 1f) < graphData.neighborRightChance && graphMatrix[node.layer, node.gridX + 1] == 0)
            {
                Node newNode = new Node(node.parent, nodes.Count + newNodes.Count, node.layer, node.gridX + 1);
                newNode.neighbours.left = node;

                try
                {
                    newNode.id = count;
                    AddNode(newNode);
                    newNodes.Add(newNode);
                }
                catch (System.DataMisalignedException) { }

            }
            else if(graphMatrix[node.layer, node.gridX + 1] != 0 && UnityEngine.Random.Range(0f, 1f) < graphData.breakWallChance)
            {
                Node neighbour = GetNodeById(graphMatrix[node.layer, node.gridX + 1]);
                node.neighbours.right = neighbour;
                neighbour.neighbours.left = node;
            }
        }
        catch (IndexOutOfRangeException) { }

        try
        {
            if (node.neighbours.left == null)
            if (nodes.Count < graphData.nodes && UnityEngine.Random.Range(0f, 1f) < graphData.neighborLeftChance && graphMatrix[node.layer, node.gridX - 1] == 0)
            {
                Node newNode = new Node(node.parent, nodes.Count + newNodes.Count, node.layer, node.gridX - 1);
                newNode.neighbours.right = node;
                try
                {
                    newNode.id = count;
                    AddNode(newNode);
                    newNodes.Add(newNode);
                }
                catch (System.DataMisalignedException) { }
            }
            else if (graphMatrix[node.layer, node.gridX - 1] != 0 && UnityEngine.Random.Range(0f, 1f) < graphData.breakWallChance)
            {
                Node neighbour = GetNodeById(graphMatrix[node.layer, node.gridX - 1]);
                node.neighbours.left = neighbour;
                neighbour.neighbours.right = node;
            }
        }
        catch (IndexOutOfRangeException) { }

        try
        {
            if (node.neighbours.up == null)
                if (nodes.Count < graphData.nodes && UnityEngine.Random.Range(0f, 1f) < graphData.neighborUpChance && graphMatrix[node.layer + 1, node.gridX] == 0)
            {
                Node newNode = new Node(node.parent, nodes.Count + newNodes.Count, node.layer + 1, node.gridX);
                newNode.neighbours.down = node;
                try
                {
                    newNode.id = count;
                    AddNode(newNode);
                    newNodes.Add(newNode);
                }
                catch (System.DataMisalignedException) { }
            }
            else if (graphMatrix[node.layer + 1, node.gridX] != 0 && UnityEngine.Random.Range(0f, 1f) < graphData.breakWallChance)
            {
                Node neighbour = GetNodeById(graphMatrix[node.layer + 1, node.gridX]);
                node.neighbours.up = neighbour;
                neighbour.neighbours.down = node;
            }
        }
        catch (IndexOutOfRangeException) { }

        try
        {
            if (node.neighbours.down == null)
                if (nodes.Count < graphData.nodes && UnityEngine.Random.Range(0f, 1f) < graphData.neighborDownChance && graphMatrix[node.layer - 1, node.gridX] == 0)
            {
                Node newNode = new Node(node.parent, nodes.Count + newNodes.Count, node.layer - 1, node.gridX);
                newNode.neighbours.up = node;
                try
                {
                    newNode.id = count;
                    AddNode(newNode);
                    newNodes.Add(newNode);
                }
                catch (System.DataMisalignedException) { }
            }
            else if (graphMatrix[node.layer - 1, node.gridX] != 0 && UnityEngine.Random.Range(0f, 1f) < graphData.breakWallChance)
            {
                Node neighbour = GetNodeById(graphMatrix[node.layer - 1, node.gridX]);
                node.neighbours.down = neighbour;
                neighbour.neighbours.up = node;
            }
        }
        catch (IndexOutOfRangeException) { }


        foreach (Node newNode in newNodes)
        {
            newNode.depth = depth;
            EnterNode(newNode, depth + 1);
        }
    }


    public void AddNode(Node node)
    {

        try
        {
            if (layerWidths[node.layer] <= graphData.maxWidth) layerWidths[node.layer]++;
            else throw new System.DataMisalignedException();
        }
        catch (KeyNotFoundException)
        {
            layerWidths.Add(node.layer, 1);
        }
        nodes.Add(node);
        count++;
        graphMatrix[node.layer, node.gridX] = node.id;
        if (node.neighbours.left != null) node.neighbours.left.neighbours.right = node;
        if (node.neighbours.right != null) node.neighbours.right.neighbours.left = node;
        if (node.neighbours.up != null) node.neighbours.up.neighbours.down = node;
        if (node.neighbours.down != null) node.neighbours.down.neighbours.up = node;
        
    }
}

public class Node
{
    public Graph parent;
    public int id = 0;
    public NeighbourNodes neighbours;
    public int layer = 0;
    public int gridX = 0;
    public NodeType type = NodeType.NORMAL;
    public int depth = 0;

    public Node(Graph parent, int id, int layer, int gridX = 0)
    {
        this.parent = parent;
        this.id = id;
        this.layer = layer;
        this.gridX = gridX;
    }

    public override string ToString()
    {
        int left = neighbours.left != null ? neighbours.left.id : -1;
        int right = neighbours.right != null ? neighbours.right.id : -1;
        int up = neighbours.up != null ? neighbours.up.id : -1;
        int down = neighbours.down != null ? neighbours.down.id : -1;

        return String.Format("<Node {0}: [{1}, {2}, {3}, {4}]>", id, left, right, up, down);
    }
}

public struct NeighbourNodes
{
    public Node left;
    public Node right;
    public Node up;
    public Node down;
}

[System.Serializable]
public class GraphData
{
    public int layers = 1;
    public int minlayers = 1;
    public int startLayer = 0;
    public int nodes = 20;
    public int maxWidth = 20;
    public float neighborUpChance = 0f;
    public float neighborDownChance = 0.5f;
    public float neighborLeftChance = 0f;
    public float neighborRightChance = 0.95f;
    public float breakWallChance = 0.5f;
    public List<NodeType> nodeTypes = new List<NodeType>() { NodeType.TREASURE, NodeType.CHALLENGE };
    public List<float> nodeTypeChances = new List<float>(2);
}

public enum NodeType
{
    NORMAL, TREASURE, CHALLENGE, BOSS
}