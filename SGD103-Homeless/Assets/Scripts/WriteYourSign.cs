﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class WriteYourSign : MonoBehaviour {
    private Texture2D canvas;
    private Vector2 lastPoint = new Vector2();
    private bool hasLastPoint = false;

    public int CanvasWidthPixels = 256;
    public int CanvasHeightPixels = 128;
    public Color PenColour = Color.black;
    public float PixelSpacing = 1.0f;

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
    
    // Draw the brush at the given point.
    void drawBrush (Vector2 point, Rect drawAreaRect, Color[] drawAreaPixels)
    {
        Vector2 pointInArea = point - drawAreaRect.position;
        if ((int)pointInArea.x >= 0 &&
            (int)pointInArea.y >= 0 &&
            (int)pointInArea.x < (int)drawAreaRect.width &&
            (int)pointInArea.y < (int)drawAreaRect.height)
        {
            drawAreaPixels[(int)pointInArea.y * (int)drawAreaRect.width + (int)pointInArea.x] = PenColour;
        }
    }

    void Update ()
    {
        // Draw on the canvas if the mouse is down.
        if (Input.GetButton("Primary"))
        {
            // Find cursor position on canvas.
            Vector2 cursorPosition;
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
                cursorPosition = new Vector2(
                    localPoint.x / GetComponent<RectTransform>().rect.width * CanvasWidthPixels,
                    localPoint.y / GetComponent<RectTransform>().rect.height * CanvasHeightPixels);
            }
            
            // Capture the pixel area that we'll draw into.
            Rect drawAreaRect;
            Color[] drawAreaPixels;
            {
                // Capture rectangle including the last point so we can fill in the gap between them.
                if (hasLastPoint)
                {
                    Vector2 firstCorner = cursorPosition;
                    Vector2 secondCorner = lastPoint;
                    bool flipX = (secondCorner.x < firstCorner.x);
                    bool flipY = (secondCorner.y < firstCorner.y);
                    if (flipX)
                    {
                        float x = firstCorner.x;
                        firstCorner.x = secondCorner.x;
                        secondCorner.x = x;
                    }
                    if (flipY)
                    {
                        float y = firstCorner.y;
                        firstCorner.y = secondCorner.y;
                        secondCorner.y = y;
                    }
                    drawAreaRect = new Rect(firstCorner, secondCorner - firstCorner + new Vector2(1, 1));
                    if (drawAreaRect.width < 1.0f)
                    {
                        drawAreaRect.width = 1.0f;
                    }
                    if (drawAreaRect.height < 1.0f)
                    {
                        drawAreaRect.height = 1.0f;
                    }
                }
                else
                {
                    drawAreaRect = new Rect(cursorPosition, new Vector2(1, 1));
                }

                // Restrict draw area rectangle to canvas borders.
                drawAreaRect.x = Mathf.Clamp(drawAreaRect.x, 0, CanvasWidthPixels - 1);
                drawAreaRect.y = Mathf.Clamp(drawAreaRect.y, 0, CanvasHeightPixels - 1);
                if (drawAreaRect.x + drawAreaRect.width > CanvasWidthPixels)
                {
                    drawAreaRect.width = CanvasWidthPixels - drawAreaRect.x;
                }
                if (drawAreaRect.y + drawAreaRect.height > CanvasHeightPixels)
                {
                    drawAreaRect.height = CanvasHeightPixels - drawAreaRect.y;
                }

                // Get pixels for the area.
                drawAreaPixels = canvas.GetPixels(
                    Mathf.FloorToInt(drawAreaRect.x),
                    Mathf.FloorToInt(drawAreaRect.y),
                    Mathf.FloorToInt(drawAreaRect.width),
                    Mathf.FloorToInt(drawAreaRect.height));
            }

            // Paint the canvas at the cursor.
            drawBrush(cursorPosition, drawAreaRect, drawAreaPixels);

            // Also draw strokes to fill the space between the last point and this point.
            Vector2 delta = cursorPosition - lastPoint;
            if (hasLastPoint)
            {
                int nPointsToAdd = Mathf.CeilToInt(delta.magnitude / PixelSpacing);
                for (var i = 0; i < nPointsToAdd; i++)
                {
                    drawBrush(lastPoint + i * delta.normalized * PixelSpacing, drawAreaRect, drawAreaPixels);
                }
            }

            // Apply changes to the draw area and the texture.
            canvas.SetPixels(
                Mathf.FloorToInt(drawAreaRect.x),
                Mathf.FloorToInt(drawAreaRect.y),
                Mathf.FloorToInt(drawAreaRect.width),
                Mathf.FloorToInt(drawAreaRect.height),
                drawAreaPixels);
            canvas.Apply();
            lastPoint = cursorPosition;
            hasLastPoint = true;
        }
        else
        {
            hasLastPoint = false;
        }
	}
}
