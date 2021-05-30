using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Runtime
{    
    [ExecuteAlways]
    [RequireComponent (typeof (Canvas), typeof (GraphicRaycaster), typeof (CanvasRenderer)), DisallowMultipleComponent]
    public class UIObject : MonoBehaviour
    {
        
        //Start visibility enum
        private enum VisibilityBehaviour
        {
            Visible,
            Hidden
        }
        
        private Canvas attachedCanvas;

        //If parent has ImprovedCanvasGroup component setting this true will ignore group behaviour
        [SerializeField] private bool ignoreGroupBehaviour = false;
        public bool IgnoreGroupBehaviour => ignoreGroupBehaviour;

        //set canvas start visibility in editor
        [SerializeField] private VisibilityBehaviour visibility;

        //canvas components for setting good default values :)
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;

        //bool to make sure that canvas visibility is not modified multiple times in single frame
        public bool IsBeingModified { get; private set; }

        //is attachedCanvas enabled
        public bool IsActive { get; private set; }

        //Action that is invoked when uiObject visibility is changed
        public event Action <UIObject> StateChanged;


        //Set good start values here
        private void Reset ()
        {
            if (!transform.parent)
            {
                gameObject.AddComponent <CanvasScaler> ();
                canvasScaler = GetComponent <CanvasScaler> ();

                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2 (1080, 1920);
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
            else
            {
                if (transform.parent.GetComponent <ImprovedCanvasGroup> ())
                {
                    transform.parent.GetComponent <ImprovedCanvasGroup> ().OnChildrenCountChange ();
                }
            }

            attachedCanvas = GetComponent <Canvas> ();
            attachedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }


        private void Awake ()
        {
            //fetch components
            attachedCanvas = GetComponent <Canvas> ();
            graphicRaycaster = GetComponent <GraphicRaycaster> ();
            
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
#endif
                Internal_SetUIEnabled (visibility == VisibilityBehaviour.Visible, true);


            //if parent gameobject has ImprovedCanvasGroup component and ignoreGroupBehaviour bool is false
            //register this UIObject to parent canvasgroup
            if (!transform.parent) 
                return;
            
            var canvasGroup = transform.parent.GetComponent <ImprovedCanvasGroup> ();

            if (canvasGroup && !ignoreGroupBehaviour)
            {
                canvasGroup.RegisterChild (this);
            }
        }

        /// <summary>
        /// Set UIObject visibility
        /// </summary>
        /// <param name="enabled"></param>
        public void SetUIEnabled (bool enabled)
        {
            if (IsBeingModified)
                return;

            UpdateVisibility (VisibilityBehaviour.Visible);
            attachedCanvas.sortingOrder += 100;

            IsBeingModified = true;
            Internal_SetUIEnabled (enabled, true);
            IsBeingModified = false;
        }

        
        /// <summary>
        /// Toggle UIObject visibility
        /// </summary>
        public void ToggleVisibility ()
        {
            IsActive = !IsActive;

            Internal_SetUIEnabled (IsActive, true);
        }


        /// <summary>
        /// Set VisibilityBehaviour enum to match new visibility
        /// </summary>
        /// <param name="newVisibility"></param>
        private void UpdateVisibility (VisibilityBehaviour newVisibility)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif
            visibility = newVisibility;
        }
        
        /// <summary>
        /// Internal method that SetUIEnabled() calls
        /// </summary>
        /// <param name="state"></param>
        /// <param name="notify"></param>
        internal void Internal_SetUIEnabled (bool state, bool notify)
        {
            IsActive = state;

            attachedCanvas.enabled = state;

            graphicRaycaster.enabled = state;


            DirtyAttachedCanvas ();

            if (notify)
                StateChanged?.Invoke (this);
        }
        

        /// <summary>
        /// Toggle canvas gameobject on and off. Since it is not possible to force only one canvas to update,
        /// this is a hack to make sure that the canvas is dirtied and updated.
        /// </summary>
        private void DirtyAttachedCanvas ()
        {
            gameObject.SetActive (false);
            gameObject.SetActive (true);
        }
    }
}