using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class UserDefineButton : MonoBehaviour, IPointerClickHandler
    {
        public Action<string> OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick(gameObject.name);
            }
        }
    }
}
