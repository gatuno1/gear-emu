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
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Author", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Modified By", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Date Modified", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("Description", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("Usage", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("Links", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Your name");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Your name");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Modified");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Description for the plugin");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("How to use the plugin");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Web Link to more information");
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.saveAsButton = new System.Windows.Forms.ToolStripButton();
            this.checkButton = new System.Windows.Forms.ToolStripButton();
            this.instanceName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.syntaxButton = new System.Windows.Forms.ToolStripButton();
            this.referencesList = new System.Windows.Forms.ListBox();
            this.toolStripReferences = new System.Windows.Forms.ToolStrip();
            this.referenceName = new System.Windows.Forms.ToolStripTextBox();
            this.addReferenceButton = new System.Windows.Forms.ToolStripButton();
            this.removeReferenceButton = new System.Windows.Forms.ToolStripButton();
            this.errorListView = new System.Windows.Forms.ListView();
            this.codeEditorView = new System.Windows.Forms.RichTextBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerPropRef = new System.Windows.Forms.SplitContainer();
            this.toolStripLinks = new System.Windows.Forms.ToolStrip();
            this.linkName = new System.Windows.Forms.ToolStripTextBox();
            this.addLinkButton = new System.Windows.Forms.ToolStripButton();
            this.removeLinkButton = new System.Windows.Forms.ToolStripButton();
            this.listProperties = new System.Windows.Forms.ListView();
            this.KeyColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainerCodeErr = new System.Windows.Forms.SplitContainer();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            classNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripMain.SuspendLayout();
            this.toolStripReferences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPropRef)).BeginInit();
            this.splitContainerPropRef.Panel1.SuspendLayout();
            this.splitContainerPropRef.Panel2.SuspendLayout();
            this.splitContainerPropRef.SuspendLayout();
            this.toolStripLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCodeErr)).BeginInit();
            this.splitContainerCodeErr.Panel1.SuspendLayout();
            this.splitContainerCodeErr.Panel2.SuspendLayout();
            this.splitContainerCodeErr.SuspendLayout();
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
            // referencesList
            // 
            this.referencesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referencesList.FormattingEnabled = true;
            this.referencesList.Location = new System.Drawing.Point(0, 0);
            this.referencesList.Name = "referencesList";
            this.referencesList.Size = new System.Drawing.Size(198, 80);
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
            this.toolStripReferences.Location = new System.Drawing.Point(0, 80);
            this.toolStripReferences.Name = "toolStripReferences";
            this.toolStripReferences.Size = new System.Drawing.Size(198, 23);
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
            this.errorListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorListView.Location = new System.Drawing.Point(0, 0);
            this.errorListView.MultiSelect = false;
            this.errorListView.Name = "errorListView";
            this.errorListView.ShowItemToolTips = true;
            this.errorListView.Size = new System.Drawing.Size(432, 103);
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
            this.codeEditorView.Location = new System.Drawing.Point(0, 0);
            this.codeEditorView.Name = "codeEditorView";
            this.codeEditorView.Size = new System.Drawing.Size(432, 310);
            this.codeEditorView.TabIndex = 7;
            this.codeEditorView.Text = "";
            this.codeEditorView.WordWrap = false;
            this.codeEditorView.TextChanged += new System.EventHandler(this.codeEditorView_TextChanged);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 25);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerPropRef);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerCodeErr);
            this.splitContainerMain.Size = new System.Drawing.Size(634, 417);
            this.splitContainerMain.SplitterDistance = 198;
            this.splitContainerMain.TabIndex = 8;
            // 
            // splitContainerPropRef
            // 
            this.splitContainerPropRef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPropRef.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPropRef.Name = "splitContainerPropRef";
            this.splitContainerPropRef.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPropRef.Panel1
            // 
            this.splitContainerPropRef.Panel1.AutoScroll = true;
            this.splitContainerPropRef.Panel1.Controls.Add(this.listProperties);
            this.splitContainerPropRef.Panel1.Controls.Add(this.toolStripLinks);
            // 
            // splitContainerPropRef.Panel2
            // 
            this.splitContainerPropRef.Panel2.Controls.Add(this.referencesList);
            this.splitContainerPropRef.Panel2.Controls.Add(this.toolStripReferences);
            this.splitContainerPropRef.Size = new System.Drawing.Size(198, 417);
            this.splitContainerPropRef.SplitterDistance = 310;
            // 
            // toolStripLinks
            // 
            this.toolStripLinks.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripLinks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linkName,
            this.addLinkButton,
            this.removeLinkButton});
            this.toolStripLinks.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripLinks.Location = new System.Drawing.Point(0, 268);
            this.toolStripLinks.Name = "toolStripLinks";
            this.toolStripLinks.Size = new System.Drawing.Size(198, 42);
            this.toolStripLinks.TabIndex = 1;
            this.toolStripLinks.Text = "toolStrip2";
            // 
            // linkName
            // 
            this.linkName.Name = "linkName";
            this.linkName.Size = new System.Drawing.Size(100, 23);
            this.linkName.ToolTipText = "Link Name to add";
            // 
            // addLinkButton
            // 
            this.addLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addLinkButton.Image = ((System.Drawing.Image)(resources.GetObject("addLinkButton.Image")));
            this.addLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addLinkButton.Name = "addLinkButton";
            this.addLinkButton.Size = new System.Drawing.Size(33, 19);
            this.addLinkButton.Text = "Add";
            this.addLinkButton.ToolTipText = "Add a new Link";
            // 
            // removeLinkButton
            // 
            this.removeLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeLinkButton.Image = ((System.Drawing.Image)(resources.GetObject("removeLinkButton.Image")));
            this.removeLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeLinkButton.Name = "removeLinkButton";
            this.removeLinkButton.Size = new System.Drawing.Size(54, 19);
            this.removeLinkButton.Text = "Remove";
            this.removeLinkButton.ToolTipText = "Remove selected Link";
            // 
            // listProperties
            // 
            this.listProperties.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.KeyColumn});
            this.listProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProperties.FullRowSelect = true;
            this.listProperties.GridLines = true;
            listViewGroup7.Header = "Author";
            listViewGroup7.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup7.Name = "Author";
            listViewGroup8.Header = "Modified By";
            listViewGroup8.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup8.Name = "ModifiedBy";
            listViewGroup9.Header = "Date Modified";
            listViewGroup9.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup9.Name = "DateModified";
            listViewGroup10.Header = "Description";
            listViewGroup10.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup10.Name = "Description";
            listViewGroup11.Header = "Usage";
            listViewGroup11.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup11.Name = "Usage";
            listViewGroup12.Header = "Links";
            listViewGroup12.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup12.Name = "Links";
            this.listProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10,
            listViewGroup11,
            listViewGroup12});
            this.listProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listProperties.HideSelection = false;
            listViewItem7.Checked = true;
            listViewItem7.Group = listViewGroup7;
            listViewItem7.StateImageIndex = 1;
            listViewItem7.ToolTipText = "The name of original author of the plugin.";
            listViewItem8.Group = listViewGroup8;
            listViewItem8.ToolTipText = "The name of the last modifier.";
            listViewItem9.Group = listViewGroup9;
            listViewItem9.ToolTipText = "When was the last modification.";
            listViewItem10.Group = listViewGroup10;
            listViewItem10.ToolTipText = "What does the plugin.";
            listViewItem11.Group = listViewGroup11;
            listViewItem11.ToolTipText = "How it is supposed to be used the plugin.";
            listViewItem12.Group = listViewGroup12;
            listViewItem12.ToolTipText = "Web links for the plugin.";
            this.listProperties.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12});
            this.listProperties.LabelEdit = true;
            this.listProperties.Location = new System.Drawing.Point(0, 0);
            this.listProperties.MultiSelect = false;
            this.listProperties.Name = "listProperties";
            this.listProperties.ShowItemToolTips = true;
            this.listProperties.Size = new System.Drawing.Size(198, 310);
            this.listProperties.TabIndex = 0;
            this.listProperties.UseCompatibleStateImageBehavior = false;
            this.listProperties.View = System.Windows.Forms.View.Details;
            // 
            // KeyColumn
            // 
            this.KeyColumn.Text = "Properties of plugin v.x";
            this.KeyColumn.Width = 191;
            // 
            // splitContainerCodeErr
            // 
            this.splitContainerCodeErr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCodeErr.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCodeErr.Name = "splitContainerCodeErr";
            this.splitContainerCodeErr.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerCodeErr.Panel1
            // 
            this.splitContainerCodeErr.Panel1.Controls.Add(this.codeEditorView);
            // 
            // splitContainerCodeErr.Panel2
            // 
            this.splitContainerCodeErr.Panel2.Controls.Add(this.errorListView);
            this.splitContainerCodeErr.Size = new System.Drawing.Size(432, 417);
            this.splitContainerCodeErr.SplitterDistance = 310;
            this.splitContainerCodeErr.TabIndex = 0;
            // 
            // PluginEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 442);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStripMain);
            this.Name = "PluginEditor";
            this.Text = "Plugin Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginEditor_FormClosing);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.toolStripReferences.ResumeLayout(false);
            this.toolStripReferences.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerPropRef.Panel1.ResumeLayout(false);
            this.splitContainerPropRef.Panel1.PerformLayout();
            this.splitContainerPropRef.Panel2.ResumeLayout(false);
            this.splitContainerPropRef.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPropRef)).EndInit();
            this.splitContainerPropRef.ResumeLayout(false);
            this.toolStripLinks.ResumeLayout(false);
            this.toolStripLinks.PerformLayout();
            this.splitContainerCodeErr.Panel1.ResumeLayout(false);
            this.splitContainerCodeErr.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCodeErr)).EndInit();
            this.splitContainerCodeErr.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton checkButton;
        private System.Windows.Forms.ListBox referencesList;
        private System.Windows.Forms.ToolStrip toolStripReferences;
        private System.Windows.Forms.ToolStripTextBox referenceName;
        private System.Windows.Forms.ToolStripButton addReferenceButton;
        private System.Windows.Forms.ToolStripTextBox instanceName;
        private System.Windows.Forms.ToolStripButton saveAsButton;
        private System.Windows.Forms.ToolStripButton removeReferenceButton;
        private System.Windows.Forms.ListView errorListView;
        private System.Windows.Forms.RichTextBox codeEditorView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton syntaxButton;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerPropRef;
        private System.Windows.Forms.SplitContainer splitContainerCodeErr;
        private System.Windows.Forms.ListView listProperties;
        private System.Windows.Forms.ColumnHeader KeyColumn;
        private System.Windows.Forms.ToolStrip toolStripLinks;
        private System.Windows.Forms.ToolStripTextBox linkName;
        private System.Windows.Forms.ToolStripButton addLinkButton;
        private System.Windows.Forms.ToolStripButton removeLinkButton;
    }
}
