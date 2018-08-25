using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwitchIRC
{
    class ResponseManager : MonoBehaviour
    {
        private List<GameObject> buttonObjects = new List<GameObject>();
        private Material material = new Material(Shader.Find(Strings.cubeShader));
        public void Start()
        {
            material.color = new Color(1, 1, 1, 0);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Response Handle";
            cube.transform.parent = transform;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localPosition = new Vector3(0, 0, 0);
            cube.transform.localScale = new Vector3(.05f, .05f, .05f);
            cube.GetComponent<MeshRenderer>().material = material;
            GameObject responsePanel = CreateResponsePanel(gameObject);
            responsePanel.transform.localRotation = Quaternion.identity;
            responsePanel.transform.localPosition = new Vector3(.2f , -1 * responsePanel.GetComponent<TextMeshPro>().preferredHeight / 2, 0);
            SceneManager.activeSceneChanged += (arg0, arg1) => {
                buttonObjects.ForEach(buttonObject =>
                {
                    Button button = buttonObject.GetComponent<Button>();
                    button.Initialize(button.message);
                });
            };
        }
        private GameObject CreateResponsePanel(GameObject parent)
        {
            GameObject responsePanel = new GameObject("Response Panel");
            responsePanel.transform.parent = parent.transform;
            responsePanel.transform.localPosition = new Vector3(0, 0, 0);
            TextMeshPro tmp = responsePanel.AddComponent<TextMeshPro>();
            RectTransform rectTransform = responsePanel.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 0);
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
            tmp.fontSize = .5f;
            tmp.text = string.Empty;
            string[] buttons = Plugin.config.responseButtons.Split(';').Reverse().ToArray();
            float halfHeight = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                tmp.text = buttons[i] + "\n" + tmp.text;
                if (halfHeight == -1)
                    halfHeight = tmp.preferredHeight / 2;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = responsePanel.transform;
                cube.transform.localPosition = new Vector3(-.0375f, tmp.preferredHeight - halfHeight, 0);
                cube.transform.localRotation = Quaternion.identity;
                cube.transform.localScale = new Vector3(.05f, .05f, .05f);
                cube.GetComponent<MeshRenderer>().material = material;
                cube.AddComponent<Button>().Initialize(buttons[i]);
                buttonObjects.Add(cube);
            }
            return responsePanel;
        }
    }
}
