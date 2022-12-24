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
        Camera _camera;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta">Positive delta zooms in (closer), negative zooms out (further).</param>
        public void Zoom( float delta )
        {
            _camera.transform.Translate( Vector3.forward * delta, Space.Self );
        }

        public void Focux( Transform obj )
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
                Zoom( -150.0f * Time.deltaTime );
            }
            else if( Input.mouseScrollDelta.y > 0 )
            {
                Zoom( 150.0f * Time.deltaTime );
            }

            if( Input.GetKey( KeyCode.A ) )
            {
                _yawPivot.Rotate( 0.0f, -45.0f * Time.deltaTime, 0.0f, Space.Self );
            }
            else if( Input.GetKey( KeyCode.D ) )
            {
                _yawPivot.Rotate( 0.0f, 45.0f * Time.deltaTime, 0.0f, Space.Self );
            }

            if( Input.GetKey( KeyCode.W ) )
            {
                _pitchPivot.Rotate( -45.0f * Time.deltaTime, 0.0f, 0.0f, Space.Self );
            }
            else if( Input.GetKey( KeyCode.S ) )
            {
                _pitchPivot.Rotate( 45.0f * Time.deltaTime, 0.0f, 0.0f, Space.Self );
            }
        }
    }
}