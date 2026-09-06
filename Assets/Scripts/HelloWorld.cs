using System;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    private float rotationSpeed = 30.0f;
    private GameObject showcaseCube;

    void Start()
    {
        Debug.Log("Standalone Unity App started successfully!");
        showcaseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        showcaseCube.transform.position = Vector3.zero;
        var r = showcaseCube.GetComponent<Renderer>();
        if (r != null)
        {
            r.material.color = new Color(0.15f, 0.65f, 0.95f);
        }
    }

    void Update()
    {
        if (showcaseCube != null)
        {
            showcaseCube.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            showcaseCube.transform.Rotate(Vector3.right, rotationSpeed * 0.5f * Time.deltaTime, Space.Self);
        }
    }

    void OnGUI()
    {
        GUI.skin.box.fontSize = 16;
        GUI.skin.label.fontSize = 14;

        GUI.Box(new Rect(20, 20, 420, 220), "Unity Hello World - Multi-Platform CI/CD");
        GUILayout.BeginArea(new Rect(35, 55, 390, 175));
        GUILayout.Label("Unity Version: " + Application.unityVersion);
        GUILayout.Label("Platform: " + Application.platform);
        GUILayout.Label("Resolution: " + Screen.width + "x" + Screen.height);
        GUILayout.Label("Time Since Startup: " + Time.realtimeSinceStartup.ToString("F1") + "s");
        GUILayout.Label("Built automatically via GitHub Actions in Cloud!");
        GUILayout.EndArea();
    }
}
