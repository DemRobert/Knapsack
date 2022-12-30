using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolverTable
{
	public int CountRows;
	public int CountColumns;

	public int StartX;
	public int StartY;

	public int EndX;
	public int EndY;

	public int Width;
	public int Height;

	public int WidthCell;
	public int HeightCell;

	public SolverTable(int countRows, int countColumns, int startX, int startY, int endX, int endY)
	{
		CountRows = countRows;
		CountColumns = countColumns;

		StartX = startX;
		StartY = startY;

		EndX = endX;
		EndY = endY;

		Width = endX - startX;
		Height = endY - startY;

		WidthCell = (int)(Width / (float)countColumns);
		HeightCell = (int)(Height / (float)CountRows);
	}

	public void Render(Texture2D texture, Color lineColor)
	{
		var vertX = StartX + WidthCell;
		for (var i = 0; i < (CountColumns-1); ++i)
		{
			DrawLine(texture, lineColor, vertX, StartY, vertX, EndY);
			vertX += WidthCell;
		}

		var horiY = StartY + HeightCell;
		for (var i = 0; i < (CountRows-1); ++i)
		{
			DrawLine(texture, lineColor, StartX, horiY, EndX, horiY);
			horiY += HeightCell;
		}
	}

	// Only straight Lines can be drawn
	private void DrawLine(Texture2D texture, Color lineColor, int lineStartX, int lineStartY, int lineEndX, int lineEndY)
	{
		// Vertical Line
		if (lineStartX == lineEndX)
		{
			var startY = Mathf.Min(lineStartY, lineEndY);
			var endY = Mathf.Max(lineStartY, lineEndY);

			for (var y = startY; y <= endY; ++y)
			{
				texture.SetPixel(lineStartX, y, lineColor);
			}
		}
		// Horizontal Line
		else if (lineStartY == lineEndY)
		{
			var startX = Mathf.Min(lineStartX, lineEndX);
			var endX = Mathf.Max(lineStartX, lineEndX);

			for (var x = startX; x <= endX; ++x)
			{
				texture.SetPixel(x, lineStartY, lineColor);
			}
		}
		// Neither Vertical nor Horizontal -> invalid
		else
		{
			Debug.Log("DrawLine: Attempted to Draw non-straight Line.");
		}
	}
}