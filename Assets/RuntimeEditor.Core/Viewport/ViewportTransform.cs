using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// A move/rotate/scale transform on a specified target object.
    /// </summary>
    public class ViewportTransform : MonoBehaviour
    {
        public enum TransformMode : byte
        {
            /// <summary>
            /// Show translation handles.
            /// </summary>
            Translate,
            /// <summary>
            /// Show rotation handles.
            /// </summary>
            Rotate,
            /// <summary>
            /// Show scale handles.
            /// </summary>
            Scale,
        }

        /// <summary>
        /// The coordinate system of the transformation.
        /// </summary>
        public enum Space : byte
        {
            /// <summary>
            /// World space.
            /// </summary>
            World,
            /// <summary>
            /// Local space of the target.
            /// </summary>
            Local,
            /// <summary>
            /// Local space of the camera.
            /// </summary>
            View
        }

        [field: SerializeField]
        public GameObject Target { get; set; }

        /// <summary>
        /// The coordinate system that the transform works in.
        /// </summary>
        [field: SerializeField]
        public Space CoordinateSystem { get; set; }

        /// <summary>
        /// Determines which arrows to spawn.
        /// </summary>
        [field: SerializeField]
        public TransformMode Mode { get; set; }

        [SerializeField]
        List<ViewportTransformHandle> _handles;

        [SerializeField]
        Camera _alwaysOnTopCamera;

        bool _isTransforming = false;

        // specify different pivots
        // specify different coordinate frames.

        // managed the arrows/planes generated for an object, updates them every frame to reflect the camera orientation.

        // manage the size of the arrows based on the camera's proximity (inverse distance) - Angular diameter [deg] = (actual diameter [m] * 57.29 [radToDeg]) / distance [m].
        // apparently `return Mathf.Atan(diameter / distance);` 
        // it should parent the arrows to an empty, scale that empty based on whatever.
        // also, arrows should completely disregard scale when applying transformations.


        // should be rendered on a separate layer. (gizmo layer)

        public void SetTarget( object target )
        {
            if( target == null )
            {
                this.Target = null;

                ClearHandles();
                return;
            }
            if( target is GameObject go )
            {
                this.Target = go;

                ClearHandles();
                SpawnHandles();
                return;
            }

            throw new System.ArgumentException( "ViewportTransform's Target must be a GameObject" );
        }

        void ClearHandles()
        {
            foreach( ViewportTransformHandle x in this._handles )
            {
                Destroy( x.gameObject );
            }

            this._handles = new List<ViewportTransformHandle>();
        }

        void SpawnHandles()
        {
            switch( Mode )
            {
                case TransformMode.Translate:
                    SpawnHandles( ViewportTransformHandle.TransformType.Translate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f), Quaternion.Euler( 0.0f, 90.0f, 0.0f ), ViewportTransformHandle.TransformAxis.X );
                    SpawnHandles( ViewportTransformHandle.TransformType.Translate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( -90.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Y );
                    SpawnHandles( ViewportTransformHandle.TransformType.Translate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( 0.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Z );

                    SpawnHandles( ViewportTransformHandle.TransformType.Translate2D, Target, new Vector3( 0.0f, 0.5f, 0.5f ), Quaternion.Euler( 0.0f, 90.0f, 90.0f ), ViewportTransformHandle.TransformAxis.YZ );
                    SpawnHandles( ViewportTransformHandle.TransformType.Translate2D, Target, new Vector3( 0.5f, 0.0f, 0.5f ), Quaternion.Euler( -90.0f, -90.0f, 0.0f ), ViewportTransformHandle.TransformAxis.XZ );
                    SpawnHandles( ViewportTransformHandle.TransformType.Translate2D, Target, new Vector3( 0.5f, 0.5f, 0.0f ), Quaternion.Euler( 0.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.XY );
                    break;

                case TransformMode.Rotate:
                    SpawnHandles( ViewportTransformHandle.TransformType.Rotate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( 0.0f, 90.0f, 0.0f ), ViewportTransformHandle.TransformAxis.X );
                    SpawnHandles( ViewportTransformHandle.TransformType.Rotate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( -90.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Y );
                    SpawnHandles( ViewportTransformHandle.TransformType.Rotate1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( 0.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Z );
                    break;

                case TransformMode.Scale:
                    SpawnHandles( ViewportTransformHandle.TransformType.Scale1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( 0.0f, 90.0f, 0.0f ), ViewportTransformHandle.TransformAxis.X );
                    SpawnHandles( ViewportTransformHandle.TransformType.Scale1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( -90.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Y );
                    SpawnHandles( ViewportTransformHandle.TransformType.Scale1D, Target, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.Euler( 0.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.Z );

                    SpawnHandles( ViewportTransformHandle.TransformType.Scale2D, Target, new Vector3( 0.0f, 0.5f, 0.5f ), Quaternion.Euler( 0.0f, 90.0f, 90.0f ), ViewportTransformHandle.TransformAxis.YZ );
                    SpawnHandles( ViewportTransformHandle.TransformType.Scale2D, Target, new Vector3( 0.5f, 0.0f, 0.5f ), Quaternion.Euler( -90.0f, -90.0f, 0.0f ), ViewportTransformHandle.TransformAxis.XZ );
                    SpawnHandles( ViewportTransformHandle.TransformType.Scale2D, Target, new Vector3( 0.5f, 0.5f, 0.0f ), Quaternion.Euler( 0.0f, 0.0f, 0.0f ), ViewportTransformHandle.TransformAxis.XY );
                    break;
            }
        }

        void SpawnHandles( ViewportTransformHandle.TransformType type, GameObject obj, Vector3 localPosition, Quaternion localRotation, ViewportTransformHandle.TransformAxis transformAxis )
        {
            // spawns the handles of certain type in a specific arrangement.
            // handles operate on a specific object.

            ViewportTransformHandle handle =
                ViewportTransformHandle.Create( _alwaysOnTopCamera, Target, type, this.transform, localPosition, localRotation, transformAxis );

            this._handles.Add( handle );
        }

        private void Awake()
        {
            ClearHandles();

            if( Target == null )
            {
                return;
            }

            SpawnHandles();
        }

        void UpdateTransformation()
        {
            foreach( var handle in _handles )
            {
                if( handle.IsHeld )
                {
                    _isTransforming = true;
                    this.transform.position += handle.TranslationDeltaLastFrame;
                    if( CoordinateSystem == Space.Local )
                    {
                        this.transform.rotation *= handle.RotationDeltaLastFrame;
                    }

                    return;
                }
            }
            _isTransforming = false;
        }

        public static float GetScaleToFixSize( Transform transform, Camera camera, float fixedSize )
        {
            float distance = (camera.transform.position - transform.position).magnitude;
            float size = distance * fixedSize * camera.fieldOfView;

            return size;
        }

        void Update()
        {
            if( Target == null )
            {
                ClearHandles();
                return;
            }

            UpdateTransformation();
        }

        void LateUpdate()
        {
            if( Target == null )
            {
                return;
            }

            // If something moves the object - adjust the handles, unless a handle is transforming, in which case, leave it.
            if( !_isTransforming )
            {
                this.transform.SetPositionAndRotation( Target.transform.position, Target.transform.rotation );

                float size = GetScaleToFixSize( this.transform, _alwaysOnTopCamera, 0.0015f );
                this.transform.localScale = new Vector3( size, size, size );
            }
        }
    }
}