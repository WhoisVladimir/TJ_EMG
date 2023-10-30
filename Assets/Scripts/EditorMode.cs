using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorMode : MonoBehaviour
{
    private Camera _mainCam;

    [SerializeField] private Transform _rootWorldCanvas;
    [SerializeField] private GameObject _textContainerPrefab;
    [SerializeField] private Button _editorModeButton;

    public event Action<GameObject> TextContainerCreated;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    public void StartEditMode()
    {
        Debug.Log("EditorMode.StartEditMode()");
        if (_editorModeButton.IsInteractable())
        {
            _editorModeButton.interactable = false;
        }
    }

    public void CreateTextContainer()
    {
        Debug.Log("EditorMode.CreateTextContainer();");

        var mouseRay = _mainCam.ScreenPointToRay(Input.mousePosition);
        GameObject textContainer = null;
        if (Physics.Raycast(mouseRay, out var hitInfo))
        {
            var hitPoint = hitInfo.point;
            var instPoint = new Vector3(hitPoint.x, hitPoint.y + 10f, hitPoint.z + 8f);
            
            textContainer = GameObject.Instantiate(_textContainerPrefab, instPoint, _textContainerPrefab.transform.rotation, _rootWorldCanvas);
        }
        else return;

        _editorModeButton.interactable = true;
        TextContainerCreated?.Invoke(textContainer);
    }

}
