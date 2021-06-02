using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UISystem.Runtime
{
	[ExecuteAlways]
	[RequireComponent (typeof (UIObject)), DisallowMultipleComponent]
	public class ImprovedCanvasGroup : MonoBehaviour
	{
		public enum GroupBehaviour
		{
			Toggle,
			Single
		}
		
		//Default view will be active on start
		[Space, SerializeField,
		 Tooltip ("Default view will be visible on start.\nIf startView is null no view will be visible.")]
		private UIObject startView = null;

		[SerializeField] private List <UIObject> canvasGroupChildren = new List <UIObject> ();


		[Tooltip ("Toggle: Multiple canvas' can be active simultaneously\nSingle: Only one canvas can be active")]
		public GroupBehaviour behaviour;

		private void Reset ()
		{
			//Automatically check all children and add them to group unless child prevents grouping 
			SearchUIObjectsInChildren ();
		}

#if UNITY_EDITOR

		private void OnEnable ()
		{
			//Everytime hierarchy is changed check if children count on gameobject has changed
			EditorApplication.hierarchyChanged += OnChildrenCountChange;
		}

		private void OnDisable ()
		{
			//To prevent memory leak
			EditorApplication.hierarchyChanged -= OnChildrenCountChange;
		}
#endif

		private void OnDestroy ()
		{
			foreach (var child in canvasGroupChildren)
			{
				if (!child.IgnoreGroupBehaviour)
					continue;

				//To prevent memory leaks
				child.StateChanged -= OnUIViewVisibilityChanged;
			}
		}

		private void Start ()
		{
			if (!Application.isPlaying)
				return;


			if (canvasGroupChildren == null || canvasGroupChildren.Count == 0)
				return;


			//Hide all children that have UIContainer component unless child prevents group actions
			foreach (var child in canvasGroupChildren)
			{
				if (child.IgnoreGroupBehaviour)
					continue;

				child.Internal_SetUIEnabled (false, false);
				child.StateChanged += OnUIViewVisibilityChanged;
			}
			if(startView == null)
			{
				throw new Exception("Start view is null! You need to assing it in inspector.");
			}
			startView.Internal_SetUIEnabled(true,false);
		}

		/// <summary>
		/// Depending how canvas group works (toggle, single), set the visibility of  UIObject passed as parameter
		/// </summary>
		/// <param name="uiView"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private void OnUIViewVisibilityChanged (UIObject uiView)
		{
			switch (behaviour)
			{
				case GroupBehaviour.Toggle:
					uiView.Internal_SetUIEnabled (uiView.IsActive, false);

					break;
				case GroupBehaviour.Single:

					if (!Application.isPlaying)
						return;

					foreach (var child in canvasGroupChildren)
					{

						if (child != uiView)
							child.Internal_SetUIEnabled (false, false);
					}

					uiView.Internal_SetUIEnabled (true, false);

					break;
				default:
					throw new ArgumentOutOfRangeException ();
			}
		}


		private void ResizeGroupList (UIObject uiObject)
		{
			if (transform.childCount <= 0)
			{
				Debug.LogError ("Rootcanvas has no children", gameObject);
				return;
			}

			if (canvasGroupChildren.Contains (uiObject))
			{
				return;
			}

			canvasGroupChildren.Add (uiObject);
		}

		/// <summary>
		/// Add UIObject children to list
		/// </summary>
		/// <param name="child"></param>
		public void RegisterChild (UIObject child)
		{
			ResizeGroupList (child);
		}


		#region EditorMethods

		/// <summary>
		/// Automatically search UIObject component from children
		/// </summary>
		public void SearchUIObjectsInChildren ()
		{
			if (transform.childCount <= 0)
				return;

			var tempList = canvasGroupChildren;

			tempList.Clear ();
			//Automatically add all children that have UIContainer and won't ignore grouping
			for (var i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild (i);

				if (!child.GetComponent <UIObject> ())
					continue;

				if (child.GetComponent <UIObject> ().IgnoreGroupBehaviour)
					continue;

				tempList.Add (child.GetComponent <UIObject> ());
			}

			canvasGroupChildren = tempList;

			//If game object has no children that have UIContainer component make warning about it
			if (transform.childCount <= 0)
			{
				Debug.LogWarning ("Canvas group has no children that have UIContainer component");
			}
		}

		/// <summary>
		/// If game object's children count is changed it may result to null indexes. Search them and delete.
		/// </summary>
		private void RemoveNulls ()
		{
			for (var i = 0; i < canvasGroupChildren.Count; i++)
			{
				var c = canvasGroupChildren [i];

				if (c == null)
				{
					canvasGroupChildren.RemoveAt (i);
				}
			}
		}

		public void OnChildrenCountChange ()
		{
			SearchUIObjectsInChildren ();
			RemoveNulls ();
		}

		#endregion
	}
}