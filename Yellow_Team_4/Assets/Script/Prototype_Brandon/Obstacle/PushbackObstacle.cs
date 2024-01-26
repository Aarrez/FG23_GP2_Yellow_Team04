using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface IOnCollisionTrigger<T> {
    public void Invoke(T trigger);
}

public class PushBackObstacle : MonoBehaviour
{
    [SerializeField] private LayerMask maskTargets;
    [SerializeField] private float radius = 1;
    [SerializeField] private ForceData forceData;    

    public ForceData ForceData {
        get { return forceData; }
    }        
    
    void Update() {
        var hits = Physics.OverlapSphere(transform.position, radius, maskTargets);                
        hits = hits.Where( x=> x.GetComponent<IOnCollisionTrigger<PushBackObstacle>>() != null).ToArray();
        if (hits.Length > 0) {
            foreach(var hit in hits) {
                hit.GetComponent<IOnCollisionTrigger<PushBackObstacle>>().Invoke(this);
            }            
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
