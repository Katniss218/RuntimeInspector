#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor( typeof( NoDragScrollRect ) )]
public class NoDragScrollRectEditor : ScrollRectEditor { }
#endif