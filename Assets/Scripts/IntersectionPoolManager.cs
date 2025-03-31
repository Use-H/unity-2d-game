using UnityEngine;
using System.Collections.Generic;

public class PathValidation : MonoBehaviour
{
    private HashSet<string> visited = new HashSet<string>();

    void Start()
    {
        RandomizeIntersections();
        RandomizeRoads();
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
                verticalBlock.SetActive(Random.value > 0.5f);

            if (horizontalBlock != null)
                horizontalBlock.SetActive(Random.value > 0.5f);
        }
    }
}