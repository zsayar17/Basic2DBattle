using UnityEngine;

public class CameraCoordinator
{
    #region  Variables
    static CameraCoordinator _Instance;
    public static CameraCoordinator Instance { get => _Instance == null ? _Instance = new CameraCoordinator() : _Instance; }
    private Vector3 _dragOrigin;
    private bool _isDragging;
    Camera _Camera = Camera.main;

    public float ZoomSpeed;
    public float MinSize;
    public float MaxSize;

    #endregion

    /*
        Tetiklendiginde calisan metod
    */
    public void Update()
    {
        Move();
        Scroll();
    }

    /*
        kamera hareketi
    */
    private void Move()
    {
        Vector3 difference;

        if (Input.GetMouseButtonDown(2) && !_isDragging)
        {
            _dragOrigin = Input.mousePosition;
            _isDragging = true;
        }
        else if (Input.GetMouseButton(2) && _isDragging)
        {
            difference = Camera.main.ScreenToWorldPoint(_dragOrigin) - Utils.ScreenToWorld;

            _Camera.transform.position += difference;
            _dragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            _isDragging = false;
        }
    }

    /*
        Scroll ile yakınlasma ve uzaklasma yapilan yer
    */
    private void Scroll()
    {
        float newSize;

        newSize = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        newSize = Mathf.Clamp(newSize, MinSize, MaxSize);
        Camera.main.orthographicSize = newSize;
    }
}
