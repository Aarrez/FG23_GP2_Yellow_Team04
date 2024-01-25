using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnInObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] ObjectsToSpawn;

    [SerializeField] private int amount = 1;
    
    [SerializeField] private float maxScaleX = 1f;
    private float minScaleX;
    [SerializeField] private float maxScaleY = 1f;
    private float minScaleY;
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(maxScaleX * 2, 1, maxScaleY * 2));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InstatiateObjects();
        }
    }
    
    public void InstatiateObjects()
    {
        System.Random rand = new System.Random();
        minScaleX = -maxScaleX;
        minScaleY = -maxScaleY;
        
        float tempMaxY = ChangeScale(Vector3.forward, ref maxScaleY);
        float tempMinY = ChangeScale(Vector3.back, ref minScaleY);
        float tempMaxX = ChangeScale(Vector3.right, ref maxScaleX);
        float tempMinX = ChangeScale(Vector3.left, ref minScaleX);
        
        for (int i = 0; i < amount - 1; i++)
        {
            int randInt = rand.Next(0, ObjectsToSpawn.Length-1);
            GameObject tempObj = ObjectsToSpawn[randInt];
            Instantiate(tempObj);
            tempObj.name = $"Prop{i}";
            float randPositionx = Random.Range(tempMinX, tempMaxX);
            float randPositionz = Random.Range(tempMinY, tempMaxY);
           
            Vector3 tempVector = new Vector3(randPositionx, transform.position.y, randPositionz);
            tempObj.transform.position = tempVector;
        }
    }

    private float ChangeScale(Vector3 direction, ref float scale)
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, scale, LayerMask.GetMask("Default"))) 
        {
            float a = Vector3.Distance(transform.position, hit.transform.position);
            
            if (scale < 0)
                return -a;
            
            return a;

        }
        return scale;
    }
}
