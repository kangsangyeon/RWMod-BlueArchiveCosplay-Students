using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPopup: MonoBehaviour
{
    public GameObject popupRoot;
    public Text txtTitle, txtMessage;
    [Space]
    public Button btnOk;
    public Button btnCancle;
    public Text txtBtnOk, txtBtnCancel;

    public void Popup(string title, string message, UnityAction action, string cancle = "취소", string ok = "확인")
    {
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(action);
        txtTitle.text = title;
        txtMessage.text = message;

        txtBtnCancel.text = cancle;
        txtBtnOk.text = ok;

        popupRoot.SetActive(true);
    }

    public void Cancle()
    {
        popupRoot.SetActive(false);
    }
}