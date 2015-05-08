/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * PluginEditor.cs
 * Editor window for plugins class
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;

using Gear.PluginSupport;

namespace Gear.GUI
{
    /// @brief Form to edit or create GEAR plugins.
    public partial class PluginEditor : Form
    {
        /// @brief File name of current plugin on editor window.
        /// @note Include full path and name to the file.
        private string m_SaveFileName;
        /// @brief Plugin System Version of the file for a plugin. 
        private string m_FormatVersion;
        /// @brief Text for plugin Metadata
        /// @details Indicates the version of plugin system for the current plugin.
        /// @since v15.03.26 - Added.
        private string metadataText
        {
            get 
            {
                if (m_FormatVersion == null)
                    return "Metadata of plugin";
                else switch (m_FormatVersion)
                {
                    case "0.0":
                        return "No Metadata for plugin V" + m_FormatVersion;
                    default:
                        return "Metadata of plugin V" + m_FormatVersion;
                }
            }
        }
        /// @brief Default font for editor code.
        /// @since v14.07.03 - Added.
        private static Font defaultFont = new Font(FontFamily.GenericMonospace, 10, 
            FontStyle.Regular);
        /// @brief Bold font for editor code.
        /// @since v15.03.26 - Added.
        private static Font fontBold = new Font(defaultFont, FontStyle.Bold);

        /// @brief Flag if the plugin definition has changed.
        /// To determine changes, it includes not only the C# code, but also class name and 
        /// reference list.
        /// @since v15.03.26 - Added.
        private bool m_CodeChanged;
        /// @brief Enable or not change detection event.
        /// @since v15.03.26 - Added.
        private bool changeDetectEnabled;

        /// @brief Regex to looking for class name inside the code of plugin.
        /// @since v15.03.26 - Added.
        private static Regex ClassNameExpression = new Regex(
            @"\bclass\s+" +
            @"(?<classname>[@]?[_]*[A-Z|a-z|0-9]+[A-Z|a-z|0-9|_]*)" +
            @"\s*\:\s*PluginBase\b",
            RegexOptions.Compiled);
        /// @brief Regex for syntax highlight.
        /// @since v15.03.26 - Added.
        private static Regex LineExpression= new Regex(
            @"\n", 
            RegexOptions.Compiled);
        /// @brief Regex for parse token in lines for syntax highlight
        /// @version 15.03.26 - Added.
        private Regex CodeLine = new Regex(
            @"([ \t{}();:])", 
            RegexOptions.Compiled);

        /// @brief keywords to highlight in editor code
        private static readonly HashSet<string> keywords = new HashSet<string> 
        {
            "add", "abstract", "alias", "as", "ascending", "async", "await",
            "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate",
            "descending", "do", "double", "dynamic", "else", "enum", "event",
            "explicit", "extern", "false", "finally", "fixed", "float",
            "for", "foreach", "from", "get", "global", "goto", "group", "if",
            "implicit", "in", "int", "interface", "internal", "into", "is",
            "join", "let", "lock", "long", "namespace", "new", "null", "object",
            "operator", "orderby", "out", "override", "params", "partial ",
            "private", "protected", "public", "readonly", "ref", "remove",
            "return", "sbyte", "sealed", "select", "set", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this",
            "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", "value", "var", "virtual", "void",
            "volatile", "where", "while", "yield"
        };
		
        /// @brief Default constructor.
        /// Init class, defines columns for error grid, setting on changes detection initially, 
        /// and try to load the default template for plugin.
        /// @param[in] loadDefaultTemplate Indicate to load default template (=true) or 
        /// no template at all(=false).
        /// @since v15.03.26 - Added parameter for loading default template for plugin.
        public PluginEditor(bool loadDefaultTemplate)
        {
            InitializeComponent();

            changeDetectEnabled = false;
            if (loadDefaultTemplate)   //load default plugin template
            {
                try
                {
                    codeEditorView.LoadFile(
                        "Resources\\PluginTemplate.cs", RichTextBoxStreamType.PlainText);
                }
                catch (IOException) { }         //do nothing, maintaining empty the code text box
                catch (ArgumentException) { }   //
                finally { }                     //
            }

            m_SaveFileName = null;
            m_FormatVersion = null;
            changeDetectEnabled = true;
            CodeChanged = false;

            // setting default font
            defaultFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
            codeEditorView.Font = defaultFont;
            if (codeEditorView.Font == null)
                codeEditorView.Font = this.Font;

            //Setup error grid
            errorListView.FullRowSelect = true;
            errorListView.GridLines = true;
            errorListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            errorListView.Columns.Add("Line", -2, HorizontalAlignment.Left);
            errorListView.Columns.Add("Column", -2, HorizontalAlignment.Left);
            errorListView.Columns.Add("Message", -2, HorizontalAlignment.Left);

            //retrieve from settings the last state for embedded code
            SetEmbeddedCodeButton(Properties.Settings.Default.EmbeddedCode);

            //setup the metadata with the default names.
            foreach (ListViewItem item in pluginMetadataList.Items)
            {
                item.Text = GetDefaultTextMetadataElement(item.Group);
                if ((item.Group.Name == "DateModified") || (item.Group.Name == "Version"))
                    SetUserDefinedMetadataElement(item, true);
            }
        }

