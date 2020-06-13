using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code snippet taken from a post on stackoverflow
// https://stackoverflow.com/questions/29406845/create-pulsing-color-in-unity
public class Pulse : MonoBehaviour
{
    public float FadeDuration = 1f;
    public Color Color1 = Color.yellow;
    public Color Color2 = Color.red;

    private Color startColor;
    private Color endColor;
    private float lastColorChangeTime;

    private Material material;
    
    
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        startColor = Color1;
        endColor = Color2;
    }

    // Update is called once per frame
    void Update()
    {
        var ratio = (Time.time - lastColorChangeTime) / FadeDuration;
        ratio = Mathf.Clamp01(ratio);
        material.color = Color.Lerp(startColor, endColor, ratio);
        //material.color = Color.Lerp(startColor, endColor, Mathf.Sqrt(ratio)); // A cool effect
        //material.color = Color.Lerp(startColor, endColor, ratio * ratio); // Another cool effect

        if (ratio == 1f)
        {
            lastColorChangeTime = Time.time;

            // Switch colors
            var temp = startColor;
            startColor = endColor;
            endColor = temp;
        }
    }
}
