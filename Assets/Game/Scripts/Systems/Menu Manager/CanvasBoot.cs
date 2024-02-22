using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBoot : MonoBehaviour
{
    [SerializeField]private GameObject MenuRoot;



    private void Awake()
    {
        
        gameObject.SetActive(true);
        if(MenuRoot != null) MenuRoot.SetActive(true);
    }
}
