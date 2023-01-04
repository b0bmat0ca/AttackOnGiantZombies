using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace raspberly.ovr
{
    public class OVRFirstGrabber : MonoBehaviour
    {
        public float grabBegin = 0.55f;
        public float grabEnd = 0.35f;

        [SerializeField]
        protected Transform m_gripTransform = null;

        [SerializeField]
        protected bool m_parentHeldObject = false;

        [SerializeField]
        protected OVRInput.Controller m_controller;

        [SerializeField]
        protected Transform m_parentTransform;


        [SerializeField] protected OVRGrabbable firstGrabbable;
        [SerializeField] protected Animator handAnimator;
        [SerializeField] protected RuntimeAnimatorController handAnimatorController;

        protected bool m_grabVolumeEnabled = true;
        protected Vector3 m_lastPos;
        protected Quaternion m_lastRot;
        protected Quaternion m_anchorOffsetRotation;
        protected Vector3 m_anchorOffsetPosition;
        protected float m_prevFlex;
        protected OVRGrabbable m_grabbedObj = null;
        protected Vector3 m_grabbedObjectPosOff;
        protected Quaternion m_grabbedObjectRotOff;
        protected bool operatingWithoutOVRCameraRig = true;
        protected OVRGrabber grabber;

        protected bool firstTrigger;
        protected bool firstUpdate;

        protected void Awake()
        {
            grabber = gameObject.GetComponent<OVRGrabber>();
            if (grabber && grabber.enabled)
            {
                grabber.enabled = false;
            }

            m_anchorOffsetPosition = transform.localPosition;
            m_anchorOffsetRotation = transform.localRotation;

            //OVRCameraRig rig = null;
            //if (transform.parent != null && transform.parent.parent != null)
            //rig = transform.parent.parent.parent.GetComponent<OVRCameraRig>();
            OVRCameraRig rig = transform.GetComponentInParent<OVRCameraRig>();

            if (rig != null)
            {
                rig.UpdatedAnchors += (r) => { OnUpdatedAnchors(); };
                operatingWithoutOVRCameraRig = false;
            }

        }
        protected void Start()
        {
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;
            if (m_parentTransform == null)
            {
                m_parentTransform = gameObject.transform;
                /*
                if (gameObject.transform.parent != null)
                {
                    m_parentTransform = gameObject.transform.parent.transform;
                }
                else
                {
                    m_parentTransform = new GameObject().transform;
                    m_parentTransform.position = Vector3.zero;
                    m_parentTransform.rotation = Quaternion.identity;
                }
                */
            }
            firstUpdate = true;
        }


        void FixedUpdate()
        {
            if (firstUpdate)
            {
                FirstGrabInitialize();
                firstUpdate = false;
            }
            else
            {
                FirstGrabFinalize();
            }
            if (operatingWithoutOVRCameraRig)
                OnUpdatedAnchors();
        }


        void OnUpdatedAnchors()
        {
            //Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
            //Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
            //Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + handPos);
            //Quaternion destRot = m_parentTransform.rotation * handRot * m_anchorOffsetRotation;
            Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
            Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;
            GetComponent<Rigidbody>().MovePosition(destPos);
            GetComponent<Rigidbody>().MoveRotation(destRot);

            if (!m_parentHeldObject)
            {
                MoveGrabbedObject(destPos, destRot);
            }
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            float prevFlex = m_prevFlex;
            m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

            CheckForGrabOrRelease(prevFlex);
        }



        void OnDestroy()
        {
            if (m_grabbedObj != null)
            {
                GrabEnd();
            }
        }



        protected void CheckForGrabOrRelease(float prevFlex)
        {
            if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
            {
                if (!firstTrigger)
                {
                    firstTrigger = true;
                }
            }
            else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd) && firstTrigger)
            {
                GrabEnd();
            }
        }



        protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
        {
            if (m_grabbedObj == null)
            {
                return;
            }

            Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
            Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
            Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

            if (forceTeleport)
            {
                grabbedRigidbody.transform.position = grabbablePosition;
                grabbedRigidbody.transform.rotation = grabbableRotation;
            }
            else
            {
                grabbedRigidbody.MovePosition(grabbablePosition);
                grabbedRigidbody.MoveRotation(grabbableRotation);
            }
        }


        protected void GrabBegin()
        {
            if (!firstGrabbable) return;

            m_grabbedObj = firstGrabbable;
            m_grabbedObj.GrabBegin(grabber, firstGrabbable.grabPoints[0]);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }


            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }


        }

        protected void GrabEnd()
        {
            if (m_grabbedObj != null)
            {
                OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
                OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
                localPose = localPose * offsetPose;

                OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
                Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
                Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

                GrabbableRelease(linearVelocity, angularVelocity);
            }

            FirstGrabFinalize();
        }

        protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
            if (m_parentHeldObject) m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;
        }


        protected void FirstGrabInitialize()
        {
            GrabBegin();
        }
        protected void FirstGrabFinalize()
        {
            handAnimator.runtimeAnimatorController = handAnimatorController;
            grabber.enabled = true;
            this.enabled = false;
        }

    }
}