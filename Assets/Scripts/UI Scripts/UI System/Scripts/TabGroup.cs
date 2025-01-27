using System;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=211t6r12XPQ
//Originally by Game Dev Guide

public class TabGroup : MonoBehaviour
{
    public List<CustomTabButton> tabButtons;
    private CustomTabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    public void Subscribe(CustomTabButton button)
    {
        tabButtons ??= new List<CustomTabButton>();
        tabButtons.Add(button);
    }

    public void OnTabEnter(CustomTabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.sprite = tabHover;
    }

    public void OnTabExit(CustomTabButton button) 
        => ResetTabs();

    public void OnTabSelected(CustomTabButton button)
    {
        if (selectedTab != null)
            selectedTab.Deselect();

        selectedTab = button;

        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
            objectsToSwap[i].SetActive(i == index);
    }

    private void ResetTabs()
    {
        foreach (CustomTabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
                continue;
            button.background.sprite = tabIdle;
        }
    }
}
