using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grids;
using UnityEngine;
using Grid = Grids.Grid;

public class PlayerController : MonoBehaviour
{
    public GameObject gold;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Grid grid = FindObjectOfType<Grid>();
            GridCell start = grid.GetCellForPosition(transform.position);
            GridCell end = grid.GetCellForPosition(gold.transform.position);
            var path = FindPath_BreadthFirst(grid, start, end);
            foreach (var node in path)
            {
                node.spriteRenderer.color = Color.green;
            }

            StartCoroutine(Co_WalkPath(path));
        }
    }

    IEnumerator Co_WalkPath(IEnumerable<GridCell> path)
    {
        foreach (var cell in path)
        {
            while (Vector2.Distance(transform.position, cell.transform.position) > 0.001f)
            {
                Vector3 targetPosition = Vector2.MoveTowards(transform.position, cell.transform.position, Time.deltaTime);
                targetPosition.z = transform.position.z;
                transform.position = targetPosition;
                yield return null;
            }
        }
    }

    // Update is called once per frame
    static IEnumerable<GridCell> FindPath_DepthFirst(Grid grid, GridCell start, GridCell end)
    {
        Stack<GridCell> path = new Stack<GridCell>();
        HashSet<GridCell> visited = new HashSet<GridCell>();
        path.Push(start);
        visited.Add(start);

        while (path.Count > 0)
        {
            bool foundNextNode = false;
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(path.Peek()))
            {
                if (visited.Contains(neighbor)) continue;
                path.Push(neighbor);
                visited.Add(neighbor);
                neighbor.spriteRenderer.color = Color.cyan;
                if (neighbor == end) return path.Reverse(); // <<<<<<<<<<
                foundNextNode = true;
                break;
            }

            if (!foundNextNode)
                path.Pop();
        }

        return null;
    }
    
    static IEnumerable<GridCell> FindPath_BreadthFirst(Grid grid, GridCell start, GridCell end)
    {
        Queue<GridCell> todo = new();                         // STACK -> QUEUE
        HashSet<GridCell> visited = new();
        todo.Enqueue(start);                                  // SAME, BUT DIFFERENT
        visited.Add(start);
        Dictionary<GridCell, GridCell> previous = new();      // NEW, TRACK PREVIOUS NEED

        while (todo.Count > 0)                                 // SAME, BUT DIFFERENT
        {
            //bool foundNextNode = false;
            var current = todo.Dequeue();               // PEEK -> DEQUEUE, SEPARATE VARIABLE
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(current)) // USE VARIABLE
            {
                if (visited.Contains(neighbor)) continue;
                todo.Enqueue(neighbor);                        // SAME, BUT DIFFERENT
                previous[neighbor] = current;                  // NEW: KEEP TRACK OF WHERE WE CAME FROM
                visited.Add(neighbor);
                neighbor.spriteRenderer.color = Color.cyan;
                if (neighbor == end) 
                    return TracePath(neighbor, previous).Reverse(); // NEW: BUILD PATH
                //foundNextNode = true;
                //break;
            }

            //if (!foundNextNode)
            //    path.Pop();
        }

        return null;
    }

    private static IEnumerable<GridCell> TracePath(GridCell neighbor, Dictionary<GridCell, GridCell> previous)
    {
        while (true)
        {
            yield return neighbor;
            if (!previous.TryGetValue(neighbor, out neighbor))
                yield break;
        }
    }
}
