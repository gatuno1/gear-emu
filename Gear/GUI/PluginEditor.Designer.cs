/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * --------------------------------------------------------------------------------
 */

namespace Gear.GUI
{
    partial class PluginEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            this.defaultFont.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripLabel classNameLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginEditor));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Author", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Modified By", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Date Modified", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Description", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Usage", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Links", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Your Name");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Your Name");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Modified");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Description for the plugin");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("How to use the plugin");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Web Link to more information");
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.saveAsButton = new System.Windows.Forms.ToolStripButton();
            this.checkButton = new System.Windows.Forms.ToolStripButton();
            this.instanceName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.syntaxButton = new System.Windows.Forms.ToolStripButton();
            this.referenceStrip = new System.Windows.Forms.Panel();
            this.referencesList = new System.Windows.Forms.ListBox();
            this.toolStripReferences = new System.Windows.Forms.ToolStrip();
            this.referenceName = new System.Windows.Forms.ToolStripTextBox();
            this.addReferenceButton = new System.Windows.Forms.ToolStripButton();
            this.removeReferenceButton = new System.Windows.Forms.ToolStripButton();
            this.errorListView = new System.Windows.Forms.ListView();
            this.codeEditorView = new System.Windows.Forms.RichTextBox();
            this.errorSplitter = new Gear.GUI.CollapsibleSplitter();
            this.referencesSplitter = new Gear.GUI.CollapsibleSplitter();
            this.AboutSplitter = new Gear.GUI.CollapsibleSplitter();
            this.panelAbout = new System.Windows.Forms.Panel();
            this.listProperties = new System.Windows.Forms.ListView();
            this.keyColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripLinks = new System.Windows.Forms.ToolStrip();
            this.linkName = new System.Windows.Forms.ToolStripTextBox();
            this.addLinkButton = new System.Windows.Forms.ToolStripButton();
            this.removeLinkButton = new System.Windows.Forms.ToolStripButton();
            this.panelDetails = new System.Windows.Forms.Panel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            classNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripMain.SuspendLayout();
            this.referenceStrip.SuspendLayout();
            this.toolStripReferences.SuspendLayout();
            this.panelAbout.SuspendLayout();
            this.toolStripLinks.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // classNameLabel
            // 
            classNameLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            classNameLabel.Name = "classNameLabel";
            classNameLabel.Size = new System.Drawing.Size(69, 22);
            classNameLabel.Text = "Class Name";
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.saveButton,
            this.saveAsButton,
            toolStripSeparator1,
            this.checkButton,
            this.instanceName,
            classNameLabel,
            this.toolStripSeparator2,
            this.syntaxButton});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(634, 25);
            this.toolStripMain.TabIndex = 0;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(40, 22);
            this.openButton.Text = "Open";
            this.openButton.ToolTipText = "Open plugin";
            this.openButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(35, 22);
            this.saveButton.Text = "Save";
            this.saveButton.ToolTipText = "Save plugin";
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveAsButton.Image = ((System.Drawing.Image)(resources.GetObject("saveAsButton.Image")));
            this.saveAsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(57, 22);
            this.saveAsButton.Text = "Save As..";
            this.saveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
            // 
            // checkButton
            // 
            this.checkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.checkButton.Image = ((System.Drawing.Image)(resources.GetObject("checkButton.Image")));
            this.checkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(75, 22);
            this.checkButton.Text = "Check Code";
            this.checkButton.ToolTipText = "Check code for errors";
            this.checkButton.Click += new System.EventHandler(this.CheckSource_Click);
            // 
            // instanceName
            // 
            this.instanceName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.instanceName.Name = "instanceName";
            this.instanceName.Size = new System.Drawing.Size(100, 25);
            this.instanceName.ToolTipText = "Name of the Class for the plugin\r\nMust be the same as the class inherited from Pl" +
    "uginBase.";
            this.instanceName.Leave += new System.EventHandler(this.instanceName_Leave);
            this.instanceName.TextChanged += new System.EventHandler(this.instanceName_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // syntaxButton
            // 
            this.syntaxButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.syntaxButton.Image = ((System.Drawing.Image)(resources.GetObject("syntaxButton.Image")));
            this.syntaxButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.syntaxButton.Name = "syntaxButton";
            this.syntaxButton.Size = new System.Drawing.Size(95, 22);
            this.syntaxButton.Text = "Sintax Highlight";
            this.syntaxButton.ToolTipText = "Sintax Highlight the code";
            this.syntaxButton.Click += new System.EventHandler(this.syntaxButton_Click);
            // 
            // referenceStrip
            // 
            this.referenceStrip.Controls.Add(this.referencesList);
            this.referenceStrip.Controls.Add(this.toolStripReferences);
            this.referenceStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.referenceStrip.Location = new System.Drawing.Point(0, 337);
            this.referenceStrip.Name = "referenceStrip";
            this.referenceStrip.Size = new System.Drawing.Size(200, 80);
            this.referenceStrip.TabIndex = 1;
            // 
            // referencesList
            // 
            this.referencesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referencesList.FormattingEnabled = true;
            this.referencesList.Location = new System.Drawing.Point(0, 0);
            this.referencesList.Name = "referencesList";
            this.referencesList.Size = new System.Drawing.Size(200, 57);
            this.referencesList.TabIndex = 1;
            // 
            // toolStripReferences
            // 
            this.toolStripReferences.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripReferences.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.referenceName,
            this.addReferenceButton,
            this.removeReferenceButton});
            this.toolStripReferences.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripReferences.Location = new System.Drawing.Point(0, 57);
            this.toolStripReferences.Name = "toolStripReferences";
            this.toolStripReferences.Size = new System.Drawing.Size(200, 23);
            this.toolStripReferences.TabIndex = 0;
            this.toolStripReferences.Text = "toolStrip2";
            // 
            // referenceName
            // 
            this.referenceName.Name = "referenceName";
            this.referenceName.Size = new System.Drawing.Size(100, 23);
            this.referenceName.ToolTipText = "Reference Name to add";
            // 
            // addReferenceButton
            // 
            this.addReferenceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addReferenceButton.Image = ((System.Drawing.Image)(resources.GetObject("addReferenceButton.Image")));
            this.addReferenceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addReferenceButton.Name = "addReferenceButton";
            this.addReferenceButton.Size = new System.Drawing.Size(33, 19);
            this.addReferenceButton.Text = "Add";
            this.addReferenceButton.ToolTipText = "Add a new Reference";
            this.addReferenceButton.Click += new System.EventHandler(this.addReferenceButton_Click);
            // 
            // removeReferenceButton
            // 
            this.removeReferenceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeReferenceButton.Image = ((System.Drawing.Image)(resources.GetObject("removeReferenceButton.Image")));
            this.removeReferenceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeReferenceButton.Name = "removeReferenceButton";
            this.removeReferenceButton.Size = new System.Drawing.Size(54, 19);
            this.removeReferenceButton.Text = "Remove";
            this.removeReferenceButton.ToolTipText = "Remove selected Reference";
            this.removeReferenceButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // errorListView
            // 
            this.errorListView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorListView.Location = new System.Drawing.Point(208, 345);
            this.errorListView.MultiSelect = false;
            this.errorListView.Name = "errorListView";
            this.errorListView.ShowItemToolTips = true;
            this.errorListView.Size = new System.Drawing.Size(426, 97);
            this.errorListView.TabIndex = 5;
            this.errorListView.UseCompatibleStateImageBehavior = false;
            this.errorListView.View = System.Windows.Forms.View.Details;
            this.errorListView.ItemActivate += new System.EventHandler(this.ErrorView_SelectedIndexChanged);
            this.errorListView.SelectedIndexChanged += new System.EventHandler(this.ErrorView_SelectedIndexChanged);
            // 
            // codeEditorView
            // 
            this.codeEditorView.AcceptsTab = true;
            this.codeEditorView.DetectUrls = false;
            this.codeEditorView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditorView.HideSelection = false;
            this.codeEditorView.Location = new System.Drawing.Point(208, 25);
            this.codeEditorView.Name = "codeEditorView";
            this.codeEditorView.Size = new System.Drawing.Size(426, 312);
            this.codeEditorView.TabIndex = 7;
            this.codeEditorView.Text = "";
            this.codeEditorView.WordWrap = false;
            this.codeEditorView.TextChanged += new System.EventHandler(this.codeEditorView_TextChanged);
            // 
            // errorSplitter
            // 
            this.errorSplitter.AnimationDelay = 20;
            this.errorSplitter.AnimationStep = 20;
            this.errorSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.errorSplitter.ControlToHide = this.errorListView;
            this.errorSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.errorSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorSplitter.ExpandParentForm = false;
            this.errorSplitter.Location = new System.Drawing.Point(208, 337);
            this.errorSplitter.Name = "collapsibleSplitter2";
            this.errorSplitter.TabIndex = 6;
            this.errorSplitter.TabStop = false;
            this.errorSplitter.UseAnimations = false;
            this.errorSplitter.VisualStyle = Gear.GUI.VisualStyles.Mozilla;
            // 
            // referencesSplitter
            // 
            this.referencesSplitter.AnimationDelay = 20;
            this.referencesSplitter.AnimationStep = 20;
            this.referencesSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.referencesSplitter.ControlToHide = this.referenceStrip;
            this.referencesSplitter.ExpandParentForm = false;
            this.referencesSplitter.Location = new System.Drawing.Point(200, 25);
            this.referencesSplitter.Name = "collapsibleSplitter1";
            this.referencesSplitter.TabIndex = 3;
            this.referencesSplitter.TabStop = false;
            this.referencesSplitter.UseAnimations = false;
            this.referencesSplitter.VisualStyle = Gear.GUI.VisualStyles.Mozilla;
            // 
            // AboutSplitter
            // 
            this.AboutSplitter.AnimationDelay = 20;
            this.AboutSplitter.AnimationStep = 20;
            this.AboutSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.AboutSplitter.ControlToHide = this.panelAbout;
            this.AboutSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.AboutSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.AboutSplitter.ExpandParentForm = false;
            this.AboutSplitter.Location = new System.Drawing.Point(0, 329);
            this.AboutSplitter.Name = "AboutSplitter";
            this.AboutSplitter.TabIndex = 2;
            this.AboutSplitter.TabStop = false;
            this.AboutSplitter.UseAnimations = false;
            this.AboutSplitter.VisualStyle = Gear.GUI.VisualStyles.Mozilla;
            // 
            // panelAbout
            // 
            this.panelAbout.Controls.Add(this.listProperties);
            this.panelAbout.Controls.Add(this.toolStripLinks);
            this.panelAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAbout.Location = new System.Drawing.Point(0, 0);
            this.panelAbout.Name = "panelAbout";
            this.panelAbout.Size = new System.Drawing.Size(200, 329);
            this.panelAbout.TabIndex = 4;
            // 
            // listProperties
            // 
            this.listProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.keyColumn});
            this.listProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProperties.FullRowSelect = true;
            this.listProperties.GridLines = true;
            listViewGroup1.Header = "Author";
            listViewGroup1.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup1.Name = "Author";
            listViewGroup2.Header = "Modified By";
            listViewGroup2.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup2.Name = "ModifiedBy";
            listViewGroup3.Header = "Date Modified";
            listViewGroup3.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup3.Name = "DateModified";
            listViewGroup4.Header = "Description";
            listViewGroup4.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup4.Name = "Description";
            listViewGroup5.Header = "Usage";
            listViewGroup5.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup5.Name = "Usage";
            listViewGroup6.Header = "Links";
            listViewGroup6.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup6.Name = "Links";
            this.listProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6});
            this.listProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listProperties.HideSelection = false;
            listViewItem1.Checked = true;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.StateImageIndex = 1;
            listViewItem1.ToolTipText = "The name of original author of the plugin.";
            listViewItem2.Group = listViewGroup2;
            listViewItem2.ToolTipText = "The name of the last modifier.";
            listViewItem3.Group = listViewGroup3;
            listViewItem3.ToolTipText = "When was made the last modification.";
            listViewItem4.Group = listViewGroup4;
            listViewItem4.ToolTipText = "What does the plugin.";
            listViewItem5.Group = listViewGroup5;
            listViewItem5.ToolTipText = "How it is supposed to be used the plugin.";
            listViewItem6.Group = listViewGroup6;
            listViewItem6.ToolTipText = "Web links for the plugin.";
            this.listProperties.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listProperties.LabelEdit = true;
            this.listProperties.Location = new System.Drawing.Point(0, 0);
            this.listProperties.MultiSelect = false;
            this.listProperties.Name = "listProperties";
            this.listProperties.ShowItemToolTips = true;
            this.listProperties.Size = new System.Drawing.Size(200, 304);
            this.listProperties.TabIndex = 3;
            this.listProperties.UseCompatibleStateImageBehavior = false;
            this.listProperties.View = System.Windows.Forms.View.Details;
            // 
            // keyColumn
            // 
            this.keyColumn.Text = "Properties of plugin v.x";
            this.keyColumn.Width = 195;
            // 
            // toolStripLinks
            // 
            this.toolStripLinks.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripLinks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linkName,
            this.addLinkButton,
            this.removeLinkButton});
            this.toolStripLinks.Location = new System.Drawing.Point(0, 304);
            this.toolStripLinks.Name = "toolStripLinks";
            this.toolStripLinks.Size = new System.Drawing.Size(200, 25);
            this.toolStripLinks.TabIndex = 4;
            this.toolStripLinks.Text = "toolStrip1";
            // 
            // linkName
            // 
            this.linkName.Name = "linkName";
            this.linkName.Size = new System.Drawing.Size(80, 25);
            this.linkName.ToolTipText = "Link Name to add";
            // 
            // addLinkButton
            // 
            this.addLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addLinkButton.Image = ((System.Drawing.Image)(resources.GetObject("addLinkButton.Image")));
            this.addLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addLinkButton.Name = "addLinkButton";
            this.addLinkButton.Size = new System.Drawing.Size(33, 22);
            this.addLinkButton.Text = "Add";
            this.addLinkButton.ToolTipText = "Add a new Link";
            // 
            // removeLinkButton
            // 
            this.removeLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeLinkButton.Image = ((System.Drawing.Image)(resources.GetObject("removeLinkButton.Image")));
            this.removeLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeLinkButton.Name = "removeLinkButton";
            this.removeLinkButton.Size = new System.Drawing.Size(54, 22);
            this.removeLinkButton.Text = "Remove";
            this.removeLinkButton.ToolTipText = "Remove selected Link";
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.panelAbout);
            this.panelDetails.Controls.Add(this.AboutSplitter);
            this.panelDetails.Controls.Add(this.referenceStrip);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelDetails.Location = new System.Drawing.Point(0, 25);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(200, 417);
            this.panelDetails.TabIndex = 2;
            // 
            // PluginEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 442);
            this.Controls.Add(this.codeEditorView);
            this.Controls.Add(this.errorSplitter);
            this.Controls.Add(this.errorListView);
            this.Controls.Add(this.referencesSplitter);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.toolStripMain);
            this.Name = "PluginEditor";
            this.Text = "Plugin Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginEditor_FormClosing);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.referenceStrip.ResumeLayout(false);
            this.referenceStrip.PerformLayout();
            this.toolStripReferences.ResumeLayout(false);
            this.toolStripReferences.PerformLayout();
            this.panelAbout.ResumeLayout(false);
            this.panelAbout.PerformLayout();
            this.toolStripLinks.ResumeLayout(false);
            this.toolStripLinks.PerformLayout();
            this.panelDetails.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton checkButton;
        private System.Windows.Forms.Panel referenceStrip;
        private System.Windows.Forms.ListBox referencesList;
        private System.Windows.Forms.ToolStrip toolStripReferences;
        private System.Windows.Forms.ToolStripTextBox referenceName;
        private System.Windows.Forms.ToolStripButton addReferenceButton;
        private CollapsibleSplitter referencesSplitter;
        private System.Windows.Forms.ToolStripTextBox instanceName;
        private System.Windows.Forms.ToolStripButton saveAsButton;
        private System.Windows.Forms.ToolStripButton removeReferenceButton;
        private System.Windows.Forms.ListView errorListView;
        private CollapsibleSplitter errorSplitter;
        private System.Windows.Forms.RichTextBox codeEditorView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton syntaxButton;
        private CollapsibleSplitter AboutSplitter;
        private System.Windows.Forms.Panel panelAbout;
        private System.Windows.Forms.ListView listProperties;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.ToolStrip toolStripLinks;
        private System.Windows.Forms.ToolStripTextBox linkName;
        private System.Windows.Forms.ToolStripButton addLinkButton;
        private System.Windows.Forms.ToolStripButton removeLinkButton;
        private System.Windows.Forms.ColumnHeader keyColumn;
    }
}
