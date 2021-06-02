using System;
using Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Editor;
using UnityEditor.UI;

namespace Editor
{
    [CustomEditor(typeof(ButtonFrame))]
    public class ButtonFrameEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty modifiedButtonAmount;
        private SerializedProperty buttonPrefab;

        private SerializedProperty isVerticalLayoutGroup;

        private ButtonFrame buttonFrame;

        public override void OnInspectorGUI()
        {
            //Find values that are modified
            modifiedButtonAmount = serializedObject.FindProperty("modifiedButtonAmount");
            isVerticalLayoutGroup = serializedObject.FindProperty("isVerticalLayoutGroup");
            buttonPrefab = serializedObject.FindProperty("buttonPrefab");

            buttonFrame = (ButtonFrame)target;

            EditorGUILayout.PropertyField(buttonPrefab);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-"))
            {
                if (modifiedButtonAmount.intValue > 0)
                {
                    modifiedButtonAmount.intValue--;
                    serializedObject.ApplyModifiedProperties();
                    buttonFrame.InstantiateButtons();
                }


            }

            EditorGUILayout.PropertyField(modifiedButtonAmount, GUIContent.none);

            if (GUILayout.Button("+"))
            {
                modifiedButtonAmount.intValue++;
                serializedObject.ApplyModifiedProperties();
                buttonFrame.InstantiateButtons();
            }

            EditorGUI.EndChangeCheck();
            EditorGUILayout.EndHorizontal();


            var btnFrameGo = buttonFrame.gameObject;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Vertical Group", GUILayout.Height(50)))
            {
                EditorGUI.BeginChangeCheck();

                isVerticalLayoutGroup.boolValue = true;

                EditorGUI.EndChangeCheck();

            }

            if (GUILayout.Button("Horizontal Group", GUILayout.Height(50)))
            {
                EditorGUI.BeginChangeCheck();
                isVerticalLayoutGroup.boolValue = false;

                EditorGUI.EndChangeCheck();
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}