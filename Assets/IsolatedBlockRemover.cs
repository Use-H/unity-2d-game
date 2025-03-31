using UnityEngine;
using System.Collections.Generic;

public class IsolatedBlockRemover : MonoBehaviour
{
    private Dictionary<Vector2Int, GameObject> roadBlocks = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> intersections = new HashSet<Vector2Int>();
    private int rows = 4;  // 내부 교차로 개수 (가로)
    private int cols = 4;  // 내부 교차로 개수 (세로)

    void Start()
    {
        InitializeGrid();
        RemoveIsolatedBlocks();
    }

    void InitializeGrid()
    {
        roadBlocks.Clear();
        intersections.Clear();

        for (int i = 1; i <= 12; i++)
        {
            AddBlock($"Vertical_block{i}");
            AddBlock($"Horizontal_block{i}");
        }
        for (int i = 1; i <= 9; i++)
        {
            AddIntersection($"Intersection_block{i}");
        }
    }

    void AddBlock(string blockName)
    {
        GameObject block = GameObject.Find(blockName);
        if (block != null && block.activeSelf)
        {
            Vector2Int pos = GetBlockPosition(blockName);
            roadBlocks[pos] = block;
        }
    }

    void AddIntersection(string blockName)
    {
        GameObject block = GameObject.Find(blockName);
        if (block != null && block.activeSelf)
        {
            Vector2Int pos = GetBlockPosition(blockName);
            roadBlocks[pos] = block;
            intersections.Add(pos);
        }
    }

    Vector2Int GetBlockPosition(string blockName)
    {
        string[] parts = blockName.Split(new char[] { '_', 'k' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1 && int.TryParse(parts[1], out int index))
        {
            int x = (index - 1) % cols;
            int y = (index - 1) / cols;
            return new Vector2Int(x, y);
        }
        return Vector2Int.zero;
    }

    void RemoveIsolatedBlocks()
    {
        HashSet<Vector2Int> reachable = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 1. 바깥쪽 Pixed_ 블록에서 연결된 블록 찾기
        foreach (var kvp in roadBlocks)
        {
            if (IsConnectedToFixed(kvp.Key))
            {
                queue.Enqueue(kvp.Key);
                reachable.Add(kvp.Key);
            }
        }

        // 2. BFS 실행 (바깥쪽과 연결된 블록 탐색)
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (roadBlocks.ContainsKey(neighbor) && !reachable.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    reachable.Add(neighbor);
                }
            }
        }

        // 3. 교차로(Intersection_block)까지 연결된 블록 찾기
        HashSet<Vector2Int> roadToIntersection = new HashSet<Vector2Int>();
        queue.Clear();
        foreach (var intersection in intersections)
        {
            if (reachable.Contains(intersection))
            {
                queue.Enqueue(intersection);
                roadToIntersection.Add(intersection);
            }
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (reachable.Contains(neighbor) && !roadToIntersection.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    roadToIntersection.Add(neighbor);
                }
            }
        }

        // 4. 고립된 블록 비활성화
        HashSet<Vector2Int> deactivated = new HashSet<Vector2Int>();
        foreach (var kvp in roadBlocks)
        {
            if (!roadToIntersection.Contains(kvp.Key))
            {
                kvp.Value.SetActive(false);
                deactivated.Add(kvp.Key);
            }
        }

        // 5. 다시 활성화할 블록 찾기 (바깥쪽과 연결되었지만, 끝에 Intersection이 없는 길)
        foreach (var kvp in roadBlocks)
        {
            if (deactivated.Contains(kvp.Key) && reachable.Contains(kvp.Key))
            {
                if (!HasConnectedIntersection(kvp.Key))
                {
                    kvp.Value.SetActive(true);  // 다시 활성화
                }
            }
        }
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(pos.x + 1, pos.y),
            new Vector2Int(pos.x - 1, pos.y),
            new Vector2Int(pos.x, pos.y + 1),
            new Vector2Int(pos.x, pos.y - 1)
        };
    }

    bool IsConnectedToFixed(Vector2Int pos)
    {
        return GameObject.Find($"Pixed_Intersection_block") != null ||
               GameObject.Find($"Pixed_Vertical_block") != null ||
               GameObject.Find($"Pixed_Horizontal_block") != null;
    }

    bool HasConnectedIntersection(Vector2Int pos)
    {
        foreach (Vector2Int neighbor in GetNeighbors(pos))
        {
            if (intersections.Contains(neighbor))
            {
                return true;
            }
        }
        return false;
    }
}