using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TextContainerSelectionMode : MonoBehaviour
{
    private readonly string _textContainerTag = "Text container";
    private readonly string _editorPanelTag = "Editor panel";
    private readonly string _editorPanelHeader = "Name Panel";
    private readonly string _editorPanelHeaderText = "Плашка ";
    private readonly string _inputFieldName = "InputField";

    private int _collisionLayerMask;

    private GameObject _clickedUI;

    private GameObject _currentTextContainer;
    private GameObject _currentInputField;
    private Dictionary<GameObject, GameObject> _selectedObjects;
    private TextMeshProUGUI _currentTextContainerTMP;
    private bool _isMultipleSelectionMode;
    private bool _isClickEndWaiting;
    private bool _isSelectionHolded;
    private bool _isAlterMotion;

    [SerializeField] private Transform _editorRootPanel;
    [SerializeField] private GameObject _editorPanelPrefab;

    [SerializeField] private GameObject _detecterPrefab;

    private void Awake()
    {
        _selectedObjects = new Dictionary<GameObject, GameObject>();
        _isMultipleSelectionMode = false;
        _isClickEndWaiting = false;
        _isSelectionHolded = false;
        _collisionLayerMask = LayerMask.GetMask("Ignore Raycast");
    }

    public void SelectCreatedContainer(GameObject container)
    {
        Debug.Log("SelectCreatedContainer();");
        if (_currentTextContainer != null)
            Deselect();

        Select(container);
    }

    private void Select(GameObject container)
    {
        Debug.Log("SelectionMode.Select();");
        if (container == null)
            return;

        _currentTextContainer = container;
        _currentTextContainerTMP = container.GetComponentInChildren<TextMeshProUGUI>();

        var isContainerAdded = _selectedObjects.ContainsKey(container);
        if (isContainerAdded)
            _currentInputField = _selectedObjects[container];
        else
        {
            var editorPanel = GameObject.Instantiate(_editorPanelPrefab, _editorRootPanel);
            _currentInputField = editorPanel.transform.Find(_inputFieldName).gameObject;
            _selectedObjects.Add(_currentTextContainer, _currentInputField);
            var panelName = editorPanel.transform.Find(_editorPanelHeader).GetComponentInChildren<TextMeshProUGUI>();
            panelName.text = _editorPanelHeaderText + _selectedObjects.Count;
            editorPanel.SetActive(true);
        }

        var inputFieldTMP = _currentInputField.GetComponent<TMP_InputField>();
        inputFieldTMP.text = _currentTextContainerTMP.text;
        inputFieldTMP.onValueChanged.AddListener(InputChanges);
        inputFieldTMP.ActivateInputField();
        inputFieldTMP.Select();
    }

    private void Deselect(GameObject container = null)
    {
        Debug.Log("Deselect;");

        if (container != null)
        {
            if (container == _currentTextContainer)
                _currentTextContainer = null;
            _selectedObjects[container].transform.parent.GetComponent<Animator>().SetTrigger("Editor close");
            _selectedObjects.Remove(container);
            return;
        }

        _currentTextContainerTMP = null;
        _currentTextContainer = null;
        _currentInputField = null;

        foreach (var item in _selectedObjects.Keys)
        {
            _selectedObjects[item].transform.parent.GetComponent<Animator>().SetTrigger("Editor close");
        }

        _selectedObjects.Clear();
    }

    public void Delete()
    {
        Debug.Log("Delete()");

        if (_selectedObjects.Count == 0)
            return;

        var selectedContainers = _selectedObjects.Keys.ToArray();
        Deselect();
        
        for (int i = 0; i < selectedContainers.Length; i++)
        {
            GameObject.Destroy(selectedContainers[i]);
        }
    }

    public void OnClick()
    {
        if (_isMultipleSelectionMode)
        {
            StartCoroutine(CheckClickSelection());
            return;
        }
        Debug.Log("Multiple selection: " + _isMultipleSelectionMode);
        Debug.Log("SelectionMode.OnKlick();");
        if (_selectedObjects.Count <= 1)
            StartCoroutine(CheckClickSelection());

        else _isClickEndWaiting = true;
    }

    public void OnHoldSelection(InputAction deltaMouse = null)
    {
        Debug.Log("OnHoldSelection;");
        _isClickEndWaiting = false;
        _isSelectionHolded = true;

        if (_isAlterMotion && deltaMouse != null)
        {
            StartCoroutine(VerticalMoveSelection(deltaMouse));
            return;
        }
        //StartCoroutine(MoveSelection());
    }

    public void OnClickEnd()
    {
        Debug.Log("SelectionMode.OnKlickEnd();");

        if ( _isClickEndWaiting)
        {
            _isClickEndWaiting = false;
            StartCoroutine(CheckClickSelection());
        }
        _isSelectionHolded = false;
    }

    public void OnMultiselectClick(bool isActiveState)
    {
        Debug.Log($"SelectionMode.OnMultiselectClick({isActiveState});");
        _isMultipleSelectionMode = isActiveState;
    }

    public void OnAlterMotion(bool isActive)
    {
        _isAlterMotion = isActive;
    }

    private IEnumerator CheckClickSelection()
    {
        Debug.Log("Check cklick selection();");
        yield return new WaitForEndOfFrame();

        var selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            if (selectedObj.CompareTag(_textContainerTag))
            {
                if (_currentTextContainer == selectedObj)
                {
                    if (_selectedObjects.Count == 1)
                        yield break;
                }

                if (!_isMultipleSelectionMode)
                {
                    Deselect();
                    Select(selectedObj);
                    yield break;
                }
                else
                {
                    if (_selectedObjects.ContainsKey(selectedObj))
                    {
                        Deselect(selectedObj);
                        if (_currentTextContainer == null && _selectedObjects.Count > 0)
                        {
                            var textContainer = _selectedObjects.Keys.First();
                            Select(textContainer);
                            yield break;
                        }
                        yield break;
                    }
                    else
                    {
                        Select(selectedObj);
                        yield break;

                    }
                }
            }
            else if (selectedObj.CompareTag(_editorPanelTag))
            {
                if (selectedObj == _currentInputField || selectedObj == _currentInputField.transform.parent.gameObject)
                    yield break;

                var txtContainer = _selectedObjects.FirstOrDefault(pair => pair.Value == selectedObj || pair.Value.transform.parent.gameObject == selectedObj).Key;
                if (txtContainer == null)
                {
                    Debug.Log("Local Error! U should be smarter!");
                    yield break;
                }

                Select(txtContainer);
                yield break;
            }

            Deselect();
            yield break;
        }

        Deselect();
        yield break;
    }

    public void InputChanges(string change)
    {
        if (change == null)
            return;
        if (_currentTextContainerTMP == null)
            return;
        _currentTextContainerTMP.text = change;
    }

    private IEnumerator MoveSelection()
    {
        yield return new WaitForEndOfFrame();

        var selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null || !selectedObj.CompareTag(_textContainerTag))
            yield break;


        var mainCam = Camera.main;
        var mouse = Mouse.current;
        var oldPos = mouse.position.ReadValue();
        var curPos = oldPos;

        while (_isSelectionHolded)
        {
            Vector2 offset = default;
            do
            {
                curPos = mouse.position.ReadValue();
                offset = curPos - oldPos;

                if (offset == Vector2.zero)
                    yield return null;

            } while (offset == Vector2.zero);

            oldPos = curPos;
            //Debug.Log("Offset: " + offset.ToString());
            foreach (var container in  _selectedObjects.Keys) 
            {
                var containerPos = container.transform.position;
                var containerScreenPos = mainCam.WorldToScreenPoint(containerPos);
                var containerNewPos = containerScreenPos + (Vector3)offset;
                var worldPoint = mainCam.ScreenToWorldPoint(containerNewPos);


                var collider = container.GetComponent<Collider>();
                var size = collider.bounds.size;
                var normalSize = new Vector3(size.x, size.z, size.y);
                var raycastPoint = new Vector3(size.x, mainCam.transform.position.y, size.y);
                var distance = Vector3.Distance(worldPoint, containerPos);
                var direction = (worldPoint - containerPos).normalized;

                yield return new WaitForFixedUpdate();

                if (Physics.BoxCast(worldPoint, size * 0.5f, direction, out var hitInfo, Quaternion.identity, distance, container.layer))
                {
                    Debug.Log("Boxcast collision: " + hitInfo.collider.gameObject.name);
                    Debug.Log("Prev pos: " + containerPos.ToString());
                    container.transform.Translate(-Vector3.forward);
                    Debug.Log("Cur pos: " + container.transform.position.ToString());

                    var indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    indicator.transform.position = hitInfo.point;
                    continue;
                }
                Debug.Log("Empty boxcast;");
                //container.transform.position = worldPoint;
                yield return new WaitForFixedUpdate();
                container.GetComponent<Rigidbody>().MovePosition(worldPoint);
                
            }
            yield return null;
        }
    }

    private IEnumerator VerticalMoveSelection(InputAction deltaMouse)
    {
        yield return new WaitForEndOfFrame();

        var selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null || !selectedObj.CompareTag(_textContainerTag))
            yield break;

        while (_isSelectionHolded && _isAlterMotion)
        {
            var verticalDelta = deltaMouse.ReadValue<Vector2>().y;

            foreach (var container in _selectedObjects.Keys)
            {
                container.transform.Translate(Vector3.forward * verticalDelta);

                var curPos = container.transform.position;
                if (curPos.y > 125f)
                    container.transform.position = new Vector3(curPos.x, 125f, curPos.z);
            }
            yield return null;
        }
    }

    private void OnGUI()
    {
        _clickedUI = EventSystem.current.currentSelectedGameObject;
    }
    private Vector2 _oldPos;
    private void FixedUpdate()
    {
        if (!_isSelectionHolded)
        {
            _oldPos = Mouse.current.position.ReadValue();
            return;
        }

        if (_clickedUI == null || !_clickedUI.CompareTag(_textContainerTag))
            return;

        var mainCam = Camera.main;
        var mouse = Mouse.current;
        //var oldPos = mouse.position.ReadValue();
        //var curPos = oldPos;

        Vector2 offset = default;
        var curPos = mouse.position.ReadValue();
        offset = curPos - _oldPos;
        if (offset == Vector2.zero)
            return;

        _oldPos = curPos;
        //Debug.Log("Offset: " + offset.ToString());
        foreach (var container in _selectedObjects.Keys)
        {
            var containerPos = container.transform.position;
            var containerScreenPos = mainCam.WorldToScreenPoint(containerPos);
            var containerNewPos = containerScreenPos + (Vector3)offset;
            var worldPoint = mainCam.ScreenToWorldPoint(containerNewPos);


            var collider = container.GetComponent<Collider>();
            var size = collider.bounds.size;
            var normalSize = new Vector3(size.x, size.z, size.y);
            var raycastPoint = new Vector3(size.x, mainCam.transform.position.y, size.y);
            var distance = Vector3.Distance(worldPoint, containerPos);
            var direction = (worldPoint - containerPos).normalized;

            //var detector = GameObject.Instantiate(_detecterPrefab, worldPoint, Quaternion.identity);
            //Debug.Log($"Prev pos: {containerPos}; New pos: {worldPoint}; Distance: {distance};");
            var collisions = Physics.OverlapBox(worldPoint, size * 0.5f, Quaternion.identity, ~_collisionLayerMask);
            if (collisions.Length > 0)
            {
                //Debug.Log($"Boxcast collision: {hitInfo.collider.gameObject.name}; point: {hitInfo.point}");
                //Debug.Log("Prev pos: " + containerPos.ToString());
                container.GetComponent<Rigidbody>().MovePosition(new Vector3(containerPos.x, containerPos.y + 1f, containerPos.z));
                //Debug.Break();
                //Debug.Log("Cur pos: " + container.transform.position.ToString());

                //var indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //indicator.transform.position = hitInfo.point;
                //continue;
                //Debug.Break();   
            }
            else
            {
                Debug.Log("Empty boxcast;");
                container.GetComponent<Rigidbody>().MovePosition(worldPoint);

            }
            //container.transform.position = worldPoint;

        }


    }

}
