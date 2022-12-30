using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Queue<GreedyAlgoStep> m_RemainingSteps = new();
    //private Queue<DynamicProgAlgoStep> m_ExecutedSteps = new();

    private float m_StepExecutionCounter;
    // One Step per Second by default; Can be adjusted by pressing J or L
    // J -> -0.2, L -> +0.2; Range = [0.2, 2.0]
    private float m_StepExecutionSpeed = 1.0f;

    private Color m_DefaultTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    private Color m_HighlightedTextColor = new(0.8f, 0.0f, 0.4f, 1.0f);

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

        if (Input.GetKeyDown(KeyCode.J) && m_StepExecutionSpeed < 1.95f)
        {
            m_StepExecutionSpeed += 0.2f;
        }
        else if (Input.GetKeyDown(KeyCode.L) && m_StepExecutionSpeed > 0.25f)
        {
            m_StepExecutionSpeed -= 0.2f;
        }

        m_StepExecutionCounter += Time.deltaTime;
        // After 1 Second
        if (m_StepExecutionCounter >= m_StepExecutionSpeed)
        {
            ExecuteStep();
            m_StepExecutionCounter = 0.0f;
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

            if (itemIndex == curCol - 1)
            {
                foreach (var text in texts)
                {
                    text.color = m_HighlightedTextColor;
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
                capacityText.color = m_HighlightedTextColor;
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
                if (i == (itemIndex + 1))
                {
                    indexText.color = m_HighlightedTextColor;
                }
                else
                {
                    indexText.color = m_DefaultTextColor;
                }
            }
        }
    }

    private void ExecuteStep()
    {
        if (m_RemainingSteps.Count <= 0)
        {
            return;
        }

        var nextStep = m_RemainingSteps.Dequeue();
        //if (nextStep.Operation == DynamicProgAlgoStep.DynamicAlgoOperations.ONE)
        //{
        //    DrawItemValue((int)nextStep.Values, nextStep.CurrentWeight, nextStep.CurItemIndex);
        //
        //    HighlightItemTableItem(nextStep.CurItemIndex);
        //    HighlightMainTableCurCapacityAndIndex(nextStep.CurCapacity, nextStep.CurItemIndex);
        //}
        if (nextStep.Operation == GreedyAlgoStep.GreedyAlgoOperations.Starting) 
        {
            //DrawItemValue((int)nextStep.Values, nextStep.CurrentWeight, nextStep.CurItemIndex);
            
        }
        DrawItemValue((int)nextStep.Values, nextStep.CurrentWeight, nextStep.CurrentIndex);
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
        return x >= (calcX - 5) && x <= (calcX + 5) &&
               y >= (calcY - 5) && y <= (calcY + 5);
    }

    private TextMeshProUGUI GetText(SolverTable table, int col, int row)
    {
        int calcTextX = table.StartX + col * table.WidthCell + table.WidthCell / 2;
        int calcTextY = table.EndY - table.HeightCell - row * table.HeightCell + table.HeightCell / 2;

        foreach (var textObj in GetTextObjects())
        {
            var rectTransform = textObj.GetComponent<RectTransform>();
            var actualX = (int)(rectTransform.localPosition.x + m_ScreenWidth / 2);
            var actualY = (int)(rectTransform.localPosition.y + m_ScreenHeight / 2);

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

        var x = itemTable.StartX + curCapacity * width;
        var y = itemTable.EndY - (height * 2) - itemIndex * height;

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
        var endX = (int)(m_ScreenWidth - freeWidth * 0.5f);

        var startY = (int)(m_ScreenHeight * 0.05f);
        var endY = (int)(m_ScreenHeight * 0.6f);

        m_Tables.Add(new(items.Length + 1, capacity + 1, startX, startY, endX, endY));

        var curTable = m_Tables[m_Tables.Count - 1];
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
        var curY = curTable.EndY - heightCell * 2;

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
        var endX = (int)(m_ScreenWidth - freeWidth * 0.5f);

        var startY = (int)(m_ScreenHeight * 0.65f);
        var endY = (int)(m_ScreenHeight * 0.95f);

        var countColumns = items.Length + 1;
        var countRows = 3;

        // Rows -> ItemIndex, Value, Weight
        m_Tables.Add(new(countRows, countColumns, startX, startY, endX, endY));

        var curTable = m_Tables[m_Tables.Count - 1];
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
            DrawText(i.ToString(), curX, startY + heightCell * 2, widthCell, heightCell);

            curX += widthCell;
        }
    }

    // x,y | the Position of the Bottom Left-Hand Corner of the Text
    // width, height | the Expand of the Text towards the Upper Right
    private void DrawText(string text, int x, int y, int width, int height)
    {
        var instance = Instantiate(TextPrefab, new(), TextPrefab.transform.rotation, transform);
        instance.name = "Text";

        x += width / 2 - m_ScreenWidth / 2;
        y += height / 2 - m_ScreenHeight / 2;

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
        DrawMainTable();
        DrawItemTable();

        //DrawText("Test", 0, 0, 100, 60);

        m_Texture.Apply();

        foreach (var step in GetSteps())
        {
            m_RemainingSteps.Enqueue((GreedyAlgoStep)step);
        }
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
