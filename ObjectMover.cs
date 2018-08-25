using System.Linq;
using UnityEngine;
using VRUIControls;

namespace TwitchIRC
{
    class ObjectMover : MonoBehaviour
    {
        private VRPointer vrPointer;
        private VRController vrController;
        private Transform grabbedObject;
        private VRController grabbingController;
        private Vector3 grabPosition;
        private Vector3 realPosition;
        private Quaternion grabRotation;
        private Quaternion realRotation;
        private const float minimumDistance = .25f;
        private const float grabDistance = .05f;
        public void Start()
        {
            vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
        }
        public void Update()
        {
            vrController = vrPointer?.vrController;
            if (vrController != null)
            {
                if(vrController.triggerValue > 0.9f)
                {
                    if (grabbingController != null) return;
                    GameObject grabbedObject = null;
                    Plugin.grabbables.ForEach((grabbable) =>
                    {
                        if(Vector3.Distance(grabbable.transform.position,vrController.position) < grabDistance)
                        {
                            grabbedObject = grabbable;
                        }
                    });
                    if (grabbedObject != null)
                    {
                        this.grabbedObject = grabbedObject.transform;
                        grabbingController = vrController;
                        grabPosition = vrController.transform.InverseTransformPoint(grabbedObject.transform.position);
                        grabRotation = Quaternion.Inverse(vrController.transform.rotation) * grabbedObject.transform.rotation;
                    }
                }
                if (grabbingController == null || !(grabbingController.triggerValue <= 0.9f))
                    return;
                if (grabbingController == null)
                    return;
                grabbingController = null;
                Plugin.SaveGrabbableTransform(grabbedObject.transform.position, grabbedObject.transform.rotation.eulerAngles, grabbedObject.name);
                grabbedObject = null;
            }
        }
        private void LateUpdate()
        {
            if (grabbedObject == null) return;
            if(grabbingController != null)
            {
                realPosition = grabbingController.transform.TransformPoint(grabPosition);
                realRotation = grabbingController.transform.rotation * grabRotation;
                grabbedObject.position = Vector3.Lerp(grabbedObject.position, realPosition, 10 * Time.deltaTime);
                grabbedObject.rotation = Quaternion.Slerp(grabbedObject.rotation, realRotation, 5 * Time.deltaTime);
            }
        }
    }
}
