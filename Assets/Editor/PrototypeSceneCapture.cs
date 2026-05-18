using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ValleDePlata.Editor
{
    public static class PrototypeSceneCapture
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";
        private const string CapturePath = "docs/prototype_reports/vertical_slice_readability_pass_0_3_2026-05-18.png";

        [MenuItem("Valle de Plata/Capture Phase 1 Believability Overview")]
        public static void CapturePhase1BelievabilityOverview()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var cameraObject = new GameObject("Temporary Believability Capture Camera");
            var renderTexture = new RenderTexture(1280, 720, 24);
            var texture = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            Camera camera = null;

            try
            {
                camera = cameraObject.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Skybox;
                camera.fieldOfView = 44f;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 120f;
                cameraObject.transform.position = new Vector3(-12f, 9.4f, -27f);
                cameraObject.transform.LookAt(new Vector3(0f, 1.45f, 22f));
                camera.targetTexture = renderTexture;

                camera.Render();
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();

                var directory = Path.GetDirectoryName(CapturePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(CapturePath, texture.EncodeToPNG());
                AssetDatabase.Refresh();
                Debug.Log($"Phase 1 believability capture written to {CapturePath}");
            }
            finally
            {
                if (camera != null)
                {
                    camera.targetTexture = null;
                }

                RenderTexture.active = null;
                renderTexture.Release();
                Object.DestroyImmediate(texture);
                Object.DestroyImmediate(renderTexture);
                Object.DestroyImmediate(cameraObject);
            }
        }
    }
}
