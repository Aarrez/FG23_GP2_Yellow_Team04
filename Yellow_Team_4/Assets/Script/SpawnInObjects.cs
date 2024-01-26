using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnInObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToSpawn;

    [SerializeField] private int amount = 1;
    
    [SerializeField] private float maxScaleX = 1f;
    private float minScaleX = -1;
    [SerializeField] private float maxScaleY = 1f;
    private float minScaleY = -1;
    
    [Header("Ray variables")]
    [Tooltip("The Distance from walls the props will spawn")]
    [SerializeField] private float paddingWall = 1f;

    [SerializeField] private LayerMask rayBlockerMask = 1;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(maxScaleX * 2, 1, maxScaleY * 2));
    }
#endif
    
    private void Start()
    {
        minScaleX = -maxScaleX;
        minScaleY = -maxScaleY;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         InstatiateObjects();
    //     }
    // }
    
    public void InstatiateObjects()
    {
        System.Random rand = new System.Random();
        
        float tempMaxY = ChangeScale(Vector3.forward, ref maxScaleY);
        float tempMinY = ChangeScale(Vector3.back, ref minScaleY);
        float tempMaxX = ChangeScale(Vector3.right, ref maxScaleX);
        float tempMinX = ChangeScale(Vector3.left, ref minScaleX);
        
        for (int i = 0; i < amount - 1; i++)
        {
            float randPositionx = Random.Range(tempMinX, tempMaxX);
            float randPositionz = Random.Range(tempMinY, tempMaxY);
           
            Vector3 tempVector = new Vector3(randPositionx, transform.position.y, randPositionz);
            int randInt = rand.Next(0, objectsToSpawn.Length-1);
            GameObject tempObj = objectsToSpawn[randInt].gameObject;
            // Can add a random or specific rotaion
            Instantiate(tempObj, tempVector, transform.rotation, transform);
        }
    }

    private float ChangeScale(Vector3 direction, ref float scale)
    {
        float tempf = scale < 0 ? -scale : scale;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, tempf, rayBlockerMask)) 
        {
            float a = Vector3.Distance(transform.position, hit.transform.position);

            if (scale < 0)
                return -a + paddingWall;
            
            return a - paddingWall;

        }
        return scale;
    }
}