        /// @brief Return last plugin successfully loaded o saved.
        /// @details Useful to remember last plugin directory.
        /// @since v15.03.26 - Added.
        public string GetLastPlugin
        {
            get { return m_SaveFileName; }
        }

        /// @brief Attribute for changed plugin detection.
        /// @since v15.03.26 - Added.
        private bool CodeChanged
        {
            get { return m_CodeChanged; }
            set  
            {
                m_CodeChanged = value;
                UpdateTitles();
            }
        }

        /// @brief Complete Name for plugin, including path.
        /// @since v15.03.26 - Added.
        private string SaveFileName
        {
            get
            {
                if (!String.IsNullOrEmpty(m_SaveFileName))
                    return new FileInfo(m_SaveFileName).Name;
                else return "<New plugin>";
            }
            set
            {
                m_SaveFileName = value;
                UpdateTitles();
            }
        }

        /// @brief Update titles of window and metadata, considering modified state.
        /// @details Considering name of the plugin and showing modified state, to tell the user 
        /// if need to save.
        private void UpdateTitles()
        {
            this.Text = ("Plugin Editor: " + SaveFileName +  (CodeChanged ? " *" : ""));
            pluginMetadataList.Columns[0].Text = metadataText;
        }

        /// @brief Load a plugin from File in Plugin Editor, updating the screen.
        /// @details This method take care of update change state of the window. 
        /// @param[in] FileName Name of the file to open.
        /// @param[in] displayErrors Flag to show errors in the error grid.
        /// @note Actually it supports one plugin code window only.
        /// @version v15.03.26 - modified to validate XML & plugin version and load it
        /// with the appropriate method.
        public bool OpenFile(string FileName, bool displayErrors)
        {
            //create the structure to fill from file
            PluginData pluginCandidate = new PluginData();
            //Determine if the XML is valid, and for which DTD version
            if (!pluginCandidate.ValidateXMLPluginFile(FileName))
            {
                //if not valid, show the errors.
                foreach(string strText in pluginCandidate.ValidationErrors)
                {
                    /// @todo change to show a dialog with the errors in load
                    errorListView.Items.Clear();
                    ListViewItem item = new ListViewItem("OpenFile", 0);
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add(strText);
                    errorListView.Items.Add(item);
                }
                return false;
            }
            else  //...XML plugin file is valid & system version is determined
            {   
                bool IsSuccess = true;
                //as is valid, we have the version to look for the correct method to 
                // load it
                switch (pluginCandidate.PluginSystemVersion)
                {
                    case "0.0" :
                        IsSuccess = 
                            PluginPersistence.ExtractFromXML_v0_0(FileName, ref pluginCandidate);
                        break;
                    case "1.0":
                        IsSuccess = 
                            PluginPersistence.ExtractFromXML_v1_0(FileName, ref pluginCandidate);
                        break;
                    default:
                        ListViewItem item = new ListViewItem("OpenFile", 0);
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        item.SubItems.Add(string.Format("Unknow version \"{0}\" for Plugin system.",
                            pluginCandidate.PluginSystemVersion));
                        errorListView.Items.Add(item);
                        IsSuccess = false;
                        break;
                }
                if (IsSuccess)  //data is read succesfully from XML into pluginCandidate
                {
                    m_FormatVersion = pluginCandidate.PluginSystemVersion;
                    //initial cleanup of screen elements
                    if (referencesList.Items.Count > 0) 
                        referencesList.Items.Clear();   //clear out the reference list
                    codeEditorView.Clear();             //clear the code of the plugin
                    //fill code section on screen
                    for(int i = 0; i < pluginCandidate.UseExtFiles.Length; i++)
                    {
                        if (pluginCandidate.UseExtFiles[i])
                        {
                            //set or reset font and color
                            codeEditorView.SelectAll();
                            codeEditorView.SelectionFont = defaultFont;
                            codeEditorView.SelectionColor = Color.Black;
                            /// @todo : Add support to show in screen more than one file.
                            /// @todo : add exceptions management to file loading.
                            string externalFile = Path.Combine(Path.GetDirectoryName(FileName),
                                pluginCandidate.ExtFiles[i]);
                            codeEditorView.LoadFile(externalFile, RichTextBoxStreamType.PlainText);
                            SetEmbeddedCodeButton(embeddedCode.Checked = false);
                        }
                        else
                        {
                            //set or reset font and color
                            codeEditorView.SelectAll();
                            codeEditorView.SelectionFont = defaultFont;
                            codeEditorView.SelectionColor = Color.Black;
                            ///@todo : Add support to show in screen more than one file.
                            ///now it overwrites the code text.
                            codeEditorView.Text = pluginCandidate.Codes[i];
                            SetEmbeddedCodeButton(embeddedCode.Checked = true);
                        }
                        CodeChanged = false;
                    }
                    //fill instance text
                    ///@todo validate the instance name inside plugin against the detected one by regex.
                    instanceName.Text = pluginCandidate.InstanceName;
                    //fill references section on screen
                    foreach (string refer in pluginCandidate.References)
                    {
                        if (!string.IsNullOrEmpty(refer))
                            referencesList.Items.Add(refer);
                    }
                    //load metadata of the plugimn
                    switch (pluginCandidate.PluginSystemVersion)
                    {
                        case "1.0":
                            ClearMetadata(true);    //reset metadata in screen, enabling it
                            SetElementOfMetadata("Version", pluginCandidate.PluginVersion);
                            SetElementOfMetadata("Authors", pluginCandidate.Authors);
                            SetElementOfMetadata("ModifiedBy", pluginCandidate.Modifier);
                            SetElementOfMetadata("DateModified", pluginCandidate.DateModified);
                            SetElementOfMetadata("Description", pluginCandidate.Description);
                            SetElementOfMetadata("Usage", pluginCandidate.Usage);
                            SetElementOfMetadata("Links", pluginCandidate.Links);
                            break;
                        case "0.0":
                            ClearMetadata(false);    //reset metadata in screen, disabling it
                            break;
                    }
                    //store the name of the last file opened
                    m_SaveFileName = FileName;
                    //clean up
                    errorListView.Items.Clear();
                    UpdateTitles();
                }
                return IsSuccess;
            }
        }

