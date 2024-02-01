using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


enum BlockCheck
{
    None,
    LayerMask,
    Tags
    
}
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

    [Header("Blocking possibilites")]
    [SerializeField] [Tooltip("Select if you want to check for blockers with Tags, LayerMasks or not at all")]
    private BlockCheck blockCheck;
    [SerializeField] private LayerMask rayBlockerMask = 1;
    [SerializeField] private string[] rayBlockerTags;

    public static UnityAction SpawnInAtCurrentPosition;
    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(maxScaleX * 2, 1, maxScaleY * 2));
    }

    private void OnValidate()
    {
        minScaleX = -maxScaleX;
        minScaleY = -maxScaleY;
    }
#endif

    private void OnEnable()
    {
        SpawnInAtCurrentPosition += InstatiationMethod;
    }

    private void OnDisable()
    {
        SpawnInAtCurrentPosition -= InstatiationMethod;
    }

    private void OnDestroy()
    {
        SpawnInAtCurrentPosition -= InstatiationMethod;
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            InstatiationMethod();
    }
#endif
    

    private void InstatiationMethod()
    {
        minScaleX = -maxScaleX;
        minScaleY = -maxScaleY;
        
        switch (blockCheck)
        {
            case BlockCheck.LayerMask:
                InstatiateObjects(rayBlockerMask);
                break;
            case BlockCheck.Tags:
                InstatiateObjects(rayBlockerTags);
                break;
            default:
                SpawnInProps(maxScaleX, maxScaleY, minScaleX, minScaleY);
                break;
        }
    }
    
    public void InstatiateObjects(LayerMask blockerMask)
    {
        float tempMaxY = ChangeScale(Vector3.forward, maxScaleY, ref blockerMask);
        float tempMinY = ChangeScale(Vector3.back, minScaleY, ref blockerMask);
        float tempMaxX = ChangeScale(Vector3.right, maxScaleX, ref blockerMask);
        float tempMinX = ChangeScale(Vector3.left, minScaleX, ref blockerMask);
        
        SpawnInProps(tempMaxX, tempMaxY, tempMinX, tempMinY);
    }

    public void InstatiateObjects(string[] tagBlockers)
    {
        float tempMaxY = ChangeScale(Vector3.forward, maxScaleY, ref tagBlockers);
        float tempMinY = ChangeScale(Vector3.back, minScaleY, ref tagBlockers);
        float tempMaxX = ChangeScale(Vector3.right, maxScaleX, ref tagBlockers);
        float tempMinX = ChangeScale(Vector3.left, minScaleX, ref tagBlockers);
        
        SpawnInProps(tempMaxX, tempMaxY, tempMinX, tempMinY);
    }

    private float ChangeScale(Vector3 direction, float scale, ref LayerMask blockerMask) 
    {
        float tempf = scale < 0 ? -scale : scale;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, tempf, blockerMask)) 
        {
            float a = Vector3.Distance(transform.position, hit.transform.position);

            if (scale < 0)
                return -a + paddingWall;
            
            return a - paddingWall;
        }
        return scale;
    }

    private float ChangeScale(Vector3 direction, float scale, ref string[] tagBlockers)
    {
        float tempf = scale < 0 ? -scale : scale;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, tempf)) 
        {
            foreach(string tag in tagBlockers)
            {
                if(hit.transform.CompareTag(tag))
                    break;
                return scale;

            }
            float a = Vector3.Distance(transform.position, hit.transform.position);

            if (scale < 0)
                return -a + paddingWall;
            
            return a - paddingWall;

        }
        return scale;
    }

    private void SpawnInProps(float maxX, float maxY, float minX, float minY)
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < amount - 1; i++)
        {
            float randPositionx = Random.Range(minX, maxX);
            float randPositionz = Random.Range(minY, maxY);
           
            Vector3 tempVector = new Vector3(randPositionx, transform.position.y, randPositionz);
            int randInt = rand.Next(0, objectsToSpawn.Length-1);
            GameObject tempObj = objectsToSpawn[randInt].gameObject;
            // Can add a random or specific rotaion
            Instantiate(tempObj, tempVector, transform.rotation, transform);
        }
    }
}
