using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightStand : MonoBehaviour
{
    public Transform cameraTransform; 
    public Material highlightMaterialStand; 
    public Material defaultMaterialStand; 
    public Material highlightMaterialItem; 
    public Material defaultMaterialItem; 
    private GameObject currentHighlightedStand = null;

    void Update()
    {
        HighlightFrontStand();
    }

    void HighlightFrontStand()
    {
        GameObject closestStand = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform stand in transform)
        {
            if (stand.childCount > 0)
            {
                GameObject item = stand.GetChild(0).gameObject;
                Item itemScript = item.GetComponent<Item>();

                if (itemScript != null && itemScript.IsUnlocked())
                {
                    float distance = Vector3.Distance(cameraTransform.position, stand.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestStand = stand.gameObject;
                    }
                }
            }
            // Calculate the distance from the camera to the stand
            
        }

        if (closestStand != null && closestStand != currentHighlightedStand)
        {
            if (currentHighlightedStand != null)
            {
                // Revert the previous highlighted stand and its item to default material
                SetMaterial(currentHighlightedStand, defaultMaterialStand, defaultMaterialItem);
            }

            // Highlight the new stand and its item
            SetMaterial(closestStand, highlightMaterialStand, highlightMaterialItem);
            currentHighlightedStand = closestStand;
        }
    }
    
    void SetMaterial(GameObject stand, Material standMat, Material itemMat)
    {
        Renderer standRenderer = stand.GetComponent<Renderer>();
        if (standRenderer != null)
        {
            standRenderer.material = standMat;
        }

        if (stand.transform.childCount > 0)
        {
            GameObject item = stand.transform.GetChild(0).gameObject;
            Renderer itemRenderer = item.GetComponent<Renderer>();
            if (itemRenderer != null)
            {
                itemRenderer.material = itemMat;
            }
        }
    }
}
