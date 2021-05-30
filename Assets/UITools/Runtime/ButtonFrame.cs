using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class ButtonFrame : MonoBehaviour
    {
        [SerializeField] protected int buttonAmountOnFrame;
        [SerializeField] private int currentButtonAmount;
        
        
        public List<GameObject> instantiatedButtons = new List<GameObject> ();
        protected GameObject buttonPrefab;
        
        protected virtual void Reset()
        {
            buttonPrefab =
                (GameObject) AssetDatabase.LoadAssetAtPath (@"Assets\Resources/UICreator/ButtonPrefab.prefab",typeof (GameObject));

        }

        private void OnValidate ()
        {

            Debug.Log("ON VALIDATE");

            if (transform.parent == null)
            {
                Debug.LogError ("Component has no parent, button frame requires parent to work!");
            }
        }


        public virtual void InstantiateButtons ()
        {
            if (currentButtonAmount == buttonAmountOnFrame)
                return;

            if (currentButtonAmount > buttonAmountOnFrame)
            {
                StartCoroutine (Delete ());
            }
            else
            {
                var buttonToInstantiate = buttonAmountOnFrame - currentButtonAmount;

                if (buttonToInstantiate < 0)
                    buttonToInstantiate = 0;

                for (var i = 0; i < buttonToInstantiate; i++)
                {
                    var button = Instantiate (buttonPrefab, transform);
                    button.name = "ButtonPrefab";
                    instantiatedButtons.Add (button);
                }

                instantiatedButtons.RemoveAll (item => item == null);
                currentButtonAmount = buttonAmountOnFrame;
            }
        }
    
        private IEnumerator Delete ()
        {
            var buttonsToRemove = currentButtonAmount - buttonAmountOnFrame;

            for (int i = instantiatedButtons.Count-1; i > (instantiatedButtons.Count-1) - buttonsToRemove; i--)
            {
                DestroyImmediate (instantiatedButtons[i]);
                yield return null;
            }
            instantiatedButtons.RemoveAll (item => item == null);
            currentButtonAmount = buttonAmountOnFrame;
        }
    }
}
