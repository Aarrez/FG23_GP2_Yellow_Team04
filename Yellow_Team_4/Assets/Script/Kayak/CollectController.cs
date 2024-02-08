using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPickupObserver
{
    void OnCoinPickup(int coin);
    void Register();
}

public class CollectController : MonoBehaviour, IKayakEntity
{
    public int coins = 0; 
    
    private LinkedList<IPickupObserver> pickupObservers = new LinkedList<IPickupObserver>();
    private PickupManager pickupManager;

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(EPickupType.COIN.ToString()))
        {
            CollectCoin(col.gameObject);
        }
    }
   
    void CollectCoin(GameObject coin)
    {                
        if (pickupManager != null) pickupManager.ReturnPickup(coin);        

        coins++;              
        // in this script we can also add ui , sound etc update logic here
        var node = pickupObservers.First;
        while(node != null) {
            var next = node.Next;
            if (node.Value == null) pickupObservers.Remove(node);
            else {
                node.Value.OnCoinPickup(coins);                
            }
        }
    }

    public void Initialize(Kayak entity)
    {
        pickupManager = FindObjectOfType<PickupManager>();
        var puo = FindObjectsOfType<MonoBehaviour>().OfType<IPickupObserver>();
        foreach(var p in puo) {
            pickupObservers.AddLast(p);
        }
    }

    public void OnFixedUpdate(float dt)
    {
    }

    public void OnUpdate(float dt)
    {
    }    
}
