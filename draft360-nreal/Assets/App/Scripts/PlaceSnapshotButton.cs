using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceSnapshotButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region EVENTS

    public static event Action PressedDownCallback;
    public static event Action PressedUpCallback;

    protected void OnPressedDown()
    {
        if (PressedDownCallback != null)
        {
            PressedDownCallback?.Invoke();
        }
    }

    protected void OnPressedUp()
    {
        if (PressedUpCallback != null)
        {
            PressedUpCallback?.Invoke();
        }
    }

    #endregion

    public static PlaceSnapshotButton Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPressedUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPressedDown();
    }
}
