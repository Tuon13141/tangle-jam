using UnityEngine;

public class CameraSizeHandler : Kit.Common.Singleton<CameraSizeHandler>
{
    [SerializeField] Camera cam;
    [SerializeField] Vector2Int targetSize;

    public Transform topObject;
    public Transform bottomObject;

    public static float topOffset = -0.5f;
    public static float bottomOffset = 3.25f;


    private float previousUpdateAspect;
    private float cameraZoom = 1;

    private float targetAspect;
    private float initialSize;

    private Vector2 initCamSize;
    private Vector2 camSize;

    private Vector3 initPosition;

    [SerializeField] private float cameraZOffset = -2f;


    protected override void Awake()
    {
        base.Awake();

        initialSize = cam.orthographicSize;
        targetAspect = (float)targetSize.x / targetSize.y;

        initCamSize = new Vector2(2f * initialSize * cam.aspect, 2f * initialSize);
        initPosition = cam.transform.localPosition;
    }

    private void OnDrawGizmos()
    {
        var view = GetZoneView();
        var center = new Vector3(view.Item1.position.x, 0, view.Item1.position.y);
        var size = view.Item1.size;

        Kit.GizmosExtend.DrawSquare(new Vector3(view.Item2.position.x, 0, view.Item2.position.y), Vector3.up, view.Item2.size, 0f, Color.red);

        Kit.GizmosExtend.DrawSquare(center, Vector3.up, view.Item1.size, 0f, Color.green);
        Kit.GizmosExtend.DrawPoint(center);

        var z1 = (center.z + size.y / 2f) - 6.1f;
        Kit.GizmosExtend.DrawPoint(new Vector3(0, 0, z1));
        Kit.GizmosExtend.DrawLine(new Vector3(-500, 0, z1), new Vector3(1000, 0, z1));

        var z2 = (center.z - size.y / 2f + 3.25f);
        Kit.GizmosExtend.DrawPoint(new Vector3(0, 0, z2));
        Kit.GizmosExtend.DrawLine(new Vector3(-500, 0, z2), new Vector3(1000, 0, z2));
    }

    void Update()
    {
        if (!Mathf.Approximately(previousUpdateAspect, cam.aspect))
        {
            previousUpdateAspect = cam.aspect;
            UpdateCameraSize();
        }
    }

    public (Rect, Rect) GetZoneView()
    {
        Rect zoneView, safeView;
        if (Application.isPlaying)
        {
            if (!Mathf.Approximately(previousUpdateAspect, cam.aspect))
            {
                previousUpdateAspect = cam.aspect;
                UpdateCameraSize();
            }
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Plane dragPlane = new Plane(Vector3.up, Vector3.zero);
        dragPlane.Raycast(ray, out float distance);
        Vector3 center = ray.GetPoint(distance);
        Vector2 size = new Vector2(2f * cam.orthographicSize * cam.aspect, 2f * cam.orthographicSize);
        zoneView = new Rect(new Vector2(center.x, center.z), size);
        safeView = new Rect(new Vector2(center.x, center.z), size);

        //safe area
        Rect safeArea = Screen.safeArea;
        Resolution screenSize = Screen.currentResolution;
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop)
        {
            float top = 1 - ((safeArea.position.y + safeArea.height) / screenSize.height);
            float bottom = safeArea.position.y / screenSize.height;
            float left = safeArea.position.x / screenSize.width;
            float right = 1 - (safeArea.position.x + safeArea.width) / screenSize.width;

            safeView = new Rect(new Vector2(center.x, center.z + (size.y * (bottom - top) / 2)), new Vector2(size.x, size.y * (1 - (top + bottom))));
        }

        return (zoneView, safeView);
    }

    public void UpdateCameraSize()
    {
        if (targetAspect > cam.aspect)
        {
            cam.orthographicSize = initialSize * (targetAspect / cam.aspect) / cameraZoom;
        }
        else
        {
            cam.orthographicSize = initialSize / cameraZoom;
        }

        var view = GetZoneView();

        // Apply camera Z offset
        cam.transform.localPosition = initPosition + new Vector3(0, 0, cameraZOffset);

        if (topObject != null)
            topObject.position = new Vector3(0, 0, view.Item2.position.y + view.Item2.size.y / 2 + topOffset);

        if (bottomObject != null)
            bottomObject.position = new Vector3(0, 0, view.Item2.position.y - view.Item2.size.y / 2 + bottomOffset);
    }
}