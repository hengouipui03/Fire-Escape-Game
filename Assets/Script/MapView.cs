using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapView : MonoBehaviour
{
    public Sprite[] mapImgs;
    public Text level;
    public Image map;
    int i = 0;

    private void Start()
    {
        level.text = map.sprite.name;
    }
    public void arrowRight()
    {
        if (i < mapImgs.Length - 1)
        {
            i++;
            level.text = mapImgs[i].name;
            map.sprite = mapImgs[i];
            map.SetNativeSize();
        }
    }

    public void arrowLeft()
    {
        if (i > 0)
        {
            i--;
            level.text = mapImgs[i].name;
            map.sprite = mapImgs[i];
            map.SetNativeSize();
        }
    }
}
