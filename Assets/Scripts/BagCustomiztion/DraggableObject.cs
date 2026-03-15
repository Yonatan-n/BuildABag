using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Camera mainCamera;

    private float minX, maxX, minY, maxY;
    private bool hasParentBounds = false;

    private Vector3 offset;
    private bool isDragging = false;
    private Vector2 pointerDownPosition;

    [Tooltip("Pixels the pointer must move before drag starts (prevents suppressing clicks)")]
    [SerializeField] private float dragThreshold = 10f;

    void Start()
    {
        mainCamera = Camera.main;
        CalculateParentBounds();
    }

    void CalculateParentBounds()
    {
        if (transform.parent == null) return;

        Collider2D parentCollider = transform.parent.GetComponent<Collider2D>();
        if (parentCollider != null)
        {
            Bounds bounds = parentCollider.bounds;
            Vector3 mySize = GetComponent<Collider2D>()?.bounds.extents ?? Vector3.zero;

            minX = bounds.min.x + mySize.x;
            maxX = bounds.max.x - mySize.x;
            minY = bounds.min.y + mySize.y;
            maxY = bounds.max.y - mySize.y;
            hasParentBounds = true;
        }
    }

    // ─── EventSystem handlers ─────────────────────────────────────────────────

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
        pointerDownPosition = eventData.position;
        offset = transform.position - ScreenToWorld(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Only start dragging after the threshold is exceeded
        if (!isDragging)
        {
            if (Vector2.Distance(eventData.position, pointerDownPosition) < dragThreshold)
                return;

            isDragging = true;
        }

        MoveToPosition(ScreenToWorld(eventData.position) + offset);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    // ─── Shared helpers ───────────────────────────────────────────────────────

    void MoveToPosition(Vector3 newPosition)
    {
        if (hasParentBounds)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        float z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
    }
}