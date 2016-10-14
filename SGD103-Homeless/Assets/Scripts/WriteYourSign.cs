using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WriteYourSign : MonoBehaviour {
    private Texture2D canvas;
    private Vector2 lastPoint = new Vector2();
    private bool hasLastPoint = false;

    public int CanvasWidthPixels = 256;
    public int CanvasHeightPixels = 128;
    public Color PenColour = Color.black;

	void Start ()
    {
        // Create and assign the texture.
        canvas = new Texture2D(CanvasWidthPixels, CanvasHeightPixels, TextureFormat.ARGB32, false);
        GetComponent<RawImage>().texture = canvas;

        // Fill with transparent black.
        Color fillColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        var pixelArray = canvas.GetPixels();
        for (var i = 0; i < pixelArray.Length; ++i)
        {
            pixelArray[i] = fillColour;
        }
        canvas.SetPixels(pixelArray);
        canvas.Apply();
    }
	
	void Update ()
    {
        // Draw on the canvas if the mouse is down.
        if (Input.GetButton("Primary"))
        {
            // Find current position.
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
            int x = (int)(localPoint.x / GetComponent<RectTransform>().rect.width * CanvasWidthPixels);
            int y = (int)(localPoint.y / GetComponent<RectTransform>().rect.height * CanvasHeightPixels);

            // Colour the canvas at that point.
            canvas.SetPixel(x, y, PenColour);

            // Also draw points at spaces between the last point and this point.
            if (hasLastPoint)
            {
                Vector2 delta = new Vector2(x - lastPoint.x, y - lastPoint.y);
                int nPointsToAdd = Mathf.CeilToInt(delta.magnitude);
                for (var i = 0; i < nPointsToAdd; i++)
                {
                    Vector2 space = new Vector2(
                        Mathf.FloorToInt(lastPoint.x + i * delta.normalized.x),
                        Mathf.FloorToInt(lastPoint.y + i * delta.normalized.y));
                    if (space.x < CanvasWidthPixels && space.y < CanvasHeightPixels && space.x > 0 && space.y > 0)
                    {
                        canvas.SetPixel((int)space.x, (int)space.y, PenColour);
                    }
                }
            }

            // Apply the changes to the texture.
            canvas.Apply();
            lastPoint.x = x;
            lastPoint.y = y;
            hasLastPoint = true;
        }
        else
        {
            hasLastPoint = false;
        }
	}
}
