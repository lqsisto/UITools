using System;
using UISystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace UISystem.Editor
{
    [CustomEditor (typeof (ImprovedCanvasGroup))]
    [CanEditMultipleObjects]
    public class ImprovedCanvasGroupEditor : UnityEditor.Editor
    {
        private ImprovedCanvasGroup improvedCanvasGroup;

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();
            improvedCanvasGroup = (ImprovedCanvasGroup) target;

            //DrawDefaultInspector ();

            if (GUILayout.Button("Find UIContainers in children"))
            {
                FindChildren ();
            }
        }

        private void FindChildren ()
        {
            improvedCanvasGroup.SearchUIObjectsInChildren ();
        }
    }
}