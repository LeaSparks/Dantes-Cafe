using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] private float _activeTime = 2f;
    [SerializeField] private float _heightOffset = 0f;
    [SerializeField] private float _heightTravelled = 1.5f;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowIndicatorAtWorldTarget(string str, Vector3 target)
    {
        gameObject.SetActive(true);

        _text.SetText(str);

        transform.position = target;//.position;
        
        if(GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace) 
            transform.rotation = Quaternion.LookRotation(gameObject.transform.position - Camera.main.transform.position);   //spacial UI needs to face camera
        else
            transform.position = Camera.main.WorldToScreenPoint(transform.position);            //non-spatial UI has to be converted to screen coordinates
        
        transform.Translate(0f, _heightOffset, 0f);

        transform.DOMoveY(transform.position.y + _heightTravelled, _activeTime).OnComplete(() => gameObject.SetActive(false));
    }

        public void ShowIndicatorAtCanvasTarget(string str, Vector3 target)
    {
        gameObject.SetActive(true);

        _text.SetText(str);

        transform.position = target;
        
        if(GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace)
        {
            transform.rotation = Quaternion.LookRotation(gameObject.transform.position - Camera.main.transform.position);   //spacial UI needs to face camera
            transform.position = Camera.main.ScreenToWorldPoint(target);
        } 
        
        transform.Translate(0f, _heightOffset, 0f);

        transform.DOMoveY(transform.position.y + _heightTravelled, _activeTime).OnComplete(() => gameObject.SetActive(false));
    }
}
