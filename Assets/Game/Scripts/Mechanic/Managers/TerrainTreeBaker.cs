using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainTreeBaker : MonoBehaviour
{
    [Header("Tree Trunk Settings")]
    public float trunkRadius = 0.5f;
    public float trunkHeight = 3f;

    [ContextMenu("Spawn Tree Obstacles")]
    public void SpawnObstacles()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;

        // Clean up old colliders if you click the button multiple times
        GameObject oldParent = GameObject.Find("Tree_Bake_Obstacles");
        if (oldParent != null)
        {
            DestroyImmediate(oldParent);
        }

        GameObject parent = new GameObject("Tree_Bake_Obstacles");
        parent.transform.position = terrain.transform.position;

        int count = 0;
        foreach (TreeInstance tree in data.treeInstances)
        {
            // Calculate the exact world position of each terrain tree
            Vector3 worldPos = Vector3.Scale(tree.position, data.size) + terrain.transform.position;

            // Spawn an invisible GameObject at the tree's base
            GameObject treeObj = new GameObject("Tree_Collider_" + count);
            treeObj.transform.position = worldPos;
            treeObj.transform.SetParent(parent.transform);

            // Add a physical collider  
            CapsuleCollider col = treeObj.AddComponent<CapsuleCollider>();
            col.radius = trunkRadius;
            col.height = trunkHeight;

            count++;
        }

        Debug.Log($"Success! Generated {count} invisible tree colliders. Now go click Bake on NavMeshSurface.");
    }
}