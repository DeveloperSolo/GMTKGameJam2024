using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Camera Panning Settings")]
    [SerializeField] private float panningDuration = 1.0f;
    [SerializeField] private AnimationCurve panningCurve;

    private Coroutine panningCoroutine = null;

    [Header("Interaction Settings")]
    [SerializeField] private Transform defaultFocusTransform;

    private Vector3 startDragMousePosition;
    private Vector3 startDragCameraPosition;

    private void Update()
    {
        if(panningCoroutine != null)
        {
            Debug.Log("help");
            return;
        }

        if(GameController.GetMouseButtonUp(MouseButton.Middle))
        {
            ResetToFocusObject();
            ProcessMovementByMouseDragging(true);
        }

        if(GameController.GetMouseButton(MouseButton.Right))
        {
            ProcessMovementByMouseDragging(GameController.GetMouseButtonDown(MouseButton.Right));
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        panningCoroutine = null;
    }

    #region Mouse Dragging Movement

    private void ProcessMovementByMouseDragging(bool justStarted)
    {
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (justStarted)
        {
            startDragMousePosition = mousePosition;
            startDragCameraPosition = transform.position;
            return;
        }

        Vector3 mouseMovement = mousePosition - startDragMousePosition;
        Vector3 cameraMovement = mouseMovement;

        Vector2 viewportSize = GetViewportWorldSize();
        cameraMovement.x *= viewportSize.x;
        cameraMovement.y *= viewportSize.y;
        transform.position = startDragCameraPosition - cameraMovement;

        transform.position = GameController.Instance.ClampToGameArea(transform.position, viewportSize);
    }

    #endregion Mouse Dragging Movement

    #region Reset Focus

    private void ResetToFocusObject()
    {
        if(defaultFocusTransform == null)
        {
            return;
        }

        Vector3 newPosition = defaultFocusTransform.position;
        newPosition.z = transform.position.z;
        newPosition = GameController.Instance.ClampToGameArea(newPosition, GetViewportWorldSize());

        StartPanningToPosition(newPosition);
    }

    #endregion Reset Focus

    #region Camera Panning

    private void StartPanningToPosition(Vector2 targetPos)
    {
        panningCoroutine = StartCoroutine(PanCameraToPosition(targetPos));
    }

    private IEnumerator PanCameraToPosition(Vector2 targetPos)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPos;
        endPos.z = startPos.z;

        float timeLeft = panningDuration;
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.deltaTime;
            float t = 1.0f - (timeLeft / panningDuration);
            transform.position = startPos + ((endPos - startPos) *  panningCurve.Evaluate(t));
        }
        transform.position = endPos;
        panningCoroutine = null;
    }

    #endregion Camera Panning

    private Vector2 GetViewportWorldSize()
    {
        Vector2 viewportSize = Vector2.zero;
        viewportSize.y = Camera.main.orthographicSize * 2.0f;
        viewportSize.x = viewportSize.y / Screen.height * Screen.width;
        return viewportSize;
    }
}