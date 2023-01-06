using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    public class ViewportCameraController : MonoBehaviour
    {
        [SerializeField]
        Transform _yawPivot;

        [SerializeField]
        Transform _pitchPivot;

        [SerializeField]
        Camera[] _cameras;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta">Positive delta zooms in (closer), negative zooms out (further).</param>
        public void Zoom( float delta )
        {
            foreach( var camera in _cameras )
            {
                camera.transform.Translate( Vector3.forward * delta, UnityEngine.Space.Self );
            }
        }

        public void Focus( Transform obj )
        {
            Focus( obj.position );
        }

        public void Focus( Vector3 position )
        {
            this._yawPivot.position = position;
        }

        void Start()
        {

        }

        void Update()
        {
            if( Input.mouseScrollDelta.y < 0 )
            {
                Zoom( 50.0f * Time.deltaTime * _cameras[0].transform.localPosition.z );
            }
            else if( Input.mouseScrollDelta.y > 0 )
            {
                Zoom( -50.0f * Time.deltaTime * _cameras[0].transform.localPosition.z );
            }

            if( Input.GetKey( KeyCode.A ) )
            {
                this._yawPivot.Rotate( 0.0f, -45.0f * Time.deltaTime, 0.0f, UnityEngine.Space.Self );
            }
            else if( Input.GetKey( KeyCode.D ) )
            {
                this._yawPivot.Rotate( 0.0f, 45.0f * Time.deltaTime, 0.0f, UnityEngine.Space.Self );
            }

            if( Input.GetKey( KeyCode.W ) )
            {
                this._pitchPivot.Rotate( -45.0f * Time.deltaTime, 0.0f, 0.0f, UnityEngine.Space.Self );
            }
            else if( Input.GetKey( KeyCode.S ) )
            {
                this._pitchPivot.Rotate( 45.0f * Time.deltaTime, 0.0f, 0.0f, UnityEngine.Space.Self );
            }
        }
    }
}