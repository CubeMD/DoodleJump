using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public GameObject player;
    public GameObject platformPrefab;
    public GameObject jumpPrefab;
    public GameObject slidePrefab;
    public GameObject enemyPrefab;

    public int numPlatformsToSapwn;

    public float halfLevelWidth;
    public float minPlatformY;
    public float maxPlatformY;

    public float jumpSpawnChance;
    public float slideSpawnChance;

    public float minEnemyY;
    public float maxEnemyY;

    private float maxHeightGenerated = -20;
    private float maxEnemyHeightGenerated = 10;


    private void FixedUpdate()
    {
        if (player.transform.position.y > maxHeightGenerated - 15)
        {
             foreach (Transform child in transform)
             {
                 if (child.position.y < player.transform.position.y - 15)
                 {
                        Destroy(child.gameObject);
                 }
             }

            GenerateSection();
        }
    }

    public void GenerateSection()
    {

        Vector3 spawnPlatformPosition = new Vector3(0, maxHeightGenerated, 0);

        for (int i = 0; i < numPlatformsToSapwn; i++)
        {
            spawnPlatformPosition.y += Random.Range(Mathf.Clamp(minPlatformY + spawnPlatformPosition.y / 500, 0, maxPlatformY - 0.1f), maxPlatformY);
            spawnPlatformPosition.x = Random.Range(-halfLevelWidth, halfLevelWidth);

            float rand = Random.value;

            if (rand > jumpSpawnChance)
            {
                Instantiate(jumpPrefab, transform.parent.position + spawnPlatformPosition, Quaternion.identity, transform);
            }
            else if (rand > slideSpawnChance)
            {
                Instantiate(slidePrefab, transform.parent.position + spawnPlatformPosition, Quaternion.identity, transform);
            }
            else
            {
                Instantiate(platformPrefab, transform.parent.position + spawnPlatformPosition, Quaternion.identity, transform);
            }
        }

        maxHeightGenerated = spawnPlatformPosition.y;

        Vector3 spawnEnemyPosition = new Vector3(0, maxEnemyHeightGenerated, 0);

        for (int i = 0; i < 3; i++)
        {
            spawnEnemyPosition.y += Random.Range(minEnemyY, maxEnemyY);
            spawnEnemyPosition.x = Random.Range(-halfLevelWidth, halfLevelWidth);
            Instantiate(enemyPrefab, transform.parent.position + spawnEnemyPosition, Quaternion.identity, transform);
        }

        maxEnemyHeightGenerated = spawnEnemyPosition.y;
    }

    public void ResetLevel()
    {
        foreach (Transform child in transform)
        {

            Destroy(child.gameObject);

        }

        maxHeightGenerated = -20;
        maxEnemyHeightGenerated = 10;

        GenerateSection();
    }
}
