using System;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=211t6r12XPQ
//Originally by Game Dev Guide

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    private TabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    public void Subscribe(TabButton button)
    {
        tabButtons ??= new List<TabButton>();
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.sprite = tabHover;
    }

    public void OnTabExit(TabButton button) 
        => ResetTabs();

    public void OnTabSelected(TabButton button)
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
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
                continue;
            button.background.sprite = tabIdle;
        }
    }
}