        /// @brief Take the plugin information from screen and call the persistence to store 
        /// in a file.
        /// @details Take care of update change state of the window. It use methods from
        /// Gear.PluginSupport.PluginPersistence class to store in file.
        /// @param[in] FileName Name of the file to save.
        /// @param[in] version String with the version of plugin system to use for saving.
        public void SaveFile(string FileName, string version)
        {
            PluginData data = new PluginData();
            //fill struct with data from screen controls
            data.PluginSystemVersion = version; //version of plugin system
            //data.PluginVersion - version of the plugin itself
            //TODO [ASB]: add try section & detect exception thrown
            data.PluginVersion = 
                GetElementsFromMetadata("Version", false)[0];    //always expect 1 element
            //TODO [ASB]: add try section & detect exception thrown
            data.Authors = GetElementsFromMetadata("Authors");
            //TODO [ASB]: add try section & detect exception thrown
            data.Modifier = GetElementsFromMetadata("ModifiedBy")[0];   //always expect 1 element
            //TODO [ASB]: add try section & detect exception thrown
            data.DateModified = 
                GetElementsFromMetadata("DateModified", false)[0];//always expect 1 element
            //cultural reference should be the one actually used in run time
            data.CulturalReference = CultureInfo.CurrentCulture.Name;
            //TODO [ASB]: add try section & detect exception thrown
            data.Description = GetElementsFromMetadata("Description")[0]; //always expect 1 element
            //TODO [ASB]: add try section & detect exception thrown
            data.Usage = GetElementsFromMetadata("Usage")[0];             //always expect 1 element
            //TODO [ASB]: add try section & detect exception thrown
            data.Links = GetElementsFromMetadata("Links");

            data.InstanceName = instanceName.Text;
            string[] references;
            if (referencesList.Items.Count > 0)
            {
                references = new string[referencesList.Items.Count];
                for (int i = 0; i < referencesList.Items.Count; i++)
                    references[i] = referencesList.Items[i].ToString();
                data.References = references;
            }
            switch (version)
            {
                case "1.0":
                    //TODO ASB: manage multiple files from user interface
                    data.UseExtFiles = new bool[1] { (!embeddedCode.Checked) };
                    string separateFileName = Path.Combine(Path.GetDirectoryName(FileName),
                        Path.GetFileNameWithoutExtension(FileName) + ".cs");
                    data.ExtFiles = new string[1] { 
                        ((!embeddedCode.Checked) ? separateFileName : "") };
                    data.Codes = new string[1] { codeEditorView.Text };
                    //update modified state for the plugin
                    CodeChanged = !PluginPersistence.SaveXML_v1_0(FileName, data);
                    break;
                case "0.0":
                    //manage only one file on user interface
                    data.UseExtFiles = new bool[1] { false };
                    data.ExtFiles = new string[1] { "" };
                    data.Codes = new string[1] { codeEditorView.Text };
                    //update modified state for the plugin
                    CodeChanged = !PluginPersistence.SaveXML_v0_0(FileName,data);
                    break;
            }

            m_SaveFileName = FileName;
        }

