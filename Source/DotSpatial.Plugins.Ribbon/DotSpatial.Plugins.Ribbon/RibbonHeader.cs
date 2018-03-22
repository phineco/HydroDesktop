// Decompiled with JetBrains decompiler
// Type: DotSpatial.RibbonHeader
// Assembly: DotSpatial.Plugins.Ribbon, Version=1.4.25.0, Culture=neutral, PublicKeyToken=null
// MVID: 35EAF35D-1435-44D1-B1C4-F90BDA67A063
// Assembly location: C:\Users\THINK\Documents\GitHub\HydroDesktop\dependencePlugin\DotSpatial.Plugins.Ribbon\DotSpatial.Plugins.Ribbon.dll

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DotSpatial
{
  [Export(typeof (IHeaderControl))]
  public class RibbonHeader : HeaderControl, IPartImportsSatisfiedNotification, IStatusControl, IProgressHandler
  {
    private static List<string> groups = new List<string>();
    private const int INT_MillisecondsToRefreshStatusBar = 200;
    private const string STR_DefaultGroupName = "Default Group";
    private bool _nextItemBeginsGroup;
    private RibbonControl _ribbon;
    private RibbonStatusBar ribbonStatusBar;
    private ApplicationMenu backstageMenu;
    private BarStaticItem defaultStatusPanel;
    private BarEditItem defaultProgressPanel;
    private RepositoryItemProgressBar defaultProgressBar;
    //private readonly Stopwatch stopwatch;

    [Import("Shell", typeof (ContainerControl))]
    public ContainerControl Shell { get; set; }

    public RibbonHeader()
    {
      //base.ctor();
      SkinManager.EnableFormSkins();
      UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
      //this.stopwatch.Start();
    }

    public override void Add(RootItem item)
    {
      Guard.ArgumentNotNull((object) item, "rootItem");
      if (!RibbonHeader.ShouldCreateRibbonPage(((HeaderItem) item).Key))
        return;
      RibbonPage page1 = this.GetExistingRibbonPage(((HeaderItem) item).Key);
      if (page1 == null)
      {
        page1 = new RibbonPage(item.Caption);
        page1.Name = ((HeaderItem) item).Key;
        this._ribbon.Pages.Add(page1);
      }
      else
        page1.Text = item.Caption;
      page1.Tag = (object) item.SortOrder;
      page1.MergeOrder = (int) item.SortOrder;
      List<RibbonPage> source = new List<RibbonPage>(this._ribbon.Pages.Count);
      foreach (RibbonPage page2 in (CollectionBase) this._ribbon.Pages)
        source.Add(page2);
      IOrderedEnumerable<RibbonPage> orderedEnumerable = source.OrderBy<RibbonPage, int>((Func<RibbonPage, int>) (entry => entry.MergeOrder));
      this._ribbon.Pages.Clear();
      foreach (RibbonPage page2 in (IEnumerable<RibbonPage>) orderedEnumerable)
        this._ribbon.Pages.Add(page2);
      ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.RootItem_PropertyChanged));
    }

    public override void Add(MenuContainerItem item)
    {
      Guard.ArgumentNotNull((object) item, nameof (item));
      RibbonPageGroup group = RibbonHeader.GetOrCreateGroup(this.GetRibbonPage((GroupedItem) item), ((GroupedItem) item).GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));
      BarSubItem barSubItem = new BarSubItem();
      barSubItem.Caption = ((ActionItem) item).Caption;
      barSubItem.Name = ((HeaderItem) item).Key;
      barSubItem.LargeGlyph = item.LargeImage;
      this.ProcessSeperator(group.ItemLinks.Add((BarItem) barSubItem));
      ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.MenuContainerItem_PropertyChanged));
    }

    public override void Add(SimpleActionItem item)
    {
      Guard.ArgumentNotNull((object) item, nameof (item));
      BarButtonItem barButtonItem = RibbonHeader.CreateBarButtonItem(item);
      if (item.ShowInQuickAccessToolbar)
        this.AddItemToQuickAccess(barButtonItem);
      if (((GroupedItem) item).GroupCaption == "kApplicationMenu")
        this.AddItemToBackStage(barButtonItem);
      else if (((GroupedItem) item).GroupCaption == "kHeaderHelpItemKey")
      {
        this._ribbon.PageHeaderItemLinks.Add((BarItem) barButtonItem);
      }
      else
      {
        RibbonPageGroup group = RibbonHeader.GetOrCreateGroup(this.GetRibbonPage((GroupedItem) item), ((GroupedItem) item).GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));
        if (item.MenuContainerKey == null)
        {
          this.ProcessSeperator(group.ItemLinks.Add((BarItem) barButtonItem));
        }
        else
        {
          foreach (BarItemLink itemLink in (ReadOnlyCollectionBase) group.ItemLinks)
          {
            if (itemLink.Item.Name == item.MenuContainerKey && itemLink.Item is BarSubItem)
            {
              this.ProcessSeperator(((BarLinkContainerItem) itemLink.Item).AddItem((BarItem) barButtonItem));
              break;
            }
          }
        }
        this._ribbon.SelectedPage = (RibbonPage) null;
        ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.SimpleActionItem_PropertyChanged));
      }
    }

    private void AddItemToBackStage(BarButtonItem newItem)
    {
      ApplicationMenu buttonDropDownControl = this._ribbon.ApplicationButtonDropDownControl as ApplicationMenu;
      if (buttonDropDownControl.ItemLinks.Count == 0)
      {
        buttonDropDownControl.AddItem((BarItem) newItem);
      }
      else
      {
        buttonDropDownControl.AddItem((BarItem) newItem);
        List<BarItem> barItemList = new List<BarItem>(buttonDropDownControl.ItemLinks.Count);
        foreach (BarItemLink itemLink in (ReadOnlyCollectionBase) buttonDropDownControl.ItemLinks)
          barItemList.Add(itemLink.Item);
        barItemList.Sort((Comparison<BarItem>) ((x, y) =>
        {
          if (x == null)
            return y == null ? 0 : -1;
          if (y == null)
            return 1;
          return x.MergeOrder.CompareTo(y.MergeOrder);
        }));
        buttonDropDownControl.ItemLinks.Clear();
        buttonDropDownControl.ItemLinks.AddRange(barItemList.ToArray());
      }
    }

    private void AddItemToQuickAccess(BarButtonItem newItem)
    {
      this._ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Default;
      this._ribbon.ShowToolbarCustomizeItem = false;
      this._ribbon.Toolbar.ItemLinks.Add((BarItem) newItem);
    }

    private void ProcessSeperator(BarItemLink barItemLink)
    {
      if (!this._nextItemBeginsGroup)
        return;
      barItemLink.BeginGroup = this._nextItemBeginsGroup;
      this._nextItemBeginsGroup = false;
    }

    private void RootItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RootItem rootItem = sender as RootItem;
      RibbonPage existingRibbonPage = this.GetExistingRibbonPage(((HeaderItem) rootItem).Key);
      switch (e.PropertyName)
      {
        case "Caption":
          existingRibbonPage.Text = rootItem.Caption;
          break;
        case "Visible":
          existingRibbonPage.Visible = rootItem.Visible;
          break;
      }
    }

    private void MenuContainerItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      MenuContainerItem menuContainerItem = sender as MenuContainerItem;
      BarItem barItem = this.GetItem(((HeaderItem) menuContainerItem).Key);
      switch (e.PropertyName)
      {
        case "LargeImage":
          barItem.LargeGlyph = menuContainerItem.LargeImage;
          break;
        default:
          this.ActionItem_PropertyChanged((ActionItem) menuContainerItem, e);
          break;
      }
    }

    private void SimpleActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SimpleActionItem simpleActionItem = sender as SimpleActionItem;
      BarItem barItem = this.GetItem(((HeaderItem) simpleActionItem).Key);
      switch (e.PropertyName)
      {
        case "SmallImage":
          barItem.Glyph = simpleActionItem.SmallImage;
          break;
        case "LargeImage":
          barItem.LargeGlyph = simpleActionItem.LargeImage;
          break;
        case "MenuContainerKey":
          Trace.WriteLine("MenuContainerKey must not be changed after item is added to header.");
          break;
        case "ToggleGroupKey":
          Trace.WriteLine("ToggleGroupKey must not be changed after item is added to header.");
          break;
        default:
          this.ActionItem_PropertyChanged((ActionItem) simpleActionItem, e);
          break;
      }
    }

    private void DropDownActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      DropDownActionItem dropDownActionItem = sender as DropDownActionItem;
      BarEditItem barEditItem = this.GetItem(((HeaderItem) dropDownActionItem).Key) as BarEditItem;
      RepositoryItemComboBox edit = barEditItem.Edit as RepositoryItemComboBox;
      switch (e.PropertyName)
      {
        case "AllowEditingText":
          if (dropDownActionItem.AllowEditingText)
          {
            edit.TextEditStyle = TextEditStyles.Standard;
            break;
          }
          edit.TextEditStyle = TextEditStyles.DisableTextEditor;
          break;
        case "NullValuePrompt":
          edit.NullValuePrompt = dropDownActionItem.NullValuePrompt;
          break;
        case "Width":
          barEditItem.Width = dropDownActionItem.Width;
          break;
        case "SelectedItem":
          barEditItem.EditValue = dropDownActionItem.SelectedItem;
          break;
        case "FontColor":
          edit.Appearance.ForeColor = dropDownActionItem.FontColor;
          break;
        case "DisplayText":
          barEditItem.EditValue = (object) dropDownActionItem.DisplayText;
          break;
        case "MultiSelect":
          break;
        default:
          this.ActionItem_PropertyChanged((ActionItem) dropDownActionItem, e);
          break;
      }
    }

    private void ActionItem_PropertyChanged(ActionItem item, PropertyChangedEventArgs e)
    {
      BarItem guiItem = this.GetItem(((HeaderItem) item).Key);
      switch (e.PropertyName)
      {
        case "Caption":
          guiItem.Caption = item.Caption;
          break;
        case "Enabled":
          guiItem.Enabled = item.Enabled;
          break;
        case "Visible":
          RibbonHeader.SetVisibility(item, guiItem);
          break;
        case "ToolTipText":
          guiItem.ResetSuperTip();
          guiItem.SuperTip = new SuperToolTip();
          guiItem.SuperTip.Items.Add(item.ToolTipText);
          break;
        case "GroupCaption":
          Trace.WriteLine("GroupCaption must not be changed after item is added to header.");
          break;
        case "RootKey":
          Trace.WriteLine("RootKey must not be changed after item is added to header.");
          break;
        default:
          throw new NotSupportedException(" This Header Control implementation doesn't have an implementation for or has banned modifying that property.");
      }
    }

    private void TextEntryActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      TextEntryActionItem textEntryActionItem = sender as TextEntryActionItem;
      BarEditItem barEditItem = this.GetItem(((HeaderItem) textEntryActionItem).Key) as BarEditItem;
      switch (e.PropertyName)
      {
        case "Width":
          barEditItem.Width = textEntryActionItem.Width;
          break;
        case "Text":
          barEditItem.EditValue = (object) textEntryActionItem.Text;
          break;
        case "fontColor":
          barEditItem.Edit.Appearance.ForeColor = textEntryActionItem.FontColor;
          break;
        default:
          this.ActionItem_PropertyChanged((ActionItem) textEntryActionItem, e);
          break;
      }
    }

    public override void Add(DropDownActionItem item)
    {
      Guard.ArgumentNotNull((object) item, nameof (item));
      this.ProcessSeperator(RibbonHeader.GetOrCreateGroup(this.GetRibbonPage((GroupedItem) item), ((GroupedItem) item).GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly())).ItemLinks.Add((BarItem) this.CreateBarEditItem(item)));
      ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.DropDownActionItem_PropertyChanged));
    }

    public override void Add(SeparatorItem item)
    {
      Guard.ArgumentNotNull((object) item, nameof (item));
      this._nextItemBeginsGroup = true;
    }

    public override void Add(TextEntryActionItem item)
    {
      Guard.ArgumentNotNull((object) item, nameof (item));
      this.ProcessSeperator(RibbonHeader.GetOrCreateGroup(this.GetRibbonPage((GroupedItem) item), ((GroupedItem) item).GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly())).ItemLinks.Add((BarItem) this.CreateBarEditItem(item)));
      ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.TextEntryActionItem_PropertyChanged));
    }

    public string GetProductName(Assembly assembly)
    {
      Guard.ArgumentNotNull((object) assembly, nameof (assembly));
      object[] customAttributes = assembly.GetCustomAttributes(typeof (AssemblyProductAttribute), false);
      if (customAttributes.Length != 0)
        return ((AssemblyProductAttribute) customAttributes[0]).Product;
      return string.Empty;
    }

    public override void Remove(string key)
    {
      foreach (BarItem barItem in (CollectionBase) this._ribbon.Items)
      {
        if (barItem.Name == key)
        {
          RibbonPageGroupItemLinkCollection links = barItem.Links[0].Links as RibbonPageGroupItemLinkCollection;
          if (links == null)
          {
            barItem.Dispose();
            break;
          }
          RibbonPageGroup pageGroup = links.PageGroup;
          RibbonPage page = pageGroup.Page;
          this._ribbon.Items.Remove(barItem);
          if (pageGroup.ItemLinks.Count == 0)
          {
            page.Groups.Remove(pageGroup);
            if (page.Groups.Count == 0)
            {
              this._ribbon.Pages.Remove(page);
              break;
            }
            break;
          }
          break;
        }
      }
      base.Remove(key);
    }

    private static void SetVisibility(ActionItem item, BarItem guiItem)
    {
      RibbonPageGroupItemLinkCollection itemLinkCollection = (RibbonPageGroupItemLinkCollection) null;
      if (guiItem.Links.Count > 0)
        itemLinkCollection = guiItem.Links[0].Links as RibbonPageGroupItemLinkCollection;
      if (item.Visible)
      {
        guiItem.Visibility = BarItemVisibility.Always;
        if (itemLinkCollection == null)
          return;
        RibbonPageGroup pageGroup = itemLinkCollection.PageGroup;
        RibbonPage page = pageGroup.Page;
        foreach (BarItemLink barItemLink in (ReadOnlyCollectionBase) itemLinkCollection)
        {
          if (barItemLink.Item != guiItem && barItemLink.Item.Visibility != BarItemVisibility.Always)
            return;
        }
        pageGroup.Visible = true;
      }
      else
      {
        guiItem.Visibility = BarItemVisibility.OnlyInCustomizing;
        if (itemLinkCollection == null)
          return;
        RibbonPageGroup pageGroup = itemLinkCollection.PageGroup;
        RibbonPage page = pageGroup.Page;
        foreach (BarItemLink barItemLink in (ReadOnlyCollectionBase) itemLinkCollection)
        {
          if (barItemLink.Item != guiItem && barItemLink.Item.Visibility == BarItemVisibility.Always)
            return;
        }
        pageGroup.Visible = false;
      }
    }

    private static BarButtonItem CreateBarButtonItem(SimpleActionItem item)
    {
      BarButtonItem newItem = new BarButtonItem();
      newItem.Caption = ((ActionItem) item).Caption;
      newItem.Name = ((HeaderItem) item).Key;
      newItem.Glyph = item.SmallImage;
      newItem.LargeGlyph = item.LargeImage;
      if (((ActionItem) item).ToolTipText != null)
      {
        newItem.SuperTip = new SuperToolTip();
        newItem.SuperTip.Items.Add(((ActionItem) item).ToolTipText);
      }
      newItem.MergeOrder = (int) item.SortOrder;
      newItem.Enabled = ((ActionItem) item).Enabled;
      RibbonHeader.SetVisibility((ActionItem) item, (BarItem) newItem);
      newItem.ItemClick += (ItemClickEventHandler) ((sender, e) => item.OnClick((EventArgs) e));
      if (item.ToggleGroupKey != null)
      {
        int num = RibbonHeader.groups.IndexOf(item.ToggleGroupKey);
        if (num == -1)
        {
          RibbonHeader.groups.Add(item.ToggleGroupKey);
          num = RibbonHeader.groups.IndexOf(item.ToggleGroupKey);
          newItem.AllowAllUp = true;
        }
        else
          newItem.AllowAllUp = false;
        newItem.ButtonStyle = BarButtonStyle.Check;
        newItem.GroupIndex = num + 1;
        item.Toggling += ((EventHandler) ((sender, e) => newItem.Toggle()));
      }
      return newItem;
    }

    private static RibbonPageGroup GetOrCreateGroup(RibbonPage page, string groupCaption)
    {
      if (groupCaption == null)
        groupCaption = "Default Group";
      RibbonPageGroup ribbonPageGroup = page.Groups[groupCaption];
      if (ribbonPageGroup == null)
      {
        RibbonPageGroup group = new RibbonPageGroup();
        group.Name = groupCaption;
        group.Text = groupCaption;
        group.ShowCaptionButton = false;
        page.Groups.Add(group);
        ribbonPageGroup = group;
      }
      return ribbonPageGroup;
    }

    private BarEditItem CreateBarEditItem(DropDownActionItem item)
    {
      BarEditItem barEditItem = new BarEditItem();
      barEditItem.Name = ((HeaderItem) item).Key;
      barEditItem.Caption = ((ActionItem) item).Caption;
      RepositoryItemComboBox repositoryItemComboBox = new RepositoryItemComboBox();
      barEditItem.Edit = (RepositoryItem) repositoryItemComboBox;
      repositoryItemComboBox.Items.AddRange((ICollection) item.Items);
      repositoryItemComboBox.NullValuePromptShowForEmptyValue = true;
      repositoryItemComboBox.Click += (EventHandler) ((sender, e) =>
      {
        if (!item.MultiSelect)
          return;
        item.SelectedItem = (object)null;
        item.MultiSelect = false;
      });
      repositoryItemComboBox.CustomDisplayText += (CustomDisplayTextEventHandler) ((sender, e) =>
      {
        if (item.MultiSelect)
        {
          e.DisplayText = "Multiple Selected";
          barEditItem.Edit.Appearance.ForeColor = Color.Gray;
        }
        else
          barEditItem.Edit.Appearance.ForeColor = Color.Black;
      });
      if (item.AllowEditingText)
      {
        repositoryItemComboBox.ImmediatePopup = true;
        repositoryItemComboBox.DropDownRows = 10;
      }
      if (!item.AllowEditingText)
        repositoryItemComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
      if (item.Width != 0)
        barEditItem.Width = item.Width;
      repositoryItemComboBox.NullValuePrompt = item.NullValuePrompt;
      repositoryItemComboBox.SelectedValueChanged += (EventHandler) ((sender, e) =>
      {
        ComboBoxEdit comboBoxEdit = sender as ComboBoxEdit;
        ((HeaderItem) item).PropertyChanged -= (new PropertyChangedEventHandler(this.DropDownActionItem_PropertyChanged));
        item.SelectedItem = comboBoxEdit.SelectedItem;
        ((HeaderItem) item).PropertyChanged +=(new PropertyChangedEventHandler(this.DropDownActionItem_PropertyChanged));
      });
      return barEditItem;
    }

    private BarEditItem CreateBarEditItem(TextEntryActionItem item)
    {
      BarEditItem barEditItem = new BarEditItem();
      barEditItem.Name = ((HeaderItem) item).Key;
      barEditItem.Caption = ((ActionItem) item).Caption;
      RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
      barEditItem.Edit = (RepositoryItem) repositoryItemTextEdit;
      if (item.Width != 0)
        barEditItem.Width = item.Width;
      if (!((ActionItem) item).Enabled)
        barEditItem.Enabled = false;
      repositoryItemTextEdit.Enter += (EventHandler) ((sender, e) => ((BaseEdit) sender).SelectAll());
      repositoryItemTextEdit.EditValueChanged += (EventHandler) ((sender, e) =>
      {
        TextEdit textEdit = sender as TextEdit;
        ((HeaderItem) item).PropertyChanged -= (new PropertyChangedEventHandler(this.TextEntryActionItem_PropertyChanged));
        item.Text = textEdit.Text;
        ((HeaderItem) item).PropertyChanged += (new PropertyChangedEventHandler(this.TextEntryActionItem_PropertyChanged));
      });
      return barEditItem;
    }

    private void EnsureExtensionsTabExists()
    {
      if (this.GetExistingRibbonPage("kExtensions") != null)
        return;
      Add(new RootItem("kExtensions", "Extensions"));
    }

    private void EnsureNonNullRoot(GroupedItem item)
    {
      if (item.RootKey != null)
        return;
      this.EnsureExtensionsTabExists();
      item.RootKey = "kExtensions";
    }

    private RibbonPage GetExistingRibbonPage(string key)
    {
      foreach (RibbonPage page in (CollectionBase) this._ribbon.Pages)
      {
        if (page.Name == key)
          return page;
      }
      return (RibbonPage) null;
    }

    private BarItem GetItem(string key)
    {
      foreach (BarItem barItem in (CollectionBase) this._ribbon.Items)
      {
        if (barItem.Name == key)
          return barItem;
      }
      return (BarItem) null;
    }

    private RibbonPage GetRibbonPage(GroupedItem item)
    {
      this.EnsureNonNullRoot(item);
      RibbonPage existingRibbonPage = this.GetExistingRibbonPage(item.RootKey);
      if (existingRibbonPage != null || !RibbonHeader.ShouldCreateRibbonPage(item.RootKey))
        return existingRibbonPage;
      RibbonPage page = new RibbonPage();
      page.Name = item.RootKey;
      this._ribbon.Pages.Add(page);
      return page;
    }

    public override void SelectRoot(string key)
    {
      this._ribbon.SelectedPage = this.GetExistingRibbonPage(key);
    }

    public void OnImportsSatisfied()
    {
      this._ribbon = new RibbonControl();
      this.backstageMenu = new ApplicationMenu();
      this.ribbonStatusBar = new RibbonStatusBar();
      this.defaultStatusPanel = new BarStaticItem();
      this.defaultProgressPanel = new BarEditItem();
      this.defaultProgressBar = new RepositoryItemProgressBar();
      this._ribbon.BeginInit();
      this.backstageMenu.BeginInit();
      this.defaultProgressBar.BeginInit();
      this.Shell.SuspendLayout();
      this._ribbon.ApplicationButtonDropDownControl = (object) this.backstageMenu;
      this._ribbon.ApplicationButtonText = "";
      this._ribbon.ExpandCollapseItem.Id = 0;
      this._ribbon.ExpandCollapseItem.Name = "";
      this._ribbon.Items.AddRange(new BarItem[3]
      {
        (BarItem) this._ribbon.ExpandCollapseItem,
        (BarItem) this.defaultStatusPanel,
        (BarItem) this.defaultProgressPanel
      });
      this._ribbon.Location = new Point(0, 0);
      this._ribbon.MaxItemId = 9;
      this._ribbon.AutoSizeItems = true;
      this._ribbon.Name = "ribbon";
      this._ribbon.RepositoryItems.AddRange(new RepositoryItem[1]
      {
        (RepositoryItem) this.defaultProgressBar
      });
      this._ribbon.RibbonStyle = RibbonControlStyle.Office2010;
      this._ribbon.Size = new Size(790, 50);
      this._ribbon.StatusBar = this.ribbonStatusBar;
      this._ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Hidden;
      this._ribbon.ShowApplicationButton = DefaultBoolean.False;
      this._ribbon.SelectedPageChanged += new EventHandler(this.Ribbon_SelectedPageChanged);
      this.backstageMenu.Name = "backstageMenu";
      this.backstageMenu.Ribbon = this._ribbon;
      this.ribbonStatusBar.ItemLinks.Add((BarItem) this.defaultStatusPanel);
      this.ribbonStatusBar.ItemLinks.Add((BarItem) this.defaultProgressPanel);
      this.ribbonStatusBar.Location = new Point(0, 468);
      this.ribbonStatusBar.Name = "ribbonStatusBar";
      this.ribbonStatusBar.Ribbon = this._ribbon;
      this.ribbonStatusBar.Size = new Size(790, 31);
      this.defaultStatusPanel.Caption = "初始化完成";
      this.defaultStatusPanel.Id = 4;
      this.defaultStatusPanel.Name = "defaultStatusPanel";
      this.defaultStatusPanel.TextAlignment = StringAlignment.Near;
      this.defaultProgressPanel.Alignment = BarItemLinkAlignment.Right;
      this.defaultProgressPanel.Edit = (RepositoryItem) this.defaultProgressBar;
      this.defaultProgressPanel.EditValue = (object) "50";
      this.defaultProgressPanel.Id = 5;
      this.defaultProgressPanel.Name = "defaultProgressPanel";
      this.defaultProgressPanel.Width = 100;
      this.defaultProgressPanel.Visibility = BarItemVisibility.Never;
      this.defaultProgressBar.Name = "defaultProgressBar";
      this.Shell.Controls.Add((Control) this.ribbonStatusBar);
      this.Shell.Controls.Add((Control) this._ribbon);
      RibbonForm shell = this.Shell as RibbonForm;
      if (shell != null)
      {
        shell.Ribbon = this._ribbon;
        shell.StatusBar = this.ribbonStatusBar;
      }
      this._ribbon.EndInit();
      this.backstageMenu.EndInit();
      this.defaultProgressBar.EndInit();
      this.Shell.ResumeLayout(false);
    }

    private void Ribbon_SelectedPageChanged(object sender, EventArgs e)
    {
      if (this._ribbon == null || this._ribbon.SelectedPage == null)
        return;
      this.OnRootItemSelected(this._ribbon.SelectedPage.Name);
    }

    public void Progress(string key, int percent, string message)
    {
      this.defaultStatusPanel.Caption = message;
      this.defaultProgressPanel.EditValue = (object) percent;
      if (percent > 0)
        this.defaultProgressPanel.Visibility = BarItemVisibility.Always;
      else
        this.defaultProgressPanel.Visibility = BarItemVisibility.Never;
      //if (this.stopwatch.ElapsedMilliseconds <= 200L)
      //  return;
      if (this.ribbonStatusBar.InvokeRequired)
        this.ribbonStatusBar.BeginInvoke(new MethodInvoker (() => this.defaultStatusPanel.Refresh()));
      else
        this.defaultStatusPanel.Refresh();
      //this.stopwatch.Restart();
    }

    public void Add(StatusPanel panel)
    {
      BarStaticItem statusPanel = new BarStaticItem();
      statusPanel.Name = panel.Key;
      statusPanel.Caption = panel.Caption;
      statusPanel.Width = panel.Width;
      this._ribbon.Items.Add((BarItem) statusPanel);
      this.ribbonStatusBar.ItemLinks.Insert(0, (BarItem) statusPanel);
      panel.PropertyChanged += ((PropertyChangedEventHandler) ((sender, e) =>
      {
        switch (e.PropertyName)
        {
          case "Caption":
            statusPanel.Caption = panel.Caption;
            break;
        }
      }));
    }

    public void Remove(StatusPanel panel)
    {
      foreach (BarItemLink itemLink in (ReadOnlyCollectionBase) this.ribbonStatusBar.ItemLinks)
      {
        if (itemLink.Item.Name == panel.Key)
        {
          this._ribbon.Items.Remove(itemLink.Item);
          this.ribbonStatusBar.ItemLinks.Remove(itemLink);
          break;
        }
      }
    }

    private static bool ShouldCreateRibbonPage(string key)
    {
      return !(key == "kApplicationMenu") && !(key == "kHeaderHelpItemKey");
    }
  }
}
