using UnityEngine;

public class UserInputHandler : MonoBehaviour
{
    private Controls _input;

    [SerializeField] private EditorMode _editorMode;
    [SerializeField] private TextContainerSelectionMode _selectionMode;
    [SerializeField] private CameraMotion _camMotion;

    private void Awake()
    {
        _input = new Controls();
        _input.UserInput.CamRotationActivator.performed += context => _camMotion.OnCamRotationActivate();
        _input.UserInput.CamRotationActivator.canceled += context => _camMotion.OnCamRotationDeactivate();
        _input.UserInput.CamRotationActivator.Enable();
        _input.UserInput.DeltaMouse.Enable();
        _input.UserInput.MapMoving.performed += context => _camMotion.OnMapMovingActivated(_input.UserInput.DeltaMouse);
        _input.UserInput.MapMoving.canceled += context => _camMotion.OnMapMovingDeactivated();
        _input.UserInput.MapMoving.Enable();

        _input.EditorMode.Select.performed += context => _editorMode.CreateTextContainer();
        
        _input.SelectionMode.Multiselect.performed += context => _selectionMode.OnMultiselectClick(true);
        _input.SelectionMode.Multiselect.canceled += context => _selectionMode.OnMultiselectClick(false);
        _input.SelectionMode.HoldSelection.performed += context => _selectionMode.OnHoldSelection(_input.UserInput.DeltaMouse);
        _input.SelectionMode.Select.performed += context => _selectionMode.OnClick();
        _input.SelectionMode.Select.canceled += context => _selectionMode.OnClickEnd();
        _input.SelectionMode.Delete.performed += context => _selectionMode.Delete();
        _input.SelectionMode.AlterMotion.performed += context => _selectionMode.OnAlterMotion(true);
        _input.SelectionMode.AlterMotion.canceled += context => _selectionMode.OnAlterMotion(false);

        _editorMode.TextContainerCreated += OnEnableSelectionMode;
    }

    private void OnEnableSelectionMode(GameObject obj)
    {
        Debug.Log("OnEnableSelectionMode();");
        _selectionMode.SelectCreatedContainer(obj);
        _input.EditorMode.Disable();
        _input.SelectionMode.Enable();
    }

    public void OnEnableEditMode()
    {
        Debug.Log("OnEnableEditMode();");
        _input.SelectionMode.Disable();
        _input.EditorMode.Enable();
        _editorMode.StartEditMode();
    }
    
}