        /// @brief Method to compile C# source code to check errors on it.
        /// Actually call a C# compiler to determine errors, using references.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void CheckSource_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(codeEditorView.Text))
            {
                MessageBox.Show("No source code to check. Please add code.",
                    "Plugin Editor - Check source.", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                string aux = null;
                if (DetectClassName(codeEditorView.Text, out aux))  //class name detected?
                {
                    int i = 0;
                    instanceName.Text = aux;        //show the name found in the screen field
                    errorListView.Items.Clear();    //clear error list, if any
                    //prepare reference list
                    string[] refs = new string[referencesList.Items.Count];
                    foreach (string s in referencesList.Items)
                        refs[i++] = s;
                    try
                    {
                        if (ModuleCompiler.LoadModule(codeEditorView.Text, 
                            instanceName.Text, refs, null) != null)
                            MessageBox.Show("Plugin compiled without errors.", 
                                "Plugin Editor - Check source.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        else
                        {
                            ModuleCompiler.EnumerateErrors(EnumErrors);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Compile Error: " + e.ToString(),
                            "Plugin Editor - Check source.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else   //not detected class name
                {
                    instanceName.Text = "Not found!";
                    MessageBox.Show("Cannot detect main plugin class name. " +
                        "Please use \"class <YourPluginName> : PluginBase {...\" " +
                        "declaration on your source code.",
                        "Plugin Editor - Check source.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// @brief Add error details on screen list.
        /// @param[in] e `CompileError` object.
        private void EnumErrors(CompilerError e)
        {
            ListViewItem item = new ListViewItem(e.ErrorNumber, 0);

            item.SubItems.Add(e.Line.ToString());
            item.SubItems.Add(e.Column.ToString());
            item.SubItems.Add(e.ErrorText);

            errorListView.Items.Add(item);
        }

        /// @brief Show a dialog to load a file with plugin information.
        /// @details This method checks if the previous plugin data was modified and not saved.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void OpenButton_Click(object sender, EventArgs e)
        {
            bool continueAnyway = true;
            if (CodeChanged)
            {
                continueAnyway = CloseAnyway(SaveFileName); //ask the user to not lost changes
            }
            if (continueAnyway)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Gear plug-in component (*.xml)|*.xml|All Files (*.*)|*.*";
                dialog.Title = "Open Gear Plug-in...";
                if (!String.IsNullOrEmpty(m_SaveFileName))
                    //retrieve from last plugin edited
                    dialog.InitialDirectory = Path.GetDirectoryName(m_SaveFileName);
                else
                    if (!String.IsNullOrEmpty(Properties.Settings.Default.LastPlugin))
                        //retrieve from global last plugin
                        dialog.InitialDirectory = 
                            Path.GetDirectoryName(Properties.Settings.Default.LastPlugin);   

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    OpenFile(dialog.FileName, false);   //try to open the file and load to screen
                }
            }
        }

        /// @brief Show dialog to save a plugin information into file, using GEAR plugin format.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_SaveFileName) & (m_FormatVersion != null))
                SaveFile(m_SaveFileName, m_FormatVersion);
            else
                SaveAsButton_Click(sender, e);

            UpdateTitles();   //update title window
        }

        /// @brief Show dialog to save a plugin information into file, using GEAR plugin format.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "New format Gear plug-in (*.xml)|*.xml|" +
                "Old format Gear plug-in (*.xml)|*.xml|" +
                "All Files (*.*)|*.*";
            if (m_FormatVersion == null)
                dialog.FilterIndex = 1; //default if not format selected
            else
                switch (m_FormatVersion)
                {
                    case "0.0":
                        dialog.FilterIndex = 2;
                        break;
                    case "1.0":
                        dialog.FilterIndex = 1;
                        break;
                };
            dialog.Title = "Save Gear Plug-in...";
            if (!String.IsNullOrEmpty(m_SaveFileName))
                //retrieve from last plugin edited
                dialog.InitialDirectory = Path.GetDirectoryName(m_SaveFileName);   
            else
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastPlugin))
                    //retrieve from global last plugin
                    dialog.InitialDirectory = 
                        Path.GetDirectoryName(Properties.Settings.Default.LastPlugin);    

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                //select the version to use for saving the plugin
                if (dialog.FilterIndex == 2)
                    m_FormatVersion = "0.0";
                else
                    m_FormatVersion = "1.0";
                SaveFile(dialog.FileName, m_FormatVersion);
                UpdateTitles();   //update title window
            }
        }

        /// @brief Add a reference from the `ReferenceName`text box.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void addReferenceButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(referenceName.Text))
            {
                referencesList.Items.Add(referenceName.Text);
                referenceName.Text = "";
                CodeChanged = true;
            }
        }

        /// @brief Remove the selected reference of the list.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void RemoveReferenceButton_Click(object sender, EventArgs e)
        {
            if (referencesList.SelectedIndex != -1)
            {
                referencesList.Items.RemoveAt(referencesList.SelectedIndex);
                CodeChanged = true;
            }
        }

        /// @brief Locate the offender code in code view, corresponding to the current error row.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void ErrorView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (errorListView.SelectedIndices.Count < 1)
                return;

            int i = 0;
            int index = errorListView.SelectedIndices[0];
            ListViewItem lvi = errorListView.Items[index];
            try
            {
                int line = Convert.ToInt32(lvi.SubItems[1].Text) - 1;
                int column = Convert.ToInt32(lvi.SubItems[2].Text) - 1;
                while (line != codeEditorView.GetLineFromCharIndex(i++)) ;
                i += column;
                codeEditorView.SelectionStart = i;
                codeEditorView.ScrollToCaret();
                codeEditorView.Select();
            }
            catch (FormatException) { }
            return;
        }

