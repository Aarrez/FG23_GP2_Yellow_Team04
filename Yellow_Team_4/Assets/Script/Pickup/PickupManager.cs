using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum EPickupType 
{
    NONE,
    COIN,
}

public class PickupManager : MonoBehaviour
{
    [SerializeField] private PickupData pickupData;

    public List<GameObject> pickupPool = new List<GameObject>();

    private static PickupManager instance;

    public static PickupManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<PickupManager>();
            }
            if (instance == null) {
                var pmo = new GameObject("PickupManager");
                var pm = pmo.AddComponent<PickupManager>();
                instance = pm;
            }

            return instance;
        }
    }

    public void ReturnPickup(GameObject pickup) {
        pickup.SetActive(false);
        pickupPool.Add(pickup);
    }

    void Spawn(EPickupType item, Vector3 position)
    {
        GameObject pickup = null;
        var filteredPickup = pickupPool.Where(x => x.tag == item.ToString()).ToList();
        if (filteredPickup.Count > 0)
        {
            pickup = filteredPickup[0];
            pickupPool.Remove(pickup);
        }
        else
        {
            switch(item) {
                case EPickupType.COIN:
                pickup = Instantiate(pickupData.Coin); //I went with "CoinPrefab" but we need to replace it with the one we using in the project
                pickupPool.Add(pickup);
                break;
            }
        }
        if (pickup != null) {
            pickup.transform.position = position;
            pickup.SetActive(true);
        }    
    }
}
