using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public List<Element> Elements = new List<Element>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            Elements.Add(transform.GetChild(i).GetComponent<Element>());
        }
    }
}
