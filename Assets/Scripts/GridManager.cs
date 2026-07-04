using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap wallsTilemap;

    private GridNode[,] gridNodes;
    private BoundsInt bounds;

    public int Width => bounds.size.x;
    public int Height => bounds.size.y;

    // Expose the grid as read-only
    public GridNode[,] GridNodes => gridNodes;

    private void Awake()
    {
        Debug.Log("Grid Generated");
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        bounds = wallsTilemap.cellBounds;

        gridNodes = new GridNode[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                bool hasWall = wallsTilemap.HasTile(cell);

                Vector3 world = grid.GetCellCenterWorld(cell);

                gridNodes[x - bounds.xMin, y - bounds.yMin] =
                    new GridNode(
                        !hasWall,
                        world,
                        x - bounds.xMin,
                        y - bounds.yMin
                    );
            }
        }
    }

    public void ResetNodes()
    {
        foreach (GridNode node in gridNodes)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }
    }

    public GridNode NodeFromWorld(Vector3 worldPosition)
    {
        Vector3Int cell = grid.WorldToCell(worldPosition);

        int x = Mathf.Clamp(cell.x - bounds.xMin, 0, bounds.size.x - 1);
        int y = Mathf.Clamp(cell.y - bounds.yMin, 0, bounds.size.y - 1);

        return gridNodes[x, y];
    }

    public List<GridNode> GetNeighbours(GridNode node)
    {
        List<GridNode> neighbours = new List<GridNode>();

        int[,] directions =
        {
            {0,1},
            {1,0},
            {0,-1},
            {-1,0}
        };

        for (int i = 0; i < 4; i++)
        {
            int checkX = node.gridX + directions[i, 0];
            int checkY = node.gridY + directions[i, 1];

            if (checkX >= 0 &&
                checkX < Width &&
                checkY >= 0 &&
                checkY < Height)
            {
                neighbours.Add(gridNodes[checkX, checkY]);
            }
        }

        return neighbours;
    }

    private void OnDrawGizmos()
    {
        if (gridNodes == null)
            return;

        foreach (GridNode node in gridNodes)
        {
            Gizmos.color = node.walkable ? Color.green : Color.red;
            Gizmos.DrawWireCube(node.worldPosition, Vector3.one * 0.4f);
        }
    }
}