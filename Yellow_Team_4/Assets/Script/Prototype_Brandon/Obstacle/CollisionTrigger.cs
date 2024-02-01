using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnCollisionTrigger<T> {
    public void Invoke(T trigger, ControllerColliderHit collision);
}
