using UnityEngine;
using UnityEditor;

public class AutoGridSnap : EditorWindow
{
    private Vector3 _prevPosition;
    private bool _doSnap = true;
    private bool _globalPos = false;
    private Vector3 _snapValue = new Vector3(0.375f, 1.62f, 0.375f);

    [MenuItem("Edit/Auto Grid Snap %_l")]

    static void Init()
    {
        var window = (AutoGridSnap)GetWindow(typeof(AutoGridSnap));
        window.maxSize = new Vector2(200, 100);
    }

    public void OnGUI()
    {
        _doSnap = EditorGUILayout.Toggle("Auto Snap", _doSnap);
        _globalPos = EditorGUILayout.Toggle("Global Position", _globalPos);
        _snapValue = EditorGUILayout.Vector3Field("Snap Value", _snapValue);
    }

    public void Update()
    {
        if (!_doSnap || EditorApplication.isPlaying || Selection.transforms.Length <= 0 ||
            Selection.transforms[0].position == _prevPosition) return;
        Snap();
        _prevPosition = Selection.transforms[0].position;
    }

    private void Snap()
    {
        foreach (var transform in Selection.transforms)
        {
            //if (transform.GetComponent<BoxElementBase>() == null) return;

            Vector3 t = transform.transform.position;
            if (!_globalPos) t = transform.transform.localPosition;

            t.x = Round(t.x, _snapValue.x);
            t.y = Round(t.y, _snapValue.y);
            t.z = Round(t.z, _snapValue.z);

            if (_globalPos) transform.transform.position = t;
            else transform.transform.localPosition = t;
        }
    }

    private float Round(float input, float snapValue)
    {
        return snapValue * Mathf.Round((input / snapValue));
    }
}