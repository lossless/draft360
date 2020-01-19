using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHands : MonoBehaviour
{
    [SerializeField] SpriteRenderer openHand;
    [SerializeField] SpriteRenderer grabbedHand;

    void Start()
    {
        ShowOpenHand();   
    }

    private void OnEnable()
    {
        GameManager.GrabbedCallback += ShowGrabbedHand;
        GameManager.UngrabbedCallback += ShowOpenHand;
    }

    private void OnDisable()
    {
        GameManager.GrabbedCallback -= ShowGrabbedHand;
        GameManager.UngrabbedCallback -= ShowOpenHand;
    }

    private void ShowOpenHand()
    {
        openHand.enabled = true;
        grabbedHand.enabled = false;
    }

    private void ShowGrabbedHand()
    {
        openHand.enabled = false;
        grabbedHand.enabled = true;
    }
}
