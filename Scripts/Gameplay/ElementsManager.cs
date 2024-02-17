using System.Collections.Generic;
using UnityEngine;

public class ElementsManager : MonoBehaviour
{
    public static ElementsManager Instance;

    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private GameObject _JointPrefab;

    [SerializeField] private List<Element> _elements;

    [SerializeField] private Color32[] _lineColors;

    private List<Element> _allElements;

    private SpriteRenderer _joint;

    private LineRenderer _currentLine;
    private EdgeCollider2D _currentLineCollider;

    private Element _selectedElement;

    private bool _lineFree;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        GetAllElements();
    }
    private void Update()
    {
        UpdateElements();
        UpdatePlayerInput();

        if(_joint != null)
            _joint.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        if (_currentLine != null)
        {
            UpdateLineCollider(false);
            CheckLine();

            _currentLine.SetPosition(1, new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        }


    }
    private void UpdatePlayerInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) // Проверяем нажатие или отпускание левой кнопки мыши
        {
            Debug.Log("1");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1<<LayerMask.NameToLayer("Elements"));

            if (hit.collider != null)
            {
                Debug.Log("2");
                Element element = hit.collider.GetComponent<Element>();
                if (element != null)
                {
                    OnElementClicked(element);
                }
            }
        }
    }
    private void GetAllElements()
    {
        _allElements = FindObjectOfType<LevelInfo>().Elements;
    }
    private void OnElementClicked(Element element)
    {
        if (_elements.Count == 0)
        {
            Select(element);
        }
        else
        {
            if (element.ConnectedIn == null && _selectedElement != element)
            {
                ConnectElements(_selectedElement, element);
            }
        }
    }
    private void ConnectElements(Element firstElement, Element secondElement)
    {
        if (firstElement.ElementType != secondElement.ElementType)
        {
            if (!_lineFree)
                return;

            firstElement.ConnectedTo = secondElement;
            secondElement.ConnectedIn = firstElement;

            _currentLine.SetPosition(1, secondElement.PointForSelect.position);
            UpdateLineCollider(true);

            Destroy(_joint.gameObject);
            _joint = null;

            secondElement.PreviousLine = _currentLine;
            Select(secondElement);

            AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.Correct, 1f);

            CheckComplete();
        }
        else
            AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.Wrong, 1f);
    }
    private void CheckLine()
    {
        Vector3 startPosition = _currentLine.GetPosition(0);
        Vector3 endPosition = _currentLine.GetPosition(1);

        if (_elements.Count >= 3)
        {
            RaycastHit2D hit = Physics2D.Linecast(startPosition, endPosition, 1 << 3);

            if (hit.collider != null)
            {
                _lineFree = false;
                _currentLine.startColor = _lineColors[2];
                _currentLine.endColor = _lineColors[2];
            }
            else
            {
                _lineFree = true;
                _currentLine.startColor = _lineColors[(int)_selectedElement.ElementType];
                _currentLine.endColor = _lineColors[(int)_selectedElement.ElementType];
            }
        }
    }
    private void UpdateElements()
    {
        if (_elements.Count >= 3)
        {
            foreach (var element in _elements)
            {
                if (element.PreviousLine != null)
                    element.PreviousLine.gameObject.layer = 3;
            }
            _elements[_elements.Count - 1].PreviousLine.gameObject.layer = 0;
        }
        else
        {
            foreach (var element in _elements)
            {
                if (element.PreviousLine != null)
                    element.PreviousLine.gameObject.layer = 0;
            }
        }
    }
    private void UpdateLineCollider(bool fullLength)
    {
        Vector3[] linePositions = new Vector3[_currentLine.positionCount];
        _currentLine.GetPositions(linePositions);

        Vector2[] colliderPoints = new Vector2[2];

        colliderPoints[0] = linePositions[0];

        Vector2 direction = ((Vector2)linePositions[1] - colliderPoints[0]).normalized;
        float length = fullLength ? Vector2.Distance(colliderPoints[0], (Vector2)linePositions[1]) : Vector2.Distance(colliderPoints[0], (Vector2)linePositions[1]) - 0.15f;

        colliderPoints[1] = colliderPoints[0] + direction * length;

        _currentLineCollider.points = colliderPoints;
    }
    private void Select(Element element)
    {
        _selectedElement = element;

        _currentLine = Instantiate(_linePrefab).GetComponent<LineRenderer>();
        _currentLine.SetPosition(0, element.PointForSelect.position);
        _currentLine.SetPosition(1, element.transform.position);

        _joint = Instantiate(_JointPrefab).GetComponent<SpriteRenderer>();
        _joint.color = _lineColors[(int)_selectedElement.ElementType];

        _lineFree = true;
        _currentLine.startColor = _lineColors[(int)_selectedElement.ElementType];
        _currentLine.endColor = _lineColors[(int)_selectedElement.ElementType];

        _currentLineCollider = _currentLine.GetComponent<EdgeCollider2D>();

        _elements.Add(element);
    }
    private void CheckComplete()
    {
        bool isCompleted = true;
        foreach (Element element in _allElements)
        {
            if (element.ConnectedIn == null)
                isCompleted = false;
            if (element.ConnectedTo == null)
                isCompleted = false;
        }

        if (isCompleted)
        {
            _currentLine = null;
            if (_joint != null)
                Destroy(_joint.gameObject);
            _joint = null;

            PlayerPrefs.SetInt("LevelLocked" + (LevelManager.Instance.CurrentLevelId+1), 0);
            GameState.Instance.FinishGame();
        }
    }
    public Element GetSelectedElement() { return _selectedElement; }
    public List<Element> GetElements() { return _allElements; }
}
