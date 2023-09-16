using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Sample : MonoBehaviour
{
    public List<Button> onOffButtons;
    public List<Button> removeButtons;

    [SerializeField]
    InputField inputField;

    private void Start()
    {
        foreach (var onOffButton in onOffButtons)
        onOffButton.onClick.AddListener(() =>
        {
            bool isOn = !inputField.gameObject.activeInHierarchy;
            inputField.gameObject.SetActive(isOn);
        });

        foreach (var removeBtn in removeButtons)
            removeBtn.onClick.AddListener(() =>
            {
                InputField backup = inputField;
                inputField = Instantiate(backup, backup.transform.parent);
                inputField.transform.localPosition = backup.transform.localPosition;
                GameObject.Destroy(backup.gameObject);
            });
    }

    public void OnValueChange(InputField o)
    {
        Debug.Log(string.Format("Sample:OnValueChange[{1}] ({0})", o.text, o.name));
    }
    public void OnEndEdit(InputField o)
    {
        Debug.Log(string.Format("Sample:OnEndEdit[{1}] ({0})", o.text, o.name));
    }
}
