using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] float zoomMin = 1.0f;
    [SerializeField] float zoomMax = 1.08f;

    [Header("Rotation")]
    [SerializeField] float rotationRange = 1.5f;

    [Header("Movement")]
    [SerializeField] float moveRange = 12f;

    [Header("Timing")]
    [SerializeField] float duration = 8f;

    private RectTransform _rect;
    private Vector3 _startScale, _targetScale;
    private Vector3 _startPos, _targetPos;
    private Vector3 _startRot, _targetRot;
    private float _elapsed;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        PickNewTarget(instant: true);
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
        float t = SmoothStep(_elapsed / duration); // calm in/out

        _rect.localScale = Vector3.Lerp(_startScale, _targetScale, t);
        _rect.localPosition = Vector3.Lerp(_startPos, _targetPos, t);
        _rect.localEulerAngles = Vector3.Lerp(_startRot, _targetRot, t);

        if (_elapsed >= duration) PickNewTarget(instant: false);
    }

    void PickNewTarget(bool instant)
    {
        _elapsed = 0f;

        _startScale = instant ? Vector3.one : _targetScale;
        _startPos = instant ? Vector3.zero : _targetPos;
        _startRot = instant ? Vector3.zero : _targetRot;

        _targetScale = Vector3.one * Random.Range(zoomMin, zoomMax);
        _targetPos = new Vector3(Random.Range(-moveRange, moveRange), Random.Range(-moveRange, moveRange), 0);
        _targetRot = new Vector3(0, 0, Random.Range(-rotationRange, rotationRange));
    }

    float SmoothStep(float t) => t * t * (3f - 2f * t);
}