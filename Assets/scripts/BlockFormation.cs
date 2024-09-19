using UnityEngine;

public class BlockFormation : MonoBehaviour
{
    public GameObject prefab;       // The prefab to be instantiated
    public int width = 5;           // Number of prefabs in the width
    public int height = 5;          // Number of prefabs in the height
    public float spacing = 2.0f;    // Space between prefabs

    void Start()
    {
        CreateBlockFormation();
    }

    void CreateBlockFormation()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position for each prefab
                Vector3 position = new Vector3((x - width / 2) * spacing * 1000, 0, (y - width / 2) * spacing * 1000);
                // Instantiate the prefab at the calculated position
                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}
