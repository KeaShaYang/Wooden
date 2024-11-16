using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseScale : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Transform tran;
    

    void Awake()
    {
        tran = transform;
    }
    
    void OnEnable()
    {
        Vector3 targetScale = target.localScale;
        tran.localScale = new Vector3(1 / targetScale.x, 1/ targetScale.y, 1/ targetScale.z);
    }
}
