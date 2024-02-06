using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(WaveController))]
public class WaterController : MonoBehaviour
{
    private MeshFilter meshFilter;

    private WaveController waveController;

    public WaveController WaveController {
        get { return waveController; }
    }

    public MeshFilter MeshFilter {
        get { return meshFilter; }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        waveController = GetComponent<WaveController>();
        waveController.Initialize(meshFilter.mesh.bounds.size.x * transform.localScale.x);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        waveController.OnUpdate(dt);

        // TODO: we move this code to the shader code
        Vector3[] vertices = meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = waveController.GetWaveHeight(transform.position.x + transform.TransformPoint(vertices[i]).x);
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }

    public Tuple<Vector3, Vector3> GetClosestNormal(Vector3 position) {
        Tuple<Vector3, Vector3> output = new Tuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
        var vlist = new List<Tuple<Vector3, Vector3>>();        
        int i = 0;
        foreach(var v in meshFilter.mesh.vertices) {
            var p = transform.position + transform.TransformPoint(v);
            var n = meshFilter.mesh.normals[i];
            vlist.Add(new Tuple<Vector3, Vector3>(p, n));
            i++;
        }

        var orderedList = vlist.OrderBy(x => (x.Item1 - position).magnitude).ToList();
        
        var t1 = (orderedList[0].Item1 - position).magnitude / (orderedList[1].Item1 - orderedList[0].Item1).magnitude;        
        var iposition1 = Vector3.Lerp(orderedList[0].Item1, orderedList[1].Item1, t1);
        var inormal1 = Vector3.Lerp(orderedList[0].Item2, orderedList[1].Item2, t1);                
        
        var t2 = (orderedList[2].Item1 - position).magnitude / (orderedList[3].Item1 - orderedList[0].Item1).magnitude;        
        var iposition2 = Vector3.Lerp(orderedList[2].Item1, orderedList[3].Item1, t2);
        var inormal2 = Vector3.Lerp(orderedList[2].Item2, orderedList[3].Item2, t2);

        var t = (iposition1 - position).magnitude / (iposition2 - iposition1).magnitude;        
        var iposition = Vector3.Lerp(iposition1, iposition2, t);
        var inormal = Vector3.Lerp(inormal1, inormal2, t);

        output = new Tuple<Vector3, Vector3>(iposition, inormal);        
        return output;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (meshFilter != null) {
            int i=0;
            foreach(var v in meshFilter.mesh.vertices) {
                Gizmos.DrawLine(transform.position + transform.TransformPoint(v), transform.position + transform.TransformPoint(v) + meshFilter.mesh.normals[i]);
                i += 1;
            }
        }
    }
    #endif
}
