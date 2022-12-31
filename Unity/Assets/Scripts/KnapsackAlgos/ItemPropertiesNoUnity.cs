using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPropertiesNoUnity
{
	public int Value;
	public int Weight;

	public ItemPropertiesNoUnity(int value, int weight)
	{
		Value = value;
		Weight = weight;
	}

	public ItemPropertiesNoUnity(ItemProperties props) :
		this(props.value, props.weight)
	{
	}
}
