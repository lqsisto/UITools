using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class ButtonFrame : HorizontalOrVerticalLayoutGroup
    {
        [SerializeField] private int modifiedButtonAmount;
        [SerializeField] protected int currentButtonAmount;

        [SerializeField] private float verticalMargin = 100f;
        [SerializeField] private float horizontalMargin = 100f;

        //disable values is always null warning
#pragma warning disable CS0649 
        [SerializeField] private bool isVerticalLayoutGroup;
#pragma warning restore CS0649 

        [SerializeField] protected Button buttonPrefab;

        private RectTransform attachedRectTransform;

        public List<GameObject> instantiatedButtons = new List<GameObject>();


        protected override void Reset()
        {
            base.Reset();

            currentButtonAmount = transform.childCount;
            modifiedButtonAmount = currentButtonAmount;

            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    instantiatedButtons.Add(transform.GetChild(i).gameObject);
                }
            }

            attachedRectTransform = GetComponent<RectTransform>();

            m_ChildAlignment = TextAnchor.MiddleCenter;

            childControlWidth = false;
            childControlHeight = false;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.delayCall += SetRectSizeToMatchContentSize;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.delayCall -= SetRectSizeToMatchContentSize;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SetRectSizeToMatchContentSize();

            if (transform.parent == null)
            {
                throw new UnityException("Gameobject has no parent");
            }

            //TODO only check children that have button component
            currentButtonAmount = transform.childCount;

        }

        public void SetRectSizeToMatchContentSize()
        {
            Debug.Log("Set rect size");
            Internal_SetRectSizeToMatchContentSize();
        }

        private void Internal_SetRectSizeToMatchContentSize()
        {
            if (!attachedRectTransform)
            {
                return;
            }
            

            if (isVerticalLayoutGroup)
            {
                var height = 0f;
                var widest = 0f;

                for (int i = 0; i < transform.childCount; i++)
                {
                    var c = transform.GetChild(i);

                    if (c.GetComponent<RectTransform>())
                    {
                        var cr = c.GetComponent<RectTransform>();
                        height += cr.rect.height;

                        if (cr.rect.width > widest)
                        {
                            widest = cr.rect.width;
                        }
                    }
                }
                height += (m_Spacing * transform.childCount) + m_Padding.top + m_Padding.bottom;
                widest += m_Padding.left + m_Padding.right + verticalMargin;
                attachedRectTransform.sizeDelta = new Vector2(widest, height);
            }
            else
            {
                var width = 0f;
                var highest = 0f;

                for (int i = 0; i < transform.childCount; i++)
                {
                    var c = transform.GetChild(i);

                    if (c.GetComponent<RectTransform>())
                    {
                        var cr = c.GetComponent<RectTransform>();
                        width += cr.rect.width;

                        if (cr.rect.height > highest)
                        {
                            highest = cr.rect.height;
                        }
                    }
                }
                width += (m_Spacing * transform.childCount) + m_Padding.left + m_Padding.right;
                highest += m_Padding.top + m_Padding.bottom + horizontalMargin;
                attachedRectTransform.sizeDelta = new Vector2(width, highest);
            }
        }
        public virtual void InstantiateButtons()
        {
            if (modifiedButtonAmount == currentButtonAmount)
                return;

            //if button amount is less when checked last
            if (modifiedButtonAmount < currentButtonAmount)
            {
                StartCoroutine(Delete());
            }
            else
            {
                var buttonToInstantiate = modifiedButtonAmount - currentButtonAmount;

                if (buttonToInstantiate < 0)
                    buttonToInstantiate = 0;

                for (var i = 0; i < buttonToInstantiate; i++)
                {
                    var button = Instantiate(buttonPrefab, transform);
                    Undo.RegisterCreatedObjectUndo(button, "Created go");
                    button.name = "ButtonPrefab";
                    instantiatedButtons.Add(button.gameObject);
                }

                instantiatedButtons.RemoveAll(item => item == null);
                currentButtonAmount = modifiedButtonAmount;

                Internal_SetRectSizeToMatchContentSize();
            }
        }

        private IEnumerator Delete()
        {
            var buttonsToRemove = currentButtonAmount - modifiedButtonAmount;

            for (int i = instantiatedButtons.Count - 1; i > (instantiatedButtons.Count - 1) - buttonsToRemove; i--)
            {
                Undo.DestroyObjectImmediate(instantiatedButtons[i]);
            }


            instantiatedButtons.RemoveAll(item => item == null);
            currentButtonAmount = modifiedButtonAmount;
            Internal_SetRectSizeToMatchContentSize();
            yield return null;
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, isVerticalLayoutGroup);

        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, isVerticalLayoutGroup);

        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, isVerticalLayoutGroup);

        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, isVerticalLayoutGroup);

        }
    }
}
