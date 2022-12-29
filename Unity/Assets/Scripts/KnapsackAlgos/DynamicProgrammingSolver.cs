using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicProgrammingSolver : MonoBehaviour
{
	public static DynamicProgrammingSolver Instance;

    public GameObject PlayerHUD;
    public GameObject BackgroundAndLines;

	private Texture2D m_Texture;
	private RawImage m_BackgroundAndLinesImageComp;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		m_Texture = new(1920, 1080);
	}

	public void Solve()
	{
		// Disable the HUD Canvas but Enable the Background Image
		PlayerHUD.SetActive(false);
		BackgroundAndLines.SetActive(true);

		// Solving...
		for (var y = 0; y < m_Texture.height; ++y)
		{
			for (var x = 0; x < m_Texture.width; ++x)
			{
				m_Texture.SetPixel(x, y, new Color(1.0f, 0.0f, 0.0f, 1.0f));
			}
		}

		m_Texture.Apply();

		//DrawLine(new(0.0f, 0.0f, 0.0f), new(100.0f, 100.0f, 0.0f));

		m_BackgroundAndLinesImageComp = BackgroundAndLines.GetComponent<RawImage>();
		m_BackgroundAndLinesImageComp.texture = m_Texture;

		// Solving done
		//BackgroundAndLines.SetActive(false);
		//PlayerHUD.SetActive(true);
	}

	private void DrawLine(Vector3 startPos, Vector3 endPos)
	{
	}
}
