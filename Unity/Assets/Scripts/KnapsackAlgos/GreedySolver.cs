using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GreedySolver : MonoBehaviour
{
	public static GreedySolver Instance;

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

	private GreedyAlgoBehaviour m_Algorithm;
	private List<GreedyAlgoStep> m_RemainingSteps = new();
	//private Stack<DynamicProgAlgoStep> m_ExecutedSteps = new();
	//private List<object> m_ExecutedStepsInfo = new();

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

			m_Autoplay = false;
			m_StepExecutionCounter = 0.0f;

			return;
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
				/*if (m_ExecutedSteps.Count > 0)
				{
					var stepToReverse = m_ExecutedSteps.Pop();
					ReverseStep(stepToReverse);

					m_RemainingSteps.Insert(0, stepToReverse);
				}*/
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

	private GreedyAlgoStep GetNextStep()
	{
		var result = m_RemainingSteps[0];
		m_RemainingSteps.RemoveAt(0);
		//m_ExecutedSteps.Push(result);

		return result;
	}

	private void ExecuteStep()
	{
		if (m_RemainingSteps.Count <= 0)
		{
			return;
		}

		var nextStep = GetNextStep();
		var curItemIndex = nextStep.CurrentIndex;
		var curItem = nextStep.CurrentItem;

		if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_VALUE)
		{
			var itemRatioTable = GetItemRatioTable();

			var curItemValueText = GetText(itemRatioTable, curItemIndex+1, 1);
			curItemValueText.color = m_TextColorHighlightBlue;
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_WEIGHT)
		{
			var itemRatioTable = GetItemRatioTable();
			var curItemValueText = GetText(itemRatioTable, curItemIndex+1, 2);

			curItemValueText.color = m_TextColorHighlightBlue;
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.COMPUTE_VW_RATIO)
		{
			var itemRatioTable = GetItemRatioTable();

			var text = (curItem.Value / (float)curItem.Weight).ToString("n1");
			var x = itemRatioTable.StartX+itemRatioTable.WidthCell + curItemIndex*itemRatioTable.WidthCell;

			DrawText(text, x, itemRatioTable.StartY, itemRatioTable.WidthCell, itemRatioTable.HeightCell);
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_COLUMN)
		{
			var itemRatioTable = GetItemRatioTable();

			var prevHighlightedTexts = new TextMeshProUGUI[]
			{
				GetText(itemRatioTable, curItemIndex+1, 1),
				GetText(itemRatioTable, curItemIndex+1, 2),
				GetText(itemRatioTable, curItemIndex+1, 3)
			};

			foreach (var text in prevHighlightedTexts)
			{
				text.color = m_DefaultTextColor;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_ALL_VW_RATIOS)
		{
			var itemRatioTable = GetItemRatioTable();
			var items = GetItems();

			var ratioTexts = new TextMeshProUGUI[items.Length];
			for (var i = 0; i < items.Length; ++i)
			{
				ratioTexts[i] = GetText(itemRatioTable, i+1, 3);
			}

			foreach (var text in ratioTexts)
			{
				text.color = m_TextColorHighlightOrange;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_CUR_BEST_VW_RATIO)
		{
			var items = GetItems();
			var itemRatioTable = GetItemRatioTable();

			var curBestItem = curItem;
			var curRatio = (curBestItem.Value / (float)curBestItem.Weight).ToString("n1");

			for (var i = 0; i < items.Length; ++i)
			{
				var curItemRatio = GetText(itemRatioTable, i+1, 3);
				if (curItemRatio.text.Equals(curRatio))
				{
					curItemRatio.color = m_TextColorHighlightGreen;
				}
				else
				{
					curItemRatio.color = m_TextColorHighlightRed;
				}
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.DRAW_CUR_BEST_ITEM_INDEX)
		{
			var itemsSorted = m_Algorithm.Items;
			var sortedItemsTable = GetSortedItemsTable();
			var itemRatioTable = GetItemRatioTable();

			int indexForMainTable;
			for (indexForMainTable = 0; indexForMainTable < itemsSorted.Length; ++indexForMainTable)
			{
				if (itemsSorted[indexForMainTable] == curItem)
				{
					break;
				}
			}

			var text = (curItem.Value / (float)curItem.Weight).ToString("n1");
			var x = sortedItemsTable.StartX+sortedItemsTable.WidthCell + indexForMainTable*sortedItemsTable.WidthCell;
			var y = sortedItemsTable.StartY;

			var usedOrigIndices = new List<int>();
			for (var curIndex = 0; ; ++curIndex)
			{
				var curIndexMainTable = GetText(sortedItemsTable, curIndex+1, 0);
				if (curIndexMainTable != null)
				{
					usedOrigIndices.Add(int.Parse(curIndexMainTable.text));
				}
				else
				{
					break;
				}
			}

			var originalIndex = -1;
			for (var i = 0; i < itemsSorted.Length; ++i)
			{
				var ratio = GetText(itemRatioTable, i+1, 3).text;
				if (ratio == text)
				{
					var alreadyUsed = false;
					foreach (var usedIndex in usedOrigIndices)
					{
						if (usedIndex == i)
						{
							alreadyUsed = true;

							break;
						}
					}

					if (!alreadyUsed)
					{
						originalIndex = i;

						break;
					}
				}
			}

			//DrawText(text, x, y, sortedItemsTable.WidthCell, sortedItemsTable.HeightCell);
			DrawText(originalIndex.ToString(), x, y+sortedItemsTable.HeightCell, sortedItemsTable.WidthCell, sortedItemsTable.HeightCell);
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.DRAW_CUR_BEST_VW_RATIO)
		{
			var itemsSorted = m_Algorithm.Items;
			var sortedItemsTable = GetSortedItemsTable();

			int indexForMainTable;
			for (indexForMainTable = 0; indexForMainTable < itemsSorted.Length; ++indexForMainTable)
			{
				if (itemsSorted[indexForMainTable] == curItem)
				{
					break;
				}
			}

			var text = (curItem.Value / (float)curItem.Weight).ToString("n1");
			var x = sortedItemsTable.StartX+sortedItemsTable.WidthCell + indexForMainTable*sortedItemsTable.WidthCell;
			var y = sortedItemsTable.StartY;

			DrawText(text, x, y, sortedItemsTable.WidthCell, sortedItemsTable.HeightCell);
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_CUR_BEST_ALL)
		{
			var itemRatioTable = GetItemRatioTable();
			var itemCount = GetItems().Length;

			for (var i = 0; i < itemCount; ++i)
			{
				GetText(itemRatioTable, i+1, 3).color = m_DefaultTextColor;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_BEST_ITEM)
		{
			var itemRatioTable = GetItemRatioTable();
			var sortedItemsTable = GetSortedItemsTable();

			var curIndexOfMain = int.Parse(GetText(sortedItemsTable, curItemIndex+1, 0).text);

			var texts = new TextMeshProUGUI[]
			{
				GetText(sortedItemsTable, curItemIndex+1, 1),

				GetText(itemRatioTable, curIndexOfMain+1, 0),
				GetText(itemRatioTable, curIndexOfMain+1, 2)
			};

			foreach (var text in texts)
			{
				text.color = m_TextColorHighlightBlue;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_CAP_AND_WEIGHT)
		{
			var itemRatioTable = GetItemRatioTable();
			var sortedItemsTable = GetSortedItemsTable();

			var curIndexOfMain = int.Parse(GetText(sortedItemsTable, curItemIndex+1, 0).text);

			var capText = GetText(sortedItemsTable, -1, 0);
			var weightText = GetText(itemRatioTable, curIndexOfMain+1, 2);

			capText.color = m_TextColorHighlightOrange;
			weightText.color = m_TextColorHighlightOrange;
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.CHOOSE_IF_TAKE)
		{
			var itemRatioTable = GetItemRatioTable();
			var sortedItemsTable = GetSortedItemsTable();

			var capText = GetText(sortedItemsTable, -1, 0);
			var curIndexOfMain = int.Parse(GetText(sortedItemsTable, curItemIndex+1, 0).text);
			var weightText = GetText(itemRatioTable, curIndexOfMain+1, 2);

			var curCapacity = int.Parse(capText.text.Substring(6));
			var weight = curItem.Weight;

			if (weight <= curCapacity)
			{
				capText.color = m_TextColorHighlightGreen;
				weightText.color = m_TextColorHighlightGreen;
			}
			else
			{
				capText.color = m_TextColorHighlightRed;
				weightText.color = m_TextColorHighlightRed;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.ADD_TO_TOT_VAL)
		{
			var sortedItemsTable = GetSortedItemsTable();

			var totValText = GetText(sortedItemsTable, -1, 1);
			var capText = GetText(sortedItemsTable, -1, 0);

			var curCapacity = int.Parse(capText.text.Substring(6));
			var weight = curItem.Weight;

			if (weight <= curCapacity)
			{
				var curTotValue = int.Parse(totValText.text.Substring(9));

				totValText.color = m_TextColorHighlightGreen;
				totValText.text = "Tot Val: " + (curTotValue + curItem.Value).ToString();

				capText.text = "Capa: " + (curCapacity - curItem.Weight).ToString();
			}
			else
			{
				totValText.color = m_TextColorHighlightRed;
			}
		}
		else if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_CAP_WEIGHT)
		{
			var sortedItemsTable = GetSortedItemsTable();
			var itemRatioTable = GetItemRatioTable();

			var curIndexOfMain = int.Parse(GetText(sortedItemsTable, curItemIndex+1, 0).text);

			var texts = new TextMeshProUGUI[]
			{
				GetText(sortedItemsTable, -1, 1),
				GetText(sortedItemsTable, -1, 0),
				GetText(sortedItemsTable, curItemIndex+1, 1),

				GetText(itemRatioTable, curIndexOfMain+1, 0),
				GetText(itemRatioTable, curIndexOfMain+1, 2)

			};
			
			foreach (var text in texts)
			{
				text.color = m_DefaultTextColor;
			}
		}
	}

	private void ReverseStep(DynamicProgAlgoStep step)
	{
		//var stepInfo = m_ExecutedStepsInfo[m_ExecutedStepsInfo.Count-1];
		
		//m_ExecutedStepsInfo.RemoveAt(m_ExecutedStepsInfo.Count-1);
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

    private SolverTable GetItemRatioTable()
    {
        return m_Tables[0];
    }

	private SolverTable GetSortedItemsTable()
	{
		return m_Tables[1];
	}

	/*private void DrawItemValue(int value, int curCapacity, int itemIndex)
	{
		var itemTable = GetMainTable();
		var width = itemTable.WidthCell;
		var height = itemTable.HeightCell;

		var x = itemTable.StartX + curCapacity*width;
		var y = itemTable.EndY-(height*2) - itemIndex*height;

		DrawText(value.ToString(), x, y, width, height);
	}*/

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

	private void DrawItemRatioTable()
	{
        var items = GetItems();

		var freeWidth = m_ScreenWidth * (1.0f - m_TableWidthPercentage);
		// Die Tabelle ist zentriert -> Links und Rechts gleich viel Freiraum
		var startX = (int)(freeWidth * 0.5f);
		var endX = (int)(m_ScreenWidth - freeWidth*0.5f);

		var startY = (int)(m_ScreenHeight * 0.6f);
        var endY = (int)(m_ScreenHeight * 0.95f);

		m_Tables.Add(new(4, items.Length+1, startX, startY, endX, endY));

		var curTable = m_Tables[m_Tables.Count-1];
		curTable.Render(m_Texture, m_LineColor);

		var widthCell = curTable.WidthCell;
		var heightCell = curTable.HeightCell;

		// Drawing the Item Indices (Column Names)
		var curX = curTable.StartX + widthCell;
		var y = curTable.EndY - heightCell;

		for (var i = 0; i < items.Length; ++i)
		{
			DrawText(i.ToString(), curX, y, widthCell, heightCell);
			curX += widthCell;
		}

		var x = curTable.StartX;
		var curY = curTable.EndY - heightCell*2;

		DrawText("Value", x, curY, widthCell, heightCell);
		DrawText("Weight", x, curY - heightCell, widthCell, heightCell);
		DrawText("V/W Ratio", x, curY - heightCell*2, widthCell, heightCell);

		// Werte und Gewichte der Items eintragen
		curX = curTable.StartX + widthCell;
		y = curTable.EndY - heightCell*2;

        foreach (var item in items)
        {
			DrawText(item.Value.ToString(), curX, y, widthCell, heightCell);
			DrawText(item.Weight.ToString(), curX, y - heightCell, widthCell, heightCell);

			curX += widthCell;
		}
	}

	private void DrawSortedItemsTable()
	{
		var items = GetItems();

		var freeWidth = m_ScreenWidth * (1.0f - m_TableWidthPercentage);
		// Die Tabelle ist rechtsorientiert
		var startX = (int)(freeWidth * 0.97f);
		var endX = (int)(m_ScreenWidth - freeWidth*0.03f);

		var startY = (int)(m_ScreenHeight * 0.05f);
		var endY = (int)(m_ScreenHeight * 0.55f);

		m_Tables.Add(new(2, items.Length+1, startX, startY, endX, endY));

		var curTable = m_Tables[m_Tables.Count-1];
		curTable.Render(m_Texture, m_LineColor);

		var widthCell = curTable.WidthCell;
		var heightCell = curTable.HeightCell;

		DrawText("V/W Ratio", curTable.StartX, curTable.StartY, widthCell, heightCell);

		var capacity = PlayerHUDController.Instance.GetKnapsackCapacity();

		var x = curTable.StartX - widthCell;
		var y = curTable.EndY-heightCell;

		DrawText("Capa: " + capacity, x, y, widthCell, heightCell);
		DrawText("Tot Val: 0", x, y - heightCell, widthCell, heightCell);
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

	private ItemPropertiesNoUnity[] GetItems()
	{
		return m_Algorithm.ItemsUnmodified;
	}

	private List<AlgoStep> GetSteps()
	{
		return m_Algorithm.GetSteps();
	}

	public void Solve(GreedyAlgoBehaviour algorithm)
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
        DrawItemRatioTable();
        DrawSortedItemsTable();

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
