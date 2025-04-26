using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ClampUIWithinSafeArea : MonoBehaviour
{
    private void Awake()
    {
        ClampUi();
    }
    private void Update()
    {
        ClampUi();//Need to be removevd while build
    }
    public void ClampUi()
    {
        var rectTransform = GetComponent<RectTransform>();

        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        /*if(Screen.width>600f)
        {
            *//* button jiski position change hogi
             Kidhr hoga adjust position
             Last Button Position X Axis 
             Wahi same X Axis value bbutton ko 
            increase Y Axis 150 *//*
        }*/
    }
}