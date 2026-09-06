using System;
using UnityEngine;

namespace OmniUnityApp
{
    public class HelloWorldApp : MonoBehaviour
    {
        public string appTitle = "OmniUnity Antigravity AI - Hello Unity Showcase";
        public float rotationSpeed = 45f;
        public Color cubeColor = new Color(0.18f, 0.65f, 1.0f, 1.0f);

        private GameObject showcaseCube;
        private Transform orbitGroup;
        private float fps;
        private float fpsTimer;
        private int frameCount;
        private string statusMessage = "AI System Active - 100% Operational";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBootstrap()
        {
            Debug.Log("[OmniUnity] AutoBootstrap initialized.");

            // 1. Ensure Camera exists and renders with skybox
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.transform.position = new Vector3(0f, 1.8f, -5.5f);
                cam.transform.LookAt(new Vector3(0f, 0.4f, 0f));
            }
            cam.clearFlags = CameraClearFlags.Skybox;

            // 2. Ensure Directional Light exists
            if (FindFirstObjectByType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = new Color(1f, 0.98f, 0.92f);
                light.intensity = 1.4f;
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // 3. Ensure runner exists in scene
            if (FindFirstObjectByType<HelloWorldApp>() == null && FindFirstObjectByType<OmniUnity.MultipurposeRuntime>() == null)
            {
                var runnerGo = new GameObject("OmniUnity_Core");
                runnerGo.AddComponent<HelloWorldApp>();
                DontDestroyOnLoad(runnerGo);
            }
        }

        void Start()
        {
            Debug.Log("[OmniUnity] HelloWorldApp runtime started successfully.");

            // Find existing ShowcaseCube in scene or create one
            showcaseCube = GameObject.Find("ShowcaseCube");
            if (showcaseCube == null)
            {
                showcaseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                showcaseCube.name = "ShowcaseCube";
                showcaseCube.transform.position = new Vector3(0f, 0.5f, 0f);
                showcaseCube.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            ApplyColor(cubeColor);

            // Create dynamic orbiting visual nodes
            var orbitGo = new GameObject("OrbitGroup");
            orbitGo.transform.position = showcaseCube.transform.position;
            orbitGroup = orbitGo.transform;

            SpawnSatellite(PrimitiveType.Sphere, new Vector3(2.2f, 0.2f, 0f), 0.5f, Color.green);
            SpawnSatellite(PrimitiveType.Sphere, new Vector3(-2.2f, -0.2f, 0f), 0.5f, Color.yellow);
            SpawnSatellite(PrimitiveType.Cylinder, new Vector3(0f, 1.8f, 0f), 0.35f, Color.magenta);
        }

        void SpawnSatellite(PrimitiveType type, Vector3 localPos, float scale, Color col)
        {
            var sat = GameObject.CreatePrimitive(type);
            sat.name = "SatelliteNode";
            sat.transform.SetParent(orbitGroup, false);
            sat.transform.localPosition = localPos;
            sat.transform.localScale = Vector3.one * scale;
            var r = sat.GetComponent<Renderer>();
            if (r != null && r.material != null)
            {
                r.material.color = col;
            }
        }

        public void ApplyColor(Color col)
        {
            cubeColor = col;
            if (showcaseCube != null)
            {
                var r = showcaseCube.GetComponent<Renderer>();
                if (r != null && r.material != null)
                {
                    r.material.color = col;
                }
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;
            fpsTimer += Time.unscaledDeltaTime;
            frameCount++;
            if (fpsTimer >= 0.5f)
            {
                fps = frameCount / fpsTimer;
                frameCount = 0;
                fpsTimer = 0f;
            }

            if (showcaseCube != null)
            {
                showcaseCube.transform.Rotate(Vector3.up, rotationSpeed * dt, Space.World);
                showcaseCube.transform.Rotate(Vector3.right, rotationSpeed * 0.45f * dt, Space.Self);
            }

            if (orbitGroup != null)
            {
                orbitGroup.Rotate(Vector3.up, -rotationSpeed * 0.7f * dt, Space.World);
                orbitGroup.Rotate(Vector3.forward, rotationSpeed * 0.3f * dt, Space.World);
            }
        }

        void OnGUI()
        {
            GUI.backgroundColor = new Color(0.06f, 0.09f, 0.16f, 0.92f);
            float panelWidth = 440;
            float panelHeight = 320;
            float startX = 25;
            float startY = 25;

            GUI.Box(new Rect(startX, startY, panelWidth, panelHeight), "");
            GUILayout.BeginArea(new Rect(startX + 15, startY + 12, panelWidth - 30, panelHeight - 24));

            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(0.25f, 0.85f, 1.0f);
            GUILayout.Label(appTitle, headerStyle);

            var statusStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Italic
            };
            statusStyle.normal.textColor = new Color(0.4f, 1.0f, 0.5f);
            GUILayout.Label($"Status: {statusMessage}", statusStyle);
            GUILayout.Space(6);

            GUILayout.Label($"Engine      : Unity {Application.unityVersion} ({Application.platform})");
            GUILayout.Label($"Resolution  : {Screen.width} x {Screen.height}");
            GUILayout.Label($"Performance : {fps:F1} FPS ({Time.smoothDeltaTime * 1000f:F1} ms)");
            GUILayout.Label($"Graphics    : {SystemInfo.graphicsDeviceName}");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Change Color", GUILayout.Height(32)))
            {
                ApplyColor(UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f));
                statusMessage = $"Color changed at {DateTime.Now:HH:mm:ss}";
            }
            if (GUILayout.Button("Toggle Speed", GUILayout.Height(32)))
            {
                rotationSpeed = (rotationSpeed > 60f) ? 30f : (rotationSpeed + 35f);
                statusMessage = $"Speed set to {rotationSpeed:F0} deg/s";
            }
            if (GUILayout.Button("Reset Scene", GUILayout.Height(32)))
            {
                if (showcaseCube != null)
                {
                    showcaseCube.transform.position = new Vector3(0f, 0.5f, 0f);
                    showcaseCube.transform.rotation = Quaternion.identity;
                }
                ApplyColor(new Color(0.18f, 0.65f, 1.0f, 1.0f));
                rotationSpeed = 45f;
                statusMessage = "Scene reset to defaults.";
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            if (GUILayout.Button("Exit Application", GUILayout.Height(28)))
            {
                Application.Quit();
            }

            GUILayout.EndArea();
        }
    }
}

namespace OmniUnity
{
    // Direct backward compatibility alias so any scene referencing OmniUnity.MultipurposeRuntime loads seamlessly without missing script errors
    public class MultipurposeRuntime : OmniUnityApp.HelloWorldApp
    {
    }
}
