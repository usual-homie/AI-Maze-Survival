using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public List<GridNode> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        gridManager.ResetNodes();

        GridNode startNode = gridManager.NodeFromWorld(startPosition);
        GridNode targetNode = gridManager.NodeFromWorld(targetPosition);

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedList = new HashSet<GridNode>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost ||
                   (openList[i].FCost == currentNode.FCost &&
                    openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (GridNode neighbour in gridManager.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedList.Contains(neighbour))
                    continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newCost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        return new List<GridNode>();
    }

    private List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();

        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    private int GetDistance(GridNode a, GridNode b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return (dstX + dstY) * 10;
    }
}