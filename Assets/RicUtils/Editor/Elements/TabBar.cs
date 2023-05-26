using RicUtils.Editor.Utilities;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RicUtils.Editor.Elements
{
    public class TabBar
    {
        public VisualElement container;
        public VisualElement tabBarContainer;
        public VisualElement contentContainer;
        public List<TabData> tabs = new List<TabData>();
        private TabData currentTab;
        private const string HIDDEN_CLASS = "hidden";
        private const string PRESSED_CLASS = "pressed";

        internal TabBar(VisualElement root)
        {
            container = new VisualElement();
            tabBarContainer = new VisualElement();
            contentContainer = new VisualElement();
            container.Add(tabBarContainer);
            container.Add(contentContainer);
            root.Add(container);

            tabBarContainer.AddToClassList("tabBar");

            container.AddStylesheet("RicUtils/TabBar.uss");
        }

        public TabData AddTab(string name)
        {
            TabData tab = new TabData();
            tab.name = name;

            tab.tabButton = tabBarContainer.AddButton(name, () =>
            {
                SwitchTab(tab);
            });

            VisualElement container = new VisualElement();

            contentContainer.Add(container);

            tab.container = container;

            tab.container.AddToClassList(HIDDEN_CLASS);

            tab.tabButton.AddToClassList("tabButton");

            tab.tabButton.focusable = false;

            if (currentTab.container == null)
            {
                currentTab = tab;
                SwitchTab(tab);
            }

            return tab;
        }

        private void SwitchTab(TabData tab)
        {
            currentTab.container.AddToClassList(HIDDEN_CLASS);
            currentTab.tabButton.RemoveFromClassList(PRESSED_CLASS);
            currentTab = tab;
            currentTab.container.RemoveFromClassList(HIDDEN_CLASS);
            currentTab.tabButton.AddToClassList(PRESSED_CLASS);
        }
    }
}