        /// @brief Check syntax on the C# source code
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since V14.07.03 - Added.
        /// @note Experimental highlighting. Probably changes in the future.
        // Syntax highlighting
        private void syntaxButton_Click(object sender, EventArgs e)
        {
            int restore_pos = codeEditorView.SelectionStart, pos = 0;    //remember last position
            changeDetectEnabled = false;    //not enable change detection
            bool commentMode = false;       //initially not in comment mode
            //Foreach line in input, identify key words and format them when 
            // adding to the rich text box.
            String[] lines = LineExpression.Split(codeEditorView.Text);
            //update progress bar
            progressHighlight.Maximum = lines.Length;
            progressHighlight.Value = 0;
            progressHighlight.Visible = true;
            //update editor code
            codeEditorView.Visible = false;
            codeEditorView.SelectAll();
            codeEditorView.Enabled = false;
            foreach (string line in lines)
            {
                progressHighlight.Value = ++pos;
                ParseLine(line, ref commentMode);   //remember comment mode between lines
            }
            //update progress bar
            progressHighlight.Visible = false;
            //update editor code
            codeEditorView.SelectionStart = restore_pos;    //restore last position
            codeEditorView.ScrollToCaret();                 //and scroll to it
            codeEditorView.Visible = true;
            codeEditorView.Enabled = true;
            changeDetectEnabled = true; //restore change detection
        }
        
        /// @brief Auxiliary method to check syntax.
        /// Examines line by line, parsing reserved C# words.
        /// @param[in] line Text line from the source code.
        /// @param[in,out] commentMode Flag to indicate if it is on comment mode between 
        /// lines (=true) or normal mode (=false).
        /// @since v14.07.03 - Added.
        /// @note Experimental highlighting. Probably will be changes in the future.
        private void ParseLine(string line, ref bool commentMode)
        {
            int index;

            if (commentMode)
            {
                // Check for a c style end comment
                index = line.IndexOf("*/");
                if (index != -1)    //found end comment in this line
                {
                    string comment = line.Substring(0, (index += 2));
                    codeEditorView.SelectionColor = Color.Green;
                    codeEditorView.SelectionFont = defaultFont;
                    codeEditorView.SelectedText = comment;
                    //parse the rest of the line (if any)
                    commentMode = false;
                    if (line.Length > index)
                        ParseLine(line.Substring(index), ref commentMode);
                    else
                        codeEditorView.SelectedText = "\n";
                }
                else  //not end comment in this line
                {
                    codeEditorView.SelectionColor = Color.Green;
                    codeEditorView.SelectionFont = defaultFont;
                    codeEditorView.SelectedText = line;
                    codeEditorView.SelectedText = "\n";
                }
            }
            else  //we are not in comment mode
            {
                bool putEndLine = true;
                string[] tokens = CodeLine.Split(line);

                foreach (string token in tokens)
                {
                    // Check for a c style comment opening
                    if (token == "/*" || token.StartsWith("/*"))
                    {
                        index = line.IndexOf("/*");
                        int indexEnd = line.IndexOf("*/");
                        //end comment found in the rest of the line?
                        if ((indexEnd != -1) && (indexEnd > index)) 
                        {
                            string comment = line.Substring(index, (indexEnd += 2) - index);
                            codeEditorView.SelectionColor = Color.Green;
                            codeEditorView.SelectionFont = defaultFont;
                            codeEditorView.SelectedText = comment;
                            if (line.Length > indexEnd)
                            {
                                ParseLine(line.Substring(indexEnd), ref commentMode);
                                putEndLine = false;
                            }
                            break;
                        }
                        else  //as there is no end comment in the line
                        {
                            commentMode = true; //we will enter comment mode
                            string comment = line.Substring(index, line.Length - index);
                            codeEditorView.SelectionColor = Color.Green;
                            codeEditorView.SelectionFont = defaultFont;
                            codeEditorView.SelectedText = comment;
                            break;  
                        }
                    }

                    // Check for a c++ style comment.
                    if (token == "//" || token.StartsWith("//"))
                    {
                        // Find the start of the comment and then extract the whole comment.
                        index = line.IndexOf("//");
                        string comment = line.Substring(index, line.Length - index);
                        codeEditorView.SelectionColor = Color.Green;
                        codeEditorView.SelectionFont = defaultFont;
                        codeEditorView.SelectedText = comment;
                        break;
                    }

                    // Set the token's default color and font.
                    codeEditorView.SelectionColor = Color.Black;
                    codeEditorView.SelectionFont = defaultFont;
                    // Check whether the token is a keyword.
                    if (keywords.Contains(token))
                    {
                        // Apply alternative color and font to highlight keyword.
                        codeEditorView.SelectionColor = Color.Blue;
                        codeEditorView.SelectionFont = fontBold;
                    }
                    codeEditorView.SelectedText = token;
                }
                if (putEndLine)
                    codeEditorView.SelectedText = "\n";
            }
        }

        /// @brief Update change state for code text box.
        /// It marks as changed, to prevent not averted loses at closure of the window.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void codeEditorView_TextChanged(object sender, EventArgs e)
        {
            if (changeDetectEnabled)
            {
                CodeChanged = true;
            }
        }

