using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DynamicProgrammingSolver : MonoBehaviour
{
	public static DynamicProgrammingSolver Instance;

    public GameObject PlayerHUD;
    public GameObject BackgroundAndLines;

	public GameObject TextPrefab;

	private Texture2D m_Texture;
	private RawImage m_BackgroundAndLinesImageComp;

	// Konstant, da sich die Canvase(?) immer an der width orientieren
	private int m_ScreenWidth = 1920;
	private int m_ScreenHeight;

	private Color m_ClearColor = new(0.7f, 0.7f, 0.7f, 1.0f);
	private Color m_LineColor = new(0.05f, 0.05f, 0.05f, 1.0f);
	// Wie viel Prozent der gesamten Bildschirmbreite eine Tabelle einnehmen soll
	private float m_TableWidthPercentage = 0.8f;

	private DynamicProgAlgoBehaviour m_Algorithm;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		var rect = GetComponent<RectTransform>();
		m_ScreenHeight = (int)rect.rect.height;

		m_Texture = new(m_ScreenWidth, m_ScreenHeight);
	}

	private void ClearScreen()
	{
		for (var y = 0; y < m_Texture.height; ++y)
		{
			for (var x = 0; x < m_Texture.width; ++x)
			{
				m_Texture.SetPixel(x, y, m_ClearColor);
			}
		}
	}

	private void DrawTable(int countRows, int countColumns, int startX, int startY, int endX, int endY)
	{
		var tableWidth = endX - startX;
		var tableHeight = endY - startY;

		var widthColumn = tableWidth / (float)countColumns;
		var heightRow = tableHeight / (float)countRows;

		var vertX = (int)(startX + widthColumn);
		for (var i = 0; i < (countColumns-1); ++i)
		{
			DrawLine(vertX, startY, vertX, endY);
			vertX += (int)widthColumn;
		}

		var horiY = (int)(startY + heightRow);
		for (var i = 0; i < (countRows-1); ++i)
		{
			DrawLine(startX, horiY, endX, horiY);
			horiY += (int)heightRow;
		}
	}

	private void DrawMainTable()
	{
		var capacity = GetCapacity();
		var items = GetItems();

		var freeWidth = m_ScreenWidth * (1.0f - m_TableWidthPercentage);
		// Die Tabelle ist zentriert -> Links und Rechts gleich viel Freiraum
		var startX = (int)(freeWidth * 0.5f);
		var endX = (int)(m_ScreenWidth - freeWidth*0.5f);

		var startY = (int)(m_ScreenHeight * 0.05f);
		var endY = (int)(m_ScreenHeight * 0.6f);

		DrawTable(items.Length+1, capacity+1, startX, startY, endX, endY);
	}

	private void DrawItemTable()
	{
		var items = GetItems();

		var freeWidth = m_ScreenWidth * (1.0f - m_TableWidthPercentage);
		// Die Tabelle ist zentriert -> Links und Rechts gleich viel Freiraum
		var startX = (int)(freeWidth * 0.5f);
		var endX = (int)(m_ScreenWidth - freeWidth*0.5f);

		var startY = (int)(m_ScreenHeight * 0.65f);
		var endY = (int)(m_ScreenHeight * 0.95f);

		// Rows -> ItemIndex, Value, Weight
		DrawTable(3, items.Length+1, startX, startY, endX, endY);
	}

	// x,y | the Position of the Bottom Left-Hand Corner of the Text
	// width, height | the Expand of the Text towards the Upper Right
	private void DrawText(string text, int x, int y, int width, int height)
	{
		var instance = Instantiate(TextPrefab, new(), TextPrefab.transform.rotation, transform);
		instance.name = "Text";

		x += width/2;
		y += height/2;

		var rectTransform = instance.GetComponent<RectTransform>();
		rectTransform.localPosition = new(x, y);
		rectTransform.sizeDelta = new(width, height);

		var textComponent = instance.GetComponent<TextMeshProUGUI>();
		textComponent.text = text;
	}

	private int GetCapacity()
	{
		return PlayerHUDController.Instance.GetKnapsackCapacity();
	}

	private ItemProperties[] GetItems()
	{
		return m_Algorithm.SelectedItems;
	}

	public void Solve(DynamicProgAlgoBehaviour algorithm)
	{
		if (m_Algorithm == null)
		{
			m_Algorithm = algorithm;
		}

		// Disable the HUD Canvas but Enable the Background Image
		PlayerHUD.SetActive(false);
		BackgroundAndLines.SetActive(true);

		// Solving...
		ClearScreen();
		DrawMainTable();
		DrawItemTable();

		DrawText("Test", -m_ScreenWidth/2, -m_ScreenHeight/2, 100, 60);

		m_Texture.Apply();

		m_BackgroundAndLinesImageComp = BackgroundAndLines.GetComponent<RawImage>();
		m_BackgroundAndLinesImageComp.texture = m_Texture;

		// Solving done
		//BackgroundAndLines.SetActive(false);
		//PlayerHUD.SetActive(true);
	}

	private bool IsEqual(float a, float b)
	{
		return Mathf.Abs(a - b) <= 0.0001f;
	}

	// Only straight Lines can be drawn
	private void DrawLine(int lineStartX, int lineStartY, int lineEndX, int lineEndY)
	{
		// Vertical Line
		if (lineStartX == lineEndX)
		{
			var startY = Mathf.Min(lineStartY, lineEndY);
			var endY = Mathf.Max(lineStartY, lineEndY);

			for (var y = startY; y <= endY; ++y)
			{
				m_Texture.SetPixel(lineStartX, y, m_LineColor);
			}
		}
		// Horizontal Line
		else if (lineStartY == lineEndY)
		{
			var startX = Mathf.Min(lineStartX, lineEndX);
			var endX = Mathf.Max(lineStartX, lineEndX);

			for (var x = startX; x <= endX; ++x)
			{
				m_Texture.SetPixel(x, lineStartY, m_LineColor);
			}
		}
		// Neither Vertical nor Horizontal -> invalid
		else
		{
			Debug.Log("DrawLine: Attempted to Draw non-straight Line.");
		}
	}
}
