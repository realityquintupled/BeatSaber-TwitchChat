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
        private ObjectMover objectMover;
        private bool pressed = false;
        private const float maxDistance = .025f;
        public void Initialize(string message)
        {
            this.message = message;
            vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
            objectMover = FindObjectOfType<ObjectMover>();
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
                        if (objectMover != null && objectMover.grabbedObject == null)
                        {
                            if (pressed == false)
                            {
                                pressed = true;
                                Plugin.twitchIRC.SendTwitchMessage(message);
                            }
                        }
                    }
                }
            }
            if (vrController.triggerValue < 0.9f)
                pressed = false;
        }
    }
}
