using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Spawner;

public class PropRandom : MonoBehaviour
{

    public List<GameObject> PropPrefabs;
    public Tilemap[] groundTilemaps;
    public float nextSpawnTime = 10; //Khoang cach thoi gian spawn prop

    public float spawnRangeX = 20f; // Khoang cach spawn Prop tren truc X
    public float spawnRangeY = 20f; // Khoang cach spawn Prop tren truc Y

    public float minDistanceBetweenProps = 5f; // Khoang cach toi thieu giua cac prop
    public float minDistanceToPlayer = 10f; // Khoang cach toi thieu voi player
    private int totalProps = 0; // Tong so prop da tao
    private const int maxProps = 7; // So Prop toi da
    private float timer; // Timer
    void Start()
    {
        foreach (Tilemap tilemap in groundTilemaps)
        {
            RePosition repositionScript = tilemap.GetComponent<RePosition>();
            if (repositionScript != null)
            {
                repositionScript.OnTilemapMove.AddListener(OnTilemapMoveHandler);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!GameManager.instance.isLive)
            return;
        if (timer > nextSpawnTime && totalProps < maxProps)
        {
            // Calculate the position for the prop
            Vector3 spawnPosition = FindValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                // Randomly select a prop prefab from the array
                GameObject propPrefab = PropPrefabs[Random.Range(0, PropPrefabs.Count)];

                // Spawn the prop
                GameObject prop = Instantiate(propPrefab, spawnPosition, Quaternion.identity);
            }

            // Calculate the next spawn time
            timer= 0;
        }
        totalProps = GameObject.FindGameObjectsWithTag("Prop").Length;
        foreach (GameObject prop in GameObject.FindGameObjectsWithTag("Prop"))
        {
            bool insideBounds = false;
            foreach (Tilemap tilemap in groundTilemaps)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(prop.transform.position);
                if (tilemap.cellBounds.Contains(cellPosition))
                {
                    insideBounds = true;
                    break;
                }
            }
            if (insideBounds)
            {
                prop.SetActive(true);
            }
            else
            {
                prop.SetActive(false);
            }
        }
    }


    Vector3 FindValidSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector3 playerPosition = GameManager.instance.player.transform.position;

        // Attempt to find a valid spawn position multiple times to prevent infinite loops
        for (int i = 0; i < 10; i++)
        {
            // Calculate a random spawn position
            spawnPosition = new Vector3(transform.position.x + Random.Range(-spawnRangeX, spawnRangeX),
                                        transform.position.y + Random.Range(-spawnRangeY, spawnRangeY),
                                        0f);

            // Check if the position is within the bounds of any tilemap and not too close to existing props
            bool insideBounds = false;
            bool tooCloseToExistingProps = false;
            float distanceToPlayer = Vector3.Distance(spawnPosition, playerPosition);

            foreach (Tilemap tilemap in groundTilemaps)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(spawnPosition);
                if (tilemap.cellBounds.Contains(cellPosition))
                {
                    insideBounds = true;

                    // Check distance to existing props
                    GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
                    foreach (GameObject prop in props)
                    {
                        if (Vector3.Distance(prop.transform.position, spawnPosition) < minDistanceBetweenProps)
                        {
                            tooCloseToExistingProps = true;
                            break;
                        }
                    }

                    if (!tooCloseToExistingProps && distanceToPlayer > minDistanceToPlayer)
                    {
                        // Found a valid spawn position
                        return spawnPosition;
                    }
                    else
                    {
                        // Reset spawnPosition for the next attempt
                        spawnPosition = Vector3.zero;
                        break;
                    }
                }
            }

            if (!insideBounds || tooCloseToExistingProps || distanceToPlayer <= minDistanceToPlayer)
            {
                // Retry finding a valid spawn position
                continue;
            }
        }

        return Vector3.zero; // Failed to find a valid spawn position
    }
    public void UpdateGroundTilemaps(Tilemap[] newTilemaps)
    {
        groundTilemaps = newTilemaps;
    }

    void OnTilemapMoveHandler()
    {
        foreach (GameObject prop in GameObject.FindGameObjectsWithTag("Prop"))
        {
            bool insideBounds = false;
            foreach (Tilemap tilemap in groundTilemaps)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(prop.transform.position);
                if (tilemap.cellBounds.Contains(cellPosition))
                {
                    insideBounds = true;
                    break;
                }
            }
            if (insideBounds)
            {
                prop.SetActive(true);
            }
            else
            {
                prop.SetActive(false);
            }
        }
    }
}