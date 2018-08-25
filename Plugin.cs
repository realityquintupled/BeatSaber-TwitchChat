using IllusionPlugin;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwitchIRC
{

    public class Plugin : IPlugin
    {
        public string Name => "TwitchChat";
        public string Version => "v2.0";
        public static List<GameObject> grabbables = new List<GameObject>();
        public static TwitchIRC twitchIRC;
        public static Config config;
        private GameObject chatPanel;
        private GameObject responsePanel;
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            config = Config.LoadConfig(Strings.configPath);
            config.SaveConfig(Strings.configPath);
            if (config.oAuthToken == new Config().oAuthToken)
                return;
            GameObject twitchContainer = new GameObject("Twitch Container");
            twitchIRC = twitchContainer.AddComponent<TwitchIRC>();
            twitchIRC.SetCredentials(config.oAuthToken, config.username.ToLower(), config.channelName.ToLower());
            UnityEngine.Object.DontDestroyOnLoad(twitchContainer);
            GameObject chatContainer = new GameObject(Strings.chatPanelName);
            GameObject chatHandle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chatHandle.name = "Chat Handle";
            chatHandle.transform.parent = chatContainer.transform;
            chatHandle.transform.localScale = new Vector3(.05f, .05f, .05f);
            chatHandle.GetComponent<MeshRenderer>().material.shader = Shader.Find(Strings.cubeShader);
            chatHandle.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
            GameObject chat = new GameObject("Chat");
            chat.transform.parent = chatContainer.transform;
            chat.transform.localScale = new Vector3(config.chatSize, config.chatSize, config.chatSize);
            chat.transform.position = new Vector3(chatHandle.transform.localScale.x/2, chatHandle.transform.localScale.y/2, 0);
            twitchIRC.OnMessageReceived += chat.AddComponent<ChatManager>().OnMessageReceived;
            chatContainer.transform.position = LoadGrabbablePosition(chatContainer.name, new Vector3(0, 1, -1));
            chatContainer.transform.rotation = LoadGrabbableRotation(chatContainer.name, new Vector3(0, 180, 0));
            chatPanel = chatContainer;
            UnityEngine.Object.DontDestroyOnLoad(chatContainer);
            grabbables.Add(chatContainer);
            chatContainer.SetActive(PlayerPrefs.GetInt("ChatActive", 1) == 1 ? true : false);
            if (config.oAuthToken != string.Empty && config.responseButtons != string.Empty)
            {
                GameObject responseManager = new GameObject(Strings.responsePanelName);
                responseManager.transform.position = LoadGrabbablePosition(responseManager.name, new Vector3(1, 1, 0));
                responseManager.transform.rotation = LoadGrabbableRotation(responseManager.name, new Vector3(0, 90, 0));
                responseManager.AddComponent<ResponseManager>();
                responsePanel = responseManager;
                UnityEngine.Object.DontDestroyOnLoad(responseManager);
                grabbables.Add(responseManager);
                responseManager.SetActive(config.showResponses ? (PlayerPrefs.GetInt("ResponsesActive", 1) == 1 ? true : false) : false);
            }
            Log("Initialization " + (twitchIRC.InitializeIRC() ? "succeded." : "failed."));
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            new GameObject().AddComponent<ObjectMover>();
            if (arg1.buildIndex != 2)
                return;
            SubMenu twitchChatMenu = SettingsUI.CreateSubMenu("Twitch Chat");
            BoolViewController twitchChatPanel = twitchChatMenu.AddBool("Twitch Chat Panel");
            twitchChatPanel.GetValue += delegate { return chatPanel != null ? chatPanel.activeSelf : false; };
            twitchChatPanel.SetValue += delegate (bool value) { chatPanel?.SetActive(value); PlayerPrefs.SetInt("ChatActive", value ? 1 : 0); };
            BoolViewController twitchResponsePanel = twitchChatMenu.AddBool("Twitch Response Panel");
            twitchResponsePanel.GetValue += delegate { return responsePanel != null ? responsePanel.activeSelf : false; };
            twitchResponsePanel.SetValue += delegate (bool value) { responsePanel?.SetActive(value); PlayerPrefs.SetInt("ResponsesActive", value ? 1 : 0); };
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Log("Manual initialization " + (twitchIRC.InitializeIRC() ? "succeded." : "failed."));
            }
            if(Input.GetKeyDown(KeyCode.F7))
            {
                GameObject chatContainer = GameObject.Find(Strings.chatPanelName);
                if (chatContainer != null)
                {
                    chatContainer.transform.position = LoadGrabbablePosition(chatContainer.name, new Vector3(0, 1, -1));
                    chatContainer.transform.rotation = LoadGrabbableRotation(chatContainer.name, new Vector3(0, 180, 0));
                }
                GameObject responseManager = GameObject.Find(Strings.responsePanelName);
                if (responseManager != null)
                {
                    responseManager.transform.position = LoadGrabbablePosition(responseManager.name, new Vector3(1, 1, 0));
                    responseManager.transform.rotation = LoadGrabbableRotation(responseManager.name, new Vector3(0, 90, 0));
                }
            }
        }

        public void OnFixedUpdate()
        {
        }

        public static void SaveGrabbableTransform(Vector3 position, Vector3 rotation, string name)
        {
            PlayerPrefs.SetFloat(name + "_PosX", position.x);
            PlayerPrefs.SetFloat(name + "_PosY", position.y);
            PlayerPrefs.SetFloat(name + "_PosZ", position.z);
            PlayerPrefs.SetFloat(name + "_RotX", rotation.x);
            PlayerPrefs.SetFloat(name + "_RotY", rotation.y);
            PlayerPrefs.SetFloat(name + "_RotZ", rotation.z);
        }

        public static Vector3 LoadGrabbablePosition(string name, Vector3 defaults)
        {
            return new Vector3(PlayerPrefs.GetFloat(name + "_PosX", defaults.x), PlayerPrefs.GetFloat(name + "_PosY", defaults.y), PlayerPrefs.GetFloat(name + "_PosZ", defaults.z));
        }

        public static Quaternion LoadGrabbableRotation(string name, Vector3 defaults)
        {
            return Quaternion.Euler(PlayerPrefs.GetFloat(name + "_RotX", defaults.x), PlayerPrefs.GetFloat(name + "_RotY", defaults.y), PlayerPrefs.GetFloat(name + "_RotZ", defaults.z));
        }

        public static void Log(string message)
        {
            Console.WriteLine("[TwitchIRC] " + message);
        }
    }
}
