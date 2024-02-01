using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger {
    GameObject GObject { get; }    
}

public interface IForceTrigger : ITrigger {
    Vector3 ForcePoint { get; set; }
    Vector3 ForceDirection { get; set; }
}
