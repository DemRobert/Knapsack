using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

	private List<SolverTable> m_Tables = new();

	private DynamicProgAlgoBehaviour m_Algorithm;
	private List<DynamicProgAlgoStep> m_RemainingSteps = new();
	private Stack<DynamicProgAlgoStep> m_ExecutedSteps = new();
	private List<object> m_ExecutedStepsInfo = new();

	private bool m_Autoplay;
	private float m_StepExecutionCounter;
	// One Step per Second by default; Can be adjusted by pressing J or L
	// J -> -0.2, L -> +0.2; Range = [0.2, 2.0]
	private float m_StepExecutionSpeed = 1.0f;

	private Color m_DefaultTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
	private Color m_TextColorHighlightRed = new(0.8f, 0.0f, 0.4f, 1.0f);
	private Color m_TextColorHighlightOrange = new(0.8f, 0.8f, 0.0f, 1.0f);
	private Color m_TextColorHighlightOrange2 = new(0.65f, 0.65f, 0.0f, 1.0f);
	private Color m_TextColorHighlightBlue = new(0.0f, 0.2f, 0.7f, 1.0f);
	private Color m_TextColorHighlightGreen = new(0.2f, 0.7f, 0.2f, 1.0f);

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		var rect = GetComponent<RectTransform>();
		m_ScreenHeight = (int)rect.rect.height;

		m_Texture = new(m_ScreenWidth, m_ScreenHeight);

		m_BackgroundAndLinesImageComp = BackgroundAndLines.GetComponent<RawImage>();
		m_BackgroundAndLinesImageComp.texture = m_Texture;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			SolvingDone();
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			m_Autoplay = !m_Autoplay;

			if (!m_Autoplay)
			{
				m_StepExecutionCounter = 0.0f;
			}
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			if (m_Autoplay)
			{
				if (m_StepExecutionSpeed < 1.95f)
				{
					m_StepExecutionSpeed += 0.2f;
				}
			}
			else
			{
				var stepToReverse = m_ExecutedSteps.Pop();
				ReverseStep(stepToReverse);

				m_RemainingSteps.Insert(0, stepToReverse);
			}
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			if (m_Autoplay)
			{
				if (m_StepExecutionSpeed > 0.25f)
				{
					m_StepExecutionSpeed -= 0.2f;
				}
			}
			else
			{
				ExecuteStep();
			}
		}

		if (m_Autoplay)
		{
			m_StepExecutionCounter += Time.deltaTime;
			// After 1 Second
			if (m_StepExecutionCounter >= m_StepExecutionSpeed)
			{
				ExecuteStep();
				m_StepExecutionCounter = 0.0f;
			}
		}
	}

	private void HighlightItemTableItem(int itemIndex)
	{
		var itemTable = GetItemTable();
		for (var curCol = 1; curCol < itemTable.CountColumns; ++curCol)
		{
			var texts = new TextMeshProUGUI[]
			{
				GetText(itemTable, curCol, 0),
				GetText(itemTable, curCol, 1),
				GetText(itemTable, curCol, 2)
			};

			if (itemIndex == curCol-1)
			{
				foreach (var text in texts)
				{
					text.color = m_TextColorHighlightBlue;
				}
			}
			else
			{
				foreach (var text in texts)
				{
					text.color = m_DefaultTextColor;
				}
			}
		}
	}

	private void HighlightMainTableCurCapacityAndIndex(int curCapacity, int itemIndex)
	{
		var mainTable = GetMainTable();

		for (var i = 1; i <= GetCapacity(); ++i)
		{
			var capacityText = GetText(mainTable, i, 0);
			if (i == curCapacity)
			{
				capacityText.color = m_TextColorHighlightBlue;
			}
			else
			{
				capacityText.color = m_DefaultTextColor;
			}
		}

		if (curCapacity == 1)
		{
			for (var i = 1; i <= GetItems().Length; ++i)
			{
				var indexText = GetText(mainTable, 0, i);
				if (i == (itemIndex+1))
				{
					indexText.color = m_TextColorHighlightBlue;
				}
				else
				{
					indexText.color = m_DefaultTextColor;
				}
			}
		}
	}

	private DynamicProgAlgoStep GetNextStep()
	{
		var result = m_RemainingSteps[0];
		m_RemainingSteps.RemoveAt(0);
		m_ExecutedSteps.Push(result);

		return result;
	}

	private void ReverseStep(DynamicProgAlgoStep step)
	{
		if (step.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.END)
		{
		}
	}

	private void ExecuteStep()
	{
		if (m_RemainingSteps.Count <= 0)
		{
			return;
		}

		var nextStep = GetNextStep();
		var curItemIndex = nextStep.CurItemIndex;
		var curCapacity = nextStep.CurCapacity;

		if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.NEW_CELL)
		{
			HighlightItemTableItem(curItemIndex);
			HighlightMainTableCurCapacityAndIndex(curCapacity, curItemIndex);

			m_ExecutedStepsInfo.Add(new object[]{curCapacity, curItemIndex});
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT01)
		{
			var curItemWeightText = GetText(GetItemTable(), curItemIndex+1, 2);
			curItemWeightText.color = m_TextColorHighlightOrange;

			m_ExecutedStepsInfo.Add(curItemIndex);
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT02)
		{
			var isItemOk = (bool)nextStep.Values;
			var curItemWeightText = GetText(GetItemTable(), curItemIndex+1, 2);

			curItemWeightText.color = isItemOk ? m_TextColorHighlightGreen : m_TextColorHighlightRed;

			m_ExecutedStepsInfo.Add(curItemIndex);
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT03)
		{
			// Skip the current Step if we are in the first Row (there is no Row above)
			if (curItemIndex == 0)
			{
				return;
			}

			var textsForReversing = new TextMeshProUGUI[3];

			var mainTable = GetMainTable();
			var textCellWithoutCurItem = GetText(mainTable, curCapacity, curItemIndex);
			textsForReversing[0] = textCellWithoutCurItem;

			var curItem = GetItems()[curItemIndex];
			// If the current Item doesn't fit because its Weight is too big
			// -> use the Celltext of the Cell above or 0 if it is the first Row (next Step)
			if (curItem.weight > curCapacity)
			{
				textCellWithoutCurItem.color = m_TextColorHighlightGreen;
			}
			else
			{
				var withCurItemOldCapacity = (int)nextStep.Values;
				if (withCurItemOldCapacity > 0)
				{
					var textCellWithCurItem = GetText(mainTable, withCurItemOldCapacity, curItemIndex);
					textsForReversing[1] = textCellWithCurItem;
					textCellWithCurItem.color = m_TextColorHighlightOrange2;
				}

				var textCurValueItemTable = GetText(GetItemTable(), curItemIndex+1, 1);
				textsForReversing[2] = textCurValueItemTable;
				textCurValueItemTable.color = m_TextColorHighlightOrange2;

				textCellWithoutCurItem.color = m_TextColorHighlightOrange;
			}

			m_ExecutedStepsInfo.Add(textsForReversing);
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.HIGHLIGHT_BEST_OPTION)
		{
			var isBetterWithCurItem = (bool)nextStep.Values;

			var mainTable = GetMainTable();
			var itemTable = GetItemTable();
			var curItem = GetItems()[curItemIndex];

			var oldTextColors = new Dictionary<TextMeshProUGUI, Color>();

			var textCurValueItemTable = GetText(itemTable, curItemIndex+1, 1);
			oldTextColors.Add(textCurValueItemTable, textCurValueItemTable.color);
			var textCellWithoutCurItem = curItemIndex == 0 ? null : GetText(mainTable, curCapacity, curItemIndex);
			if (textCellWithoutCurItem != null)
			{
				oldTextColors.Add(textCellWithoutCurItem, textCellWithoutCurItem.color);
			}

			var textCurWeightItemTable = GetText(itemTable, curItemIndex+1, 2);
			oldTextColors.Add(textCurWeightItemTable, textCurWeightItemTable.color);
			textCurWeightItemTable.color = m_TextColorHighlightBlue;

			var withCurItemOldCapacity = curCapacity - curItem.weight;
			TextMeshProUGUI textCellWithCurItem = null;
			if (withCurItemOldCapacity > 0 && curItemIndex > 0)
			{
				textCellWithCurItem = GetText(mainTable, withCurItemOldCapacity, curItemIndex);
			}

			if (isBetterWithCurItem)
			{
				if (textCellWithoutCurItem != null)
				{
					textCellWithoutCurItem.color = m_TextColorHighlightRed;
				}

				textCurValueItemTable.color = m_TextColorHighlightGreen;
				if (textCellWithCurItem != null)
				{
					textCellWithCurItem.color = m_TextColorHighlightGreen;
				}
			}
			else
			{
				if (textCellWithoutCurItem != null)
				{
					textCellWithoutCurItem.color = m_TextColorHighlightGreen;
				}

				textCurValueItemTable.color = m_TextColorHighlightRed;
				if (textCellWithCurItem != null)
				{
					textCellWithCurItem.color = m_TextColorHighlightRed;
				}
			}

			m_ExecutedStepsInfo.Add(oldTextColors);
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_VALUES)
		{
			// This means the Item's weight exceeded the Capacity
			if (nextStep.Values is int)
			{
				DrawItemValue((int)nextStep.Values, curCapacity, curItemIndex);
			}
			else
			{
				var curItem = GetItems()[curItemIndex];
				var mainTable = GetMainTable();

				var isBetterWithCurItem = (bool)nextStep.Values;
				if (isBetterWithCurItem)
				{
					if (curItemIndex == 0)
					{
						DrawItemValue(curItem.value, curCapacity, curItemIndex);
					}
					else
					{
						var textCellWithCurItemCol = curCapacity - curItem.weight;
						var textCellWithCurItemValue = 0;

						if (textCellWithCurItemCol > 0)
						{
							textCellWithCurItemValue = int.Parse(GetText(mainTable, textCellWithCurItemCol, curItemIndex).text);
						}

						DrawItemValue(textCellWithCurItemValue + curItem.value, curCapacity, curItemIndex);
					}
				}
				else
				{
					if (curItemIndex == 0)
					{
						DrawItemValue(0, curCapacity, curItemIndex);
					}
					else
					{
						var textCellWithoutCurItem = GetText(mainTable, curCapacity, curItemIndex);
						DrawItemValue(int.Parse(textCellWithoutCurItem.text), curCapacity, curItemIndex);
					}
				}
			}

			m_ExecutedStepsInfo.Add(new object[]{curCapacity, curItemIndex});
		}
		else if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.END)
		{
			var mainTable = GetMainTable();
			var curItem = GetItems()[curItemIndex];
			var curWeight = curItem.weight;

			var oldTextColors = new Dictionary<TextMeshProUGUI, Color>();

			if (curItemIndex != 0)
			{
				var textCellAbove = GetText(mainTable, curCapacity, curItemIndex);
				oldTextColors.Add(textCellAbove, textCellAbove.color);
				textCellAbove.color = m_DefaultTextColor;

				var indexCellWithCurItem = curCapacity - curWeight;
				if (indexCellWithCurItem > 0)
				{
					var textCellWithCurItem = GetText(mainTable, indexCellWithCurItem, curItemIndex);
					textCellWithCurItem.color = m_DefaultTextColor;
				}
			}
		}
	}

	private List<GameObject> GetTextObjects()
	{
		var result = new List<GameObject>(transform.childCount - 1);
		for (var i = 0; i < transform.childCount; ++i)
		{
			var curChild = transform.GetChild(i);
			if (curChild.name.Equals("Background"))
			{
				continue;
			}

			result.Add(curChild.gameObject);
		}

		return result;
	}

	private bool GetTextIsEqualPos(int x, int calcX, int y, int calcY)
	{
		return x >= (calcX-5) && x <= (calcX+5) &&
			   y >= (calcY-5) && y <= (calcY+5);
	}

	private TextMeshProUGUI GetText(SolverTable table, int col, int row)
	{
		int calcTextX = table.StartX + col*table.WidthCell + table.WidthCell/2;
		int calcTextY = table.EndY-table.HeightCell - row*table.HeightCell + table.HeightCell/2;

		foreach (var textObj in GetTextObjects())
		{
			var rectTransform = textObj.GetComponent<RectTransform>();
			var actualX = (int)(rectTransform.localPosition.x + m_ScreenWidth/2);
			var actualY = (int)(rectTransform.localPosition.y + m_ScreenHeight/2);

			if (GetTextIsEqualPos(actualX, calcTextX, actualY, calcTextY))
			{
				return textObj.GetComponent<TextMeshProUGUI>();
			}
		}

		return null;
	}

	private void DrawItemValue(int value, int curCapacity, int itemIndex)
	{
		var itemTable = GetMainTable();
		var width = itemTable.WidthCell;
		var height = itemTable.HeightCell;

		var x = itemTable.StartX + curCapacity*width;
		var y = itemTable.EndY-(height*2) - itemIndex*height;

		DrawText(value.ToString(), x, y, width, height);
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

		// Remove all Texts
		for (var i = 0; i < transform.childCount; ++i)
		{
			var child = transform.GetChild(i);
			if (!child.name.Equals("Background"))
			{
				Destroy(child.gameObject);
			}
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

		m_Tables.Add(new(items.Length+1, capacity+1, startX, startY, endX, endY));

		var curTable = m_Tables[m_Tables.Count-1];
		curTable.Render(m_Texture, m_LineColor);

		var widthCell = curTable.WidthCell;
		var heightCell = curTable.HeightCell;

		// Drawing the Capacities (Column Names)
		var curX = curTable.StartX + widthCell;
		var y = curTable.EndY - heightCell;

		for (var i = 1; i <= capacity; ++i)
		{
			DrawText(i.ToString(), curX, y, widthCell, heightCell);
			curX += widthCell;
		}

		// Drawing the Item Indices (Row Names)
		var x = curTable.StartX;
		var curY = curTable.EndY - heightCell*2;

		for (var i = 0; i < items.Length; ++i)
		{
			DrawText(i.ToString(), x, curY, widthCell, heightCell);
			curY -= heightCell;
		}
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

		var countColumns = items.Length+1;
		var countRows = 3;

		// Rows -> ItemIndex, Value, Weight
		m_Tables.Add(new(countRows, countColumns, startX, startY, endX, endY));

		var curTable = m_Tables[m_Tables.Count-1];
		curTable.Render(m_Texture, m_LineColor);

		var widthCell = curTable.WidthCell;
		var heightCell = curTable.HeightCell;

		// Draw the Value(Col=0, Row=1) and Weight(Col=0, Row=2) Texts
		DrawText("Value", startX, startY + heightCell, widthCell, heightCell);
		DrawText("Weight", startX, startY, widthCell, heightCell);

		var curX = startX + widthCell;
		for (var i = 0; i < items.Length; ++i)
		{
			var curItem = items[i];

			// From Bottom to Top in Table
			DrawText(curItem.weight.ToString(), curX, startY, widthCell, heightCell);
			DrawText(curItem.value.ToString(), curX, startY + heightCell, widthCell, heightCell);
			DrawText(i.ToString(), curX, startY + heightCell*2, widthCell, heightCell);

			curX += widthCell;
		}
	}

	// x,y | the Position of the Bottom Left-Hand Corner of the Text
	// width, height | the Expand of the Text towards the Upper Right
	private void DrawText(string text, int x, int y, int width, int height)
	{
		var instance = Instantiate(TextPrefab, new(), TextPrefab.transform.rotation, transform);
		instance.name = "Text";

		x += width/2 - m_ScreenWidth/2;
		y += height/2 - m_ScreenHeight/2;

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
		return m_Algorithm.Items;
	}

	private List<AlgoStep> GetSteps()
	{
		return m_Algorithm.GetSteps();
	}

	private SolverTable GetMainTable()
	{
		return m_Tables[0];
	}

	private SolverTable GetItemTable()
	{
		return m_Tables[1];
	}

	public void Solve(DynamicProgAlgoBehaviour algorithm)
	{
		if (m_Algorithm != null)
		{
			return;
		}

		m_Algorithm = algorithm;

		// Disable the HUD Canvas but Enable the Background Image
		PlayerHUD.SetActive(false);
		BackgroundAndLines.SetActive(true);

		// Solving...
		ClearScreen();
		DrawMainTable();
		DrawItemTable();

		//DrawText("Test", 0, 0, 100, 60);

		m_Texture.Apply();

		m_RemainingSteps.AddRange(GetSteps());
	}

	public void SolvingDone()
	{
		m_RemainingSteps.Clear();
		m_Tables.Clear();
		ClearScreen();

		BackgroundAndLines.SetActive(false);
		PlayerHUD.SetActive(true);

		m_Algorithm = null;
	}
}
