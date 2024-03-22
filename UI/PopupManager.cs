// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public Transform popup;
    public Transform canvas;

    public void InstantiatePopup(string contents)
    {
        Popup p = Instantiate(popup, canvas).gameObject.GetComponent<Popup>();
        p.contents = contents;

        StartCoroutine(deletePopup(p.transform));
    }

    IEnumerator deletePopup(Transform p)
    {
        yield return new WaitForSeconds(3);
        Destroy(p.gameObject);
    }
}
