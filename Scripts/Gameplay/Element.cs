using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public ElementType ElementType => _elementType;
    [SerializeField] private ElementType _elementType;

    public Transform PointForSelect;

    public Element ConnectedIn;
    public Element ConnectedTo;

    public LineRenderer PreviousLine;
}
