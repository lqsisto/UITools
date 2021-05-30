using UISystem.Runtime;
using UnityEditor;

namespace UISystem.Editor
{
    [CustomEditor (typeof (UIObject)), CanEditMultipleObjects]
    public class UIObjectEditor : UnityEditor.Editor
    {
        private UIObject container;

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();
            container = (UIObject) target;

            if (container.transform.parent &&
                container.transform.parent.GetComponent <ImprovedCanvasGroup> () &&
                !container.IgnoreGroupBehaviour)
            {
                EditorGUILayout.HelpBox (
                    "Parent component has canvas group in use. Visibility settings here will be ignored",
                    MessageType.Warning);
            }
        }
    }
}