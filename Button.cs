using System.Linq;
using UnityEngine;
using VRUIControls;

namespace TwitchIRC
{
    class Button : MonoBehaviour
    {
        public string message;
        private VRPointer vrPointer;
        private VRController vrController;
        private bool pressed = false;
        private const float maxDistance = .025f;
        public void Initialize(string message)
        {
            this.message = message;
            vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
            Plugin.Log($"Button {message} initialized");
        }
        public void Update()
        {
            vrController = vrPointer?.vrController;
            if (vrController != null)
            {
                if (vrController.triggerValue > 0.9f)
                {
                    if (Vector3.Distance(vrController.transform.position, transform.position) < maxDistance)
                    {
                        if (pressed == false)
                        {
                            Plugin.Log($"Button {message} pressed.");
                            pressed = true;
                            Plugin.twitchIRC.SendTwitchMessage(message);
                        }
                    }
                }
            }
            if (vrController.triggerValue < 0.9f)
                pressed = false;
        }
    }
}
