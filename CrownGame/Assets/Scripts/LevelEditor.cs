using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private Texture2D source;
    [SerializeField] private List<Pixel> pixels;

    private void Start()
    {
        loadSource();
    }

    private void loadSource()
    {
        for (int i = 0; i < source.width; i++)
        {
            for (int j = 0; j < source.height; j++)
            {
                Color color = source.GetPixel(i, j);
                //Debug.Log(color.a);
                if (color.a == 0)
                {
                    continue;
                }
                loadPixel(color, i, j);
            }
        }
    }

    private void loadPixel(Color color, int x, int y)
    {
        foreach (Pixel item in pixels)
        {
            if (item.color.r == color.r && item.color.g == color.g && item.color.b == color.b )
            {
                Instantiate(item.asset, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }
    }
}
[System.Serializable]
public class Pixel
{
    public Color color;
    public GameObject asset;
}