using UnityEngine; 
using Unity.Netcode;
public class iceCubeSpawner : NetworkBehaviour{
    [SerializeField] private GameObject iceCubePrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float minX = -6f, maxX = 6f;
    [SerializeField] private float minY = -3.5f, maxY = 3.5f;

    private float timer;

    private void Update()
    {
        if (!IsServer || !GameManager.instance.partieEnCours) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnIceCube();
        }
    }

    void SpawnIceCube()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f        );

        GameObject iceCube = Instantiate(iceCubePrefab, spawnPosition, Quaternion.identity);
        iceCube.GetComponent<NetworkObject>().Spawn();
    }
}