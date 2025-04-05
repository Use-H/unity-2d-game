using UnityEngine;
using System.Collections.Generic;

public class RandomPathManager : MonoBehaviour
{
    private HashSet<string> visited = new HashSet<string>();
    private Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

    void Start()
    {
        InitializeGraph();
        RandomizeIntersections();
        RandomizeRoads();
        EnsureConnectivity(); // 고립된 블록 없도록 연결 확인
        ActivateAdjacentIntersections(); // 추가된 부분: 활성화된 도로의 양옆 교차로 활성화
    }

    void InitializeGraph()
    {
        for (int i = 1; i <= 9; i++)
        {
            graph[$"Intersection_block{i}"] = new List<string>();
        }

        for (int i = 1; i <= 12; i++)
        {
            graph[$"Vertical_block{i}"] = new List<string>();
            graph[$"Horizontal_block{i}"] = new List<string>();
        }
    }

    void RandomizeIntersections()
    {
        for (int i = 1; i <= 9; i++)
        {
            GameObject block = GameObject.Find($"Intersection_block{i}");
            if (block != null)
            {
                block.SetActive(Random.value > 0.5f);
            }
        }
    }

    void RandomizeRoads()
    {
        for (int i = 1; i <= 12; i++)
        {
            GameObject verticalBlock = GameObject.Find($"Vertical_block{i}");
            GameObject horizontalBlock = GameObject.Find($"Horizontal_block{i}");

            if (verticalBlock != null)
            {
                bool activate = Random.value > 0.5f;
                verticalBlock.SetActive(activate);
            }

            if (horizontalBlock != null)
            {
                bool activate = Random.value > 0.5f;
                horizontalBlock.SetActive(activate);
            }
        }
    }

    void EnsureConnectivity()
    {
        HashSet<string> reachable = new HashSet<string>();
        Queue<string> queue = new Queue<string>();

        foreach (var node in graph.Keys)
        {
            GameObject obj = GameObject.Find(node);
            if (obj != null && obj.activeSelf)
            {
                queue.Enqueue(node);
                break;
            }
        }

        while (queue.Count > 0)
        {
            string current = queue.Dequeue();
            if (reachable.Contains(current)) continue;

            reachable.Add(current);

            foreach (string neighbor in graph[current])
            {
                GameObject obj = GameObject.Find(neighbor);
                if (obj != null && obj.activeSelf && !reachable.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        foreach (var node in graph.Keys)
        {
            GameObject obj = GameObject.Find(node);
            if (obj != null && !reachable.Contains(node))
            {
                obj.SetActive(true);
            }
        }
    }

    void ActivateAdjacentIntersections()
    {
        GameObject[] intersections = GameObject.FindGameObjectsWithTag("Intersection");
        GameObject[] verticalBlocks = GameObject.FindGameObjectsWithTag("Vertical");
        GameObject[] horizontalBlocks = GameObject.FindGameObjectsWithTag("Horizontal");

        float threshold = 25f; // x 또는 y 좌표가 ±25 차이

        foreach (GameObject road in verticalBlocks)
        {
            if (road.activeSelf)
            {
                ActivateNearbyIntersections(road.transform.position, intersections, threshold, "Vertical");
            }
        }

        foreach (GameObject road in horizontalBlocks)
        {
            if (road.activeSelf)
            {
                ActivateNearbyIntersections(road.transform.position, intersections, threshold, "Horizontal");
            }
        }
    }

    void ActivateNearbyIntersections(Vector3 roadPosition, GameObject[] intersections, float threshold, string type)
    {
        foreach (GameObject intersection in intersections)
        {
            Vector3 intersectionPos = intersection.transform.position;
            
            // Horizontal이면 x좌표 확인, Vertical이면 y좌표 확인
            if ((type == "Horizontal" && Mathf.Abs(roadPosition.x - intersectionPos.x) <= threshold) ||
                (type == "Vertical" && Mathf.Abs(roadPosition.y - intersectionPos.y) <= threshold))
            {
                intersection.SetActive(true);
            }
        }
    }
}