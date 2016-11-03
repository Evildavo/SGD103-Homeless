using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class WriteYourSign : MonoBehaviour
{
    private Vector2 lastPoint = new Vector2();
    private bool hasLastPoint = false;
    private Color[] PenTipPixels;
    private CardboardSign CardboardSign;
    private Begging begging;

    public Main Main;

    public int CanvasWidthPixels = 256;
    public int CanvasHeightPixels = 128;
    public RectTransform CanvasArea;
    public Color PenColour = Color.black;
    public float PixelSpacing = 1.0f;
    public Texture2D PenTipTexture;
    [ReadOnly]
    public Texture2D CanvasTexture;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return gameObject.activeInHierarchy;
    }

    public void ClearCanvas()
    {
        // Fill with transparent black.
        Color fillColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        var pixelArray = CanvasTexture.GetPixels();
        for (var i = 0; i < pixelArray.Length; ++i)
        {
            pixelArray[i] = fillColour;
        }
        CanvasTexture.SetPixels(pixelArray);
        CanvasTexture.Apply();

        // Apply to cardboard sign object.
        if (CardboardSign)
        {
            CardboardSign.CanvasMaterial.mainTexture = CanvasTexture;
            CardboardSign.CanvasMaterial.color = Color.white;
        }
    }

    public void BegButtonSelected()
    {
        if (begging)
        {
            begging.StartBegging();
        }
    }

	void Start ()
    {
        // Get the cardboard sign.
        CardboardSign = FindObjectOfType<CardboardSign>();

        // Get the begging spot.
        begging = Main.PlayerState.CurrentBeggingSpot;

        // Create and assign the texture.
        CanvasTexture = new Texture2D(CanvasWidthPixels, CanvasHeightPixels, TextureFormat.ARGB32, false);
        RawImage rawImage = GetComponentInChildren<RawImage>();
        rawImage.enabled = true;
        rawImage.texture = CanvasTexture;

        ClearCanvas();
    }
    
    // Draw the brush at the given point.
    void drawBrush (Vector2 point, Rect drawAreaRect, Color[] drawAreaPixels)
    {
        Vector2 centre = new Vector2(PenTipTexture.width / 2, PenTipTexture.height / 2);
        for (var i = 0; i < PenTipTexture.height; i++)
        {
            for (var j = 0; j < PenTipTexture.width; j++)
            {
                Vector2 pointInPenTipTexture = new Vector2(j, i);
                Vector2 pointInArea = point - centre + pointInPenTipTexture - drawAreaRect.position;
                if ((int)pointInArea.x >= 0 &&
                    (int)pointInArea.y >= 0 &&
                    (int)pointInArea.x < (int)drawAreaRect.width &&
                    (int)pointInArea.y < (int)drawAreaRect.height)
                {
                    int pointIndex = (int)pointInArea.y * (int)drawAreaRect.width + (int)pointInArea.x;
                    int penTipIndex = j * PenTipTexture.width + i;

                    // Apply the new colour using alpha-transparency blending. 
                    Color sourceColour = new Color(PenColour.r, PenColour.g, PenColour.b, PenTipPixels[penTipIndex].a);
                    Color destinationColour = drawAreaPixels[pointIndex];
                    drawAreaPixels[pointIndex] = new Color(
                        sourceColour.r * sourceColour.a + destinationColour.r * (1.0f - sourceColour.a),
                        sourceColour.g * sourceColour.a + destinationColour.g * (1.0f - sourceColour.a),
                        sourceColour.b * sourceColour.a + destinationColour.b * (1.0f - sourceColour.a),
                        sourceColour.a + destinationColour.a * (1.0f - sourceColour.a));
                }
            }
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
                    CanvasArea, Input.mousePosition, null, out localPoint);
                cursorPosition = new Vector2(
                    localPoint.x / CanvasArea.rect.width * CanvasWidthPixels,
                    localPoint.y / CanvasArea.rect.height * CanvasHeightPixels);
            }
            
            // Capture the pixel area that we'll draw into.
            Rect drawAreaRect;
            Color[] drawAreaPixels;
            Vector2 penTipSize = new Vector2(PenTipTexture.width, PenTipTexture.height);
            Vector2 penTipCentre = penTipSize / 2;
            {
                // Capture rectangle including the last point so we can fill in the gap between them.
                if (hasLastPoint)
                {
                    // Capture the area between two corners.
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

                    // Rectangle should have a minimum size of 1 pixel.
                    if (drawAreaRect.width < 1.0f)
                    {
                        drawAreaRect.width = 1.0f;
                    }
                    if (drawAreaRect.height < 1.0f)
                    {
                        drawAreaRect.height = 1.0f;
                    }

                    // Extend rectangle to fit the pen tip area.
                    drawAreaRect.xMin -= penTipCentre.x;
                    drawAreaRect.xMax += penTipCentre.x;
                    drawAreaRect.yMin -= penTipCentre.y;
                    drawAreaRect.yMax += penTipCentre.y;
                }
                else
                {
                    drawAreaRect = new Rect(cursorPosition - penTipCentre, penTipSize);
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
                drawAreaPixels = CanvasTexture.GetPixels(
                    Mathf.FloorToInt(drawAreaRect.x),
                    Mathf.FloorToInt(drawAreaRect.y),
                    Mathf.FloorToInt(drawAreaRect.width),
                    Mathf.FloorToInt(drawAreaRect.height));
            }

            // Get the pixels of the brush.
            PenTipPixels = PenTipTexture.GetPixels();

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
            CanvasTexture.SetPixels(
                Mathf.FloorToInt(drawAreaRect.x),
                Mathf.FloorToInt(drawAreaRect.y),
                Mathf.FloorToInt(drawAreaRect.width),
                Mathf.FloorToInt(drawAreaRect.height),
                drawAreaPixels);
            CanvasTexture.Apply();
            lastPoint = cursorPosition;
            hasLastPoint = true;

            // Apply to cardboard sign object.
            if (CardboardSign)
            {
                CardboardSign.CanvasMaterial.mainTexture = CanvasTexture;
                CardboardSign.CanvasMaterial.color = Color.white;
                CardboardSign.Show();
            }
        }
        else
        {
            hasLastPoint = false;
        }
	}
}
