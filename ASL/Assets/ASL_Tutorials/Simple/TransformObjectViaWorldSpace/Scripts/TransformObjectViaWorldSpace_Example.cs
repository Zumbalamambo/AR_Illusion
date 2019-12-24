﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleDemos
{
    /// <summary>
    /// Example of how to perform the various Transforms (position, rotation, scale) on an object via world space.
    /// As well as some examples of what to do for additive style movement
    /// </summary>
    public class TransformObjectViaWorldSpace_Example : MonoBehaviour
    {
        /// <summary>Provides an easy way to access the object we want to perform transformations on. </summary>
        public GameObject m_ObjectToManipulate;
        /// <summary>Bool triggering if we should send a single transform or not. Will be set back to false after setting to true
        /// to prevent variables from being sent again</summary>
        public bool m_SendTransform = false;
        /// <summary>Bool triggering if we are doing additive transforms, such as building off of our previous location
        /// instead of just moving to a new position</summary>        
        public bool m_SendAdditiveTransform = false;
        /// <summary>Bool triggering if we should reset the variables and location of the cube</summary>
        public bool m_Reset = false;
        /// <summary>The position we want to move to</summary>
        public Vector3 m_MoveToPosition;
        /// <summary>The amount we want to move every Update cycle</summary>
        public Vector3 m_AdditiveMovementAmount;

        /// <summary>
        /// The rotation axis to be used when rotating our object. All of these values are generated by their respective positions in m_RotateToAmount and m_AdditiveRotateAmount
        /// The custom option uses all 4 (x, y, z and w) to create the rotation axis
        /// </summary>
        public enum RotationAxis
        {
            /// <summary>The x axis</summary>
            x,
            /// <summary>The y axis</summary>
            y,
            /// <summary>The z axis</summary>
            z,
            /// <summary> Use all 4 values of our quaternion to create our rotation axis</summary>
            custom
        };

        /// <summary>
        /// My axis of rotation for my object
        /// </summary>
        public RotationAxis m_MyRotationAxis;

        /// <summary>The custom axis you wish to perform a rotation on</summary>
        public Vector3 m_MyCustomAxis;

        /// <summary>The angle we want to rotate by. </summary>
        [Range(0.0f, 360.0f)]
        public float m_Angle;

        /// <summary>The amount we want to scale our object by</summary>
        public Vector3 m_ScaleToAmount;
        /// <summary> The amount we want to increase our scale by every Update cycle</summary>
        public Vector3 m_AdditiveScaleAmount;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            //Set scale values to 1 because you can't have a a scale of 0
            m_ScaleToAmount = Vector3.one;
            m_AdditiveScaleAmount = new Vector3(0.001f, 0.001f, 0.001f);
            m_Angle = 0;
        }

        /*For more examples go to:
        * https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/abf6ccf8-a07a-5800-7e1d-2d4888db95e1.htm Increment Position
		* https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/fc24d52f-311d-236f-7732-210591252bb3.htm Set Position
		* https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/ce8cfe06-1591-ce1f-9f2e-d3b26e3409a6.htm Increment Rotation
        * https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/da81c11d-988c-29c7-fe68-89e803aa2730.htm Set Rotation
		* https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/675dc901-c410-2dbb-0599-9476f3ee673c.htm Increment Scale
        * https://uwb-arsandbox.github.io/ASL_Master/ASLDocumentation/Help/html/d8ff15ef-baef-0017-48f8-33bb6800e66f.htm Set Scale
        */

        /// <summary>
        /// Logic of our demo
        /// </summary>
        void Update()
        {
            //"Lock" custom axis to 0 until user selects custom as an option
            if (m_MyRotationAxis != RotationAxis.custom)
            {
                m_MyCustomAxis = Vector3.zero;
            }

            if (m_SendAdditiveTransform)
            {
                //Add m_AdditiveMovementAmount to our current location to make it our new location
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndIncrementWorldPosition(m_AdditiveMovementAmount);
                });

                //SendAndIncrementLocalRotation is set by
                //doing *= which is how quaternions should be set. If in your demo it doesn't seem to be rotating properly
                //it's because quaternions don't work the way you think they do. Try adjusting the w value. 
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    Quaternion rotateAmount; //Used to rotate on specific axis
                    //Determine axis of rotation: - We don't adjust W so if you touch that, you're on your own...
                    if (m_MyRotationAxis == RotationAxis.x)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.right);
                    }
                    else if (m_MyRotationAxis == RotationAxis.y)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.up);
                    }
                    else if (m_MyRotationAxis == RotationAxis.z)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.forward);
                    }
                    else //Custom axis
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, m_MyCustomAxis.normalized);
                    }
                    //Send new rotation position
                    rotateAmount.Normalize(); //Normalize to prevent data overflow
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndIncrementWorldRotation(rotateAmount);                   
                });

                //Add m_AdditiveScaleAmount to our current scale to make it our new scale
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndIncrementWorldScale(m_AdditiveScaleAmount);
                });
                //m_SendAdditiveTransform = false;
            }
            if (m_SendTransform)
            {               
                //Just set position
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldPosition(m_MoveToPosition);
                });

                //Just set Rotation
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    Quaternion rotateAmount; //Used to rotate on specific axis
                    //Determine axis of rotation:
                    if (m_MyRotationAxis == RotationAxis.x)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.right);
                    }
                    else if (m_MyRotationAxis == RotationAxis.y)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.up);
                    }
                    else if (m_MyRotationAxis == RotationAxis.z)
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, Vector3.forward);
                    }
                    else //Custom axis
                    {
                        rotateAmount = Quaternion.AngleAxis(m_Angle, m_MyCustomAxis.normalized);
                    }
                    //Send new rotation position
                    rotateAmount.Normalize(); //Normalize to prevent data overflow
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldRotation(rotateAmount);
                });

                //Just set Scale
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldScale(m_ScaleToAmount);
                });

                //Set back to false so these values are only sent once
                m_SendTransform = false;
            }       
            //Reset all values to what they are on scene load and Move object back to original position and orientation
            if (m_Reset)
            {
                //Reset variables
                m_AdditiveMovementAmount = Vector3.zero;
                m_MoveToPosition = Vector3.zero;
                m_AdditiveScaleAmount = Vector3.one;
                m_ScaleToAmount = Vector3.one;
                m_MyCustomAxis = Vector3.zero;
                m_Angle = 0;
                
                m_Reset = false;
                m_SendTransform = false;
                m_SendAdditiveTransform = false;

                //Reset Position
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldPosition(m_MoveToPosition);
                });

                //Reset Rotation
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldRotation(Quaternion.identity);
                });

                //Reset Scale
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetWorldScale(m_ScaleToAmount);
                });

            }
        }

        
    }
}
