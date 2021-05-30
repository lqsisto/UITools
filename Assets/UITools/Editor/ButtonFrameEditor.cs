using System;
using Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Editor;

namespace Editor
{
    [CustomEditor (typeof (ButtonFrame))]
    public class ButtonFrameEditor : UnityEditor.Editor
    {
        private SerializedProperty buttonAmount;
        private ButtonFrame buttonFrame;


        private void OnEnable ()
        {
            buttonAmount = serializedObject.FindProperty ("buttonAmountOnFrame");
        }

        public override void OnInspectorGUI ()
        {
            buttonFrame = (ButtonFrame)target;

            serializedObject.Update ();
            

            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("-"))
            {
                if (buttonAmount.intValue > 0)
                {
                    buttonAmount.intValue--;
                    serializedObject.ApplyModifiedProperties ();
                    buttonFrame.InstantiateButtons ();
                }
                
                
            }

            EditorGUILayout.PropertyField (buttonAmount, GUIContent.none);

            if (GUILayout.Button ("+"))
            {
                buttonAmount.intValue++;
                serializedObject.ApplyModifiedProperties ();
                buttonFrame.InstantiateButtons ();
            }

            
            EditorGUILayout.EndHorizontal ();


            var btnFrameGo = buttonFrame.gameObject;

            if(GUILayout.Button("Vertical Group"))
            {
                EditorGUI.BeginChangeCheck();
                if(!btnFrameGo.GetComponent<VerticalLayoutGroup>())
                {
                    if(btnFrameGo.GetComponent<HorizontalLayoutGroup>())
                    {
                        Undo.DestroyObjectImmediate(btnFrameGo.GetComponent<HorizontalLayoutGroup>());
                    }

                    Undo.AddComponent<VerticalLayoutGroup>(btnFrameGo);
                }
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.DestroyObjectImmediate(btnFrameGo.GetComponent<VerticalLayoutGroup>());
                    Undo.AddComponent<HorizontalLayoutGroup>(btnFrameGo);
                }
            }

            if(GUILayout.Button("Horizontal Group"))
            {
                EditorGUI.BeginChangeCheck();
                if(!btnFrameGo.GetComponent<HorizontalLayoutGroup>())
                {
                    if(btnFrameGo.GetComponent<VerticalLayoutGroup>())
                    {
                        Undo.DestroyObjectImmediate (btnFrameGo.GetComponent<VerticalLayoutGroup>());
                    }
                    Undo.AddComponent<HorizontalLayoutGroup>(btnFrameGo);
                }
                //if(EditorGUI.EndChangeCheck()){}
                EditorGUI.EndChangeCheck();
            }

            serializedObject.ApplyModifiedProperties ();
            EditorGUILayout.Space();
        }
    }
}