        /// @brief Detect the plugin class name from the code text given as parameter.
        /// @param[in] code Text of the source code of plugin to look for the class 
        /// name declaration.
        /// @param[out] match Name of the plugin class found. If not, it will be null.
        /// @returns If a match had found =True, else =False.
        /// @since v15.03.26 - Added.
        private bool DetectClassName(string code, out string match)
        {
            string aux = null;
            match = null;
            //Look for a 'suspect' for class definition to show it to user later.
            Match suspect = ClassNameExpression.Match(code);
            if (suspect.Success)  //if a match is found
            {
                //detect class name from the detected groups
                aux = suspect.Groups["classname"].Value;  
                if (String.IsNullOrEmpty(aux))
                {
                    return false;
                }
                else
                {
                    match = aux;
                    return true;
                }
            }
            else
                return false;
        }

        /// @brief Event handler for closing plugin window.
        /// If code, references or class name have changed and them are not saved, a Dialog is 
        /// presented to the user to proceed or abort the closing.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `FormClosingEventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void PluginEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CodeChanged)
            {
                if (!CloseAnyway(SaveFileName)) //ask the user to not loose changes
                    e.Cancel = true;    //cancel the closing event
            }
            Properties.Settings.Default.LastPlugin = GetLastPlugin;
            Properties.Settings.Default.Save();
        }

        /// @brief Ask the user to not loose changes.
        /// @param[in] fileName Filename to show in dialog.
        /// @returns Boolean to close (=true) or not (=false).
        /// @since v15.03.26 - Added.
        private bool CloseAnyway(string fileName)
        {
            //dialog to not lost changes
            DialogResult confirm = MessageBox.Show(
                "Are you sure to close plugin \"" + fileName + 
                "\" without saving?\nYour changes will lost.",
                "Save.",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2
            );
            if (confirm == DialogResult.OK)
                return true;
            else
                return false;
        }

        /// @brief Toggle the button state, updating the name & tooltip text.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void embeddedCode_Click(object sender, EventArgs e)
        {
            SetEmbeddedCodeButton(embeddedCode.Checked);
            //remember setting
            Properties.Settings.Default.EmbeddedCode = embeddedCode.Checked;
        }

        /// @brief Update the name & tooltip text depending on each state.
        /// @param[in] newValue Value to set.
        /// @since v15.03.26 - Added.
        private void SetEmbeddedCodeButton(bool newValue)
        {
            embeddedCode.Checked = newValue;
            if (embeddedCode.Checked)
            {
                embeddedCode.Text = "Embedded";
                embeddedCode.ToolTipText = "Embedded code in XML plugin file.";
            }
            else
            {
                embeddedCode.Text = "Separated";
                embeddedCode.ToolTipText = "Code in separated file from XML plugin file.";
            }
        }

        /// @brief Add the text to the selected element of metadata list.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void addPluginMetadataButton_Click(object sender, EventArgs e)
        {
            if ( !String.IsNullOrEmpty(textPluginMetadataBox.Text) &&
                (pluginMetadataList.SelectedIndices.Count > 0))
            {
                ListViewItem SelectedItem = null;    //selected item reference
                foreach(ListViewItem item in pluginMetadataList.SelectedItems)
                {
                    SelectedItem = item;    //should be only one, so get the last
                };
                //get the group the selected item belongs
                ListViewGroup group = SelectedItem.Group;
                //filter only groups for metadata elements with many items allowed (>1)
                if (!IsUserDefinedMetadataElement(SelectedItem) ||
                    ((group.Name != "Authors") && (group.Name != "Links")) )
                {
                    SelectedItem.Text = textPluginMetadataBox.Text;
                    //set color to normal text
                    SetUserDefinedMetadataElement(SelectedItem, true);
                }
                else 
                { 
                    //create new item with the text given and same group as item selected
                    ListViewItem newItem = new ListViewItem(textPluginMetadataBox.Text, group);
                    //set color to normal text
                    SetUserDefinedMetadataElement(newItem, true);
                    //add to the list
                    pluginMetadataList.Items.Add(newItem);
                }
                //clear the text box as we had inserted that text in the corresponding 
                // metadata element
                textPluginMetadataBox.Text = "";
                //mark as changed
                CodeChanged = true;
            }
        }

        /// @brief Remove the selected author or Link of the metadata list.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void removePluginMetadataButton_Click(object sender, EventArgs e)
        {
            if (pluginMetadataList.SelectedItems.Count > 0)
            {
                string ResetText = null;
                ListViewItem ItemToDelete = null;   //selected item reference
                foreach (ListViewItem item in pluginMetadataList.SelectedItems)
                {
                    ItemToDelete = item;     //should be only one, so get the last
                };
                //get the group where the selected item belongs & the default name for it
                ListViewGroup group = ItemToDelete.Group;
                ResetText = GetDefaultTextMetadataElement(group);
                if (!String.IsNullOrEmpty(ResetText))
                {
                    ListView.ListViewItemCollection ItemsInGroup = group.Items; 
                    if (ItemsInGroup.Count > 1) //if there are many items
                        pluginMetadataList.Items.Remove(ItemToDelete);  //delete it
                    else
                    {   
                        //instead reset the text to default
                        ItemToDelete.Text = ResetText;
                        SetUserDefinedMetadataElement(ItemToDelete, false);
                    }
                    //mark as changed
                    CodeChanged = true;
                }
            }
        }

        /// @brief Detect changes on selected item and adjust strip buttons Add & Remove for
        /// metadata accordantly.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since v15.03.26 - Added.
        private void pluginMetadataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem SelectedItem = null;   //selected item reference
            foreach (ListViewItem item in pluginMetadataList.SelectedItems)
            {
                SelectedItem = item;     //should be only one, so get the last
            };
            //set default names for metadata elements with many items allowed (>1)
            if (SelectedItem != null)
            {
                switch (SelectedItem.Group.Name)
                {
                    case "Authors":
                    case "Links":
                        removePluginMetadataButton.Text = "Remove";
                        addPluginMetadataButton.Text = "Add";
                        break;
                    default:
                        removePluginMetadataButton.Text = "Clear";
                        addPluginMetadataButton.Text = "Change";
                        break;
                }
            }
        }

        /// @brief Determine the default text for the group of metadata element 
        /// given as parameter.
        /// @param[in] group Group of metadata element.
        /// @returns Default text for the given group.
        /// @since v15.03.26 - Added.
        private string GetDefaultTextMetadataElement(ListViewGroup group)
        {
            string tex = null;
            switch (group.Name)
            {
                case "Authors":
                    tex = "Your name";
                    break;
                case "ModifiedBy":
                    tex = "Your name";
                    break;
                case "DateModified":
                    tex = DateTime.Now.Date.GetDateTimeFormats('d')[0];
                    break;
                case "Version":
                    tex = "1.0";
                    break;
                case "Description":
                    tex = "Description for the plugin";
                    break;
                case "Usage":
                    tex = "How to use the plugin";
                    break;
                case "Links":
                    tex = "Web Link to more information";
                    break;
                default:
                    tex = "Not recognized metadata element";
                    break;
            }
            return tex;
        }

        /// @brief Set the visibility of the given metadata element.
        /// @param[in] item Metadata element to set.
        /// @param[in] userDefined User defined (=true), or default value used (=false).
        /// @since v15.03.26 - Added
        private void SetUserDefinedMetadataElement(ListViewItem item, bool userDefined)
        {
            if (item != null)
            {
                if (userDefined)
                    item.ForeColor = System.Drawing.SystemColors.WindowText;
                else
                    item.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            }
        }

        /// @brief Get the visibility of the given metadata element.
        /// @param[in] item Metadata element to set.
        /// @returns User defined (=true), or default value used (=false).
        /// @since v15.03.26 - Added.
        private bool IsUserDefinedMetadataElement(ListViewItem item)
        {
            if (item != null)
            {
                if (item.ForeColor == System.Drawing.SystemColors.WindowText)
                    return true;
                else
                    return false;
            }
            else   //the item is not defined
            {   
                //TODO ASB : define and throw an exception
                return false;   //delete this line when add the exception thrown code
            }
                
        }

        /// @brief Clear the metadata screen elements of plugin editor, setting all fields 
        /// to defaults, setting its visibility, and allowing only one element on each group.
        /// @param[in] enable Enable the Metadata control or not.
        /// @since v15.03.26 - Added.
        private void ClearMetadata(bool enable)
        {
            pluginMetadataList.BeginUpdate();
            pluginMetadataList.Enabled = true;
            foreach (ListViewGroup group in pluginMetadataList.Groups)  //loop for each group
            {
                int num = group.Items.Count;
                if (num > 1)  //we have to delete elements?
                {
                    //remove all except the first
                    for (int i = (num - 1); i >= 1; i--)
                    {
                        pluginMetadataList.Items.RemoveAt(group.Items[i].Index);
                    }
                }
                //set the only element or remaining one
                group.Items[0].Text = GetDefaultTextMetadataElement(group);
                //change visibility to default text
                SetUserDefinedMetadataElement(group.Items[0], false);
            }
            pluginMetadataList.Enabled = enable;
            pluginMetadataList.EndUpdate();
            toolStripLinks.Enabled = enable;
        }

        /// @brief Set an element of metadata, given the group name.
        /// @param[in] groupName Name of the group to set a element.
        /// @param[in] value Value to set.
        /// @pre The actual algorithm assumes GUI.PluginEditor.ClearMetadata() was ejecuted before.
        /// @since v15.03.26 - Added.
        private void SetElementOfMetadata(string groupName, string value)
        {
            //looking for the group of interest
            foreach (ListViewGroup group in pluginMetadataList.Groups)  
            {
                if (group.Name == groupName)    //Is the desired group?
                {
                    if (string.IsNullOrEmpty(value))    //the text given as parameter is valid?
                    {
                        group.Items[0].Text = GetDefaultTextMetadataElement(group);
                        //change visibility to default text
                        SetUserDefinedMetadataElement(group.Items[0], false);
                    }
                    else
                    {
                        group.Items[0].Text = value;
                        //change visibility to user text
                        SetUserDefinedMetadataElement(group.Items[0], true);
                    }
                    break;
                }
            }
        }

        /// @brief Set the elemens of metadata, given the group name.
        /// @param[in] groupName Name of the group to set elements.
        /// @param[in] values Array of values to set.
        /// @pre The actual algorithm assumes GUI.PluginEditor.ClearMetadata() was ejecuted before.
        /// @since v15.03.26 - Added.
        private void SetElementOfMetadata(string groupName, string[] values)
        {
            if ((values != null) && (values.Length > 0))  //there are data to set?
            {
                //lookin for the group of interest
                foreach (ListViewGroup group in pluginMetadataList.Groups)
                {
                    if (group.Name == groupName)    //Is the desired group?
                    {
                        string val;
                        for (int i = 0; i < values.Length; i++) //loop for each value
                        {
                            bool isValid = !string.IsNullOrEmpty(values[i]);
                            //if not valid, get the default value; else use the given value
                            val = (isValid) ? values[i] : GetDefaultTextMetadataElement(group);
                            if (i > 0)
                            {
                                ListViewItem newItem = new ListViewItem(val, group);
                                pluginMetadataList.Items.Add(newItem);
                            }
                            else
                                //assuming always exists zero-index element
                                group.Items[i].Text = val;
                            //set the visibility as is using a default value or not
                            SetUserDefinedMetadataElement(group.Items[i], isValid);
                        }
                        break;
                    }
                }
            }
        }

        /// @brief Retrieve a list of elements for the given group from the metadata list, 
        /// but resetting the names depending of the given parameter.
        /// @param[in] groupName Name of the group to retrieve elements.
        /// @param[in] resetEmpty Boolean to reset the value if not user defined (=true), 
        /// or default (=false).
        /// @returns Array of elements of the group.
        /// @since v15.03.26 - Added.
        private string[] GetElementsFromMetadata(string groupName, bool resetEmpty)
        {
            string[] result = null;
            foreach (ListViewGroup group in pluginMetadataList.Groups)
            {
                if (group.Name == groupName)
                {
                    ListView.ListViewItemCollection items = group.Items;
                    if (items.Count > 0)
                    {
                        result = new string[items.Count];   //set the dimension of the array
                        for (int i = 0; i < items.Count; i++)
                        {
                            result[i] = items[i].Text;
                            //check for default text for the group
                            if (result[i] == GetDefaultTextMetadataElement(group) && resetEmpty)
                            {
                                result[i] = ""; //clear it
                            }
                        }
                        break;  //stop searching
                    }
                    else
                    {
                        //TODO ASB : throw an exception to the caller: always have to exist all the metadata elements!
                    }
                }
            }
            if (result == null)
            {
                //TODO ASB : throw an exception to the caller: always have to exist all the metadata elements!
            }
            return result;
        }

        /// @brief Retrieve a list of elements for the given group from the metadata list, 
        /// clearing the values if they are not user defined.
        /// @param[in] groupName Name of the group to retrieve elements.
        /// @returns Array of elements of the group.
        /// @since v15.03.26 - Added.
        private string[] GetElementsFromMetadata(string groupName)
        {
            return this.GetElementsFromMetadata(groupName, true);
        }

        /// @brief Manage text appearance when the user edit on Metadata text.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version v15.03.26 -Added.
        private void pluginMetadataList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            string labelValue;
            ListViewItem SelectedItem = null;    //selected item reference
            foreach (ListViewItem item in pluginMetadataList.SelectedItems)
            {
                SelectedItem = item;    //should be only one, so will get the last
            };
            switch (e.Label)    //detect changes on edited text
            {
                case null:      //nothing was changed
                    labelValue = SelectedItem.Text; //use the last text
                    break;
                case "":        //text was cleared
                    labelValue = 
                        GetDefaultTextMetadataElement(SelectedItem.Group); //use default text
                    CodeChanged = true;
                    break;
                default:        //text was changed
                    labelValue = e.Label;   //use the new text
                    CodeChanged = true;
                    break;
            }
            //compare new text edited by user with the default, to set visibility
            if (labelValue != GetDefaultTextMetadataElement(SelectedItem.Group)) //are different?
            {
                //then change visibility to user defined
                SetUserDefinedMetadataElement(SelectedItem, true);
            }
            else
            {
                //then change visibility to default text
                SetUserDefinedMetadataElement(SelectedItem, false);
            }
        }

    }
}

/// @page PluginEditorOpenFileCommonFig Common sequence inside PluginEditor.OpenFile().
/// @par Details inside OpenFile Method
/// Sequence inside the 
/// @ref Gear.GUI.PluginEditor.OpenFile() 
/// method, used in @ref PluginLoadingFromGearDesktop "\"Plugin loading from GEAR Desktop\""
/// and @ref PluginLoadInsidePluginEditor "\"Plugin loading inside Plugin Editor\"".
/// @par
/// @mscfile "Plugin_Editor_OpenFile_Common-fig.mscgen" "Figure: Common case inside PluginEditor.OpenFile(.)"


/// @page PluginLoadInsidePluginEditor Plugin Loading Sequence from inside Plugin Editor.
/// @par Main Sequence.
/// Sequence of plugin loading, after the user presses the "Open" button in the editor window (ideal 
/// flow case).
/// @par
/// @mscfile "Load_plugin_inside_Plugin_Editor-fig1.mscgen" "Figure: Loading a Plugin in Editor inside from Plugin Editor."
/// 