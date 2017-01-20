﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour {

	public Image[] itemImage = new Image[numItemSloths];
	public Item[] items = new Item[numItemSloths];

	public const int numItemSloths = 4;

	public void AddItem(Item item)
	{
		for (int i =0; i < items.Length; i++)
		{
			if (items[i] == null)
			{
				items[i] = item;
				itemImage[i].sprite = item.sprite;
				itemImage[i].enabled = true;
				return;
			}
		}
	}

	public void RemoveItem(Item item)
	{
		for (int i =0; i < items.Length; i++)
		{
			if (items[i] == item)
			{
				items[i] = null;
				itemImage[i].sprite = null;
				itemImage[i].enabled = false;
				return;
			}
		}
	}
}
