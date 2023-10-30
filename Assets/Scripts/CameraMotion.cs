using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMotion : MonoBehaviour
{
    private CinemachineFreeLook _rotationCam;
    private CinemachineVirtualCamera _topDownCam;
    private Camera _mainCam;
    private GameObject _rotationCamTarget;
    private bool _isTopDownCamMoveActive;

    [SerializeField] private GameObject _rotationCamObj;
    [SerializeField] private GameObject _topDownCamObj;
    [SerializeField] private GameObject _topDownCamTarget;
    [SerializeField] private float _mapScrollingSpeed;

    private void Awake()
    {
        _mainCam = Camera.main;
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        _rotationCam = _rotationCamObj.GetComponent<CinemachineFreeLook>();
        _topDownCam = _topDownCamObj.GetComponent<CinemachineVirtualCamera>();
        _isTopDownCamMoveActive = false;
    }

    public void OnCamRotationActivate()
    {
        var screenRay = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f,300));
        if (Physics.Raycast(screenRay, out var hitInfo))
        {
            _rotationCamTarget = new GameObject();
            _rotationCamTarget.transform.position = hitInfo.point;

            _rotationCam.Follow = _rotationCamTarget.transform;
            _rotationCam.LookAt = _rotationCamTarget.transform;
        }

        _rotationCamObj.SetActive(true);
    }

    public void OnCamRotationDeactivate()
    {
        _rotationCamObj.SetActive(false);
        GameObject.Destroy(_rotationCamTarget);
    }

    public void OnCameraUpdated(CinemachineBrain brain)
    {
        if (brain.ActiveVirtualCamera == (ICinemachineCamera)_topDownCam && !brain.IsBlending)
        {
            var mainCamPos = _mainCam.transform.position;
            var targetPos = _topDownCamTarget.transform.position;
            if (Mathf.Abs(mainCamPos.x - targetPos.x) > 20f || Mathf.Abs(mainCamPos.z - targetPos.z) > 20f)
            {
                _topDownCamTarget.transform.position = new Vector3(mainCamPos.x, targetPos.y, mainCamPos.z);
            }
        }
    }

    public void OnMapMovingActivated(InputAction deltaMouse)
    {
        _isTopDownCamMoveActive = true;
        StartCoroutine(MoveCam(deltaMouse));
    }

    public void OnMapMovingDeactivated()
    {
        _isTopDownCamMoveActive = false;
    }
    private IEnumerator MoveCam(InputAction deltaMouse)
    {

        while (_isTopDownCamMoveActive)
        {
            var mouseInput = deltaMouse.ReadValue<Vector2>().normalized;
            _topDownCamTarget.transform.Translate(new Vector3(-mouseInput.x, 0f, -mouseInput.y) * _mapScrollingSpeed);

            yield return null;
        }
    }

}
