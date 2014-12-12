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
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;

using Gear.PluginSupport;
using System.Text.RegularExpressions;

/// @copydoc Gear.GUI
/// 
namespace Gear.GUI
{
    /// @todo Document Gear.GUI.PluginEditor
    public partial class PluginEditor : Form
    {
        /// @brief File name of current plugin on editor window.
        /// @note Include full path and name to the file.
        private string m_SaveFileName;
        /// @brief Version of the file for a plugin. 
        private string m_FormatVersion;
        /// @brief Text for About properties
        /// @version V14.12.12 - Added
        private string aboutText
        {
            get 
            {
                if (m_FormatVersion == null)
                    return "Properties of plugin";
                else switch (m_FormatVersion)
                {
                    case "0.0":
                        return "No properties for old plugin format";
                    default:
                        return "Properties of plugin v." + m_FormatVersion;
                }
            }
        }
        /// @brief Default font for editor code.
        /// @version V14.07.03 - Added.
        private Font defaultFont;    
        /// @brief Flag if the plugin definition has changed.
        /// To determine changes, it includes not only the C# code, but also class name and reference list.
        /// @version V14.07.17 - Added.
        private bool m_CodeChanged;
        /// @brief Enable or not change detection event.
        /// @version V14.07.17 - Added.
        private bool changeDetectEnabled;
        /// @brief Types of change detected.
        /// To mantain consistency between class name in C# code and class name declared in the other field.
        /// @version 14.07.25 - Added.
        private enum ChangeType : byte
        {
            none = 0,   //!< @brief No change detected.
            name,       //!< @brief Name class change detected.
            code,       //!< @brief Code change detected.
            about       //!< @brief About properties change detected.
        }
        /// @brief Store the last change detected.
        /// To determine changes, it includes only the C# code and class name.
        /// @version 14.07.25 - Added.
        private ChangeType LastChange;
        /// @brief Store the last consistency problem detected.
        /// @version 14.07.25 - Added.
        private string LastProblem;

        /// @brief Default constructor.
        /// Init class, defines columns for error grid, setting on changes detection initially, and 
        /// try to load the default template for plugin.
        /// @param[in] loadDefaultTemplate Indicate to load default template (=true) or no (=false).
        /// @version 14.7.27 - Added parameter for loading default template for plugin.
        public PluginEditor(bool loadDefaultTemplate)
        {
            InitializeComponent();

            changeDetectEnabled = false;
            if (loadDefaultTemplate)   //load default plugin template
            {
                try
                {
                    codeEditorView.LoadFile("Resources\\PluginTemplate.cs", RichTextBoxStreamType.PlainText);
                }
                catch (IOException) { }         //do nothing, mantaining empty the code text box
                catch (ArgumentException) { }   //
                finally { }                     //
            }

            m_SaveFileName = null;
            m_FormatVersion = null;
            changeDetectEnabled = true;
            CodeChanged = false;
            LastChange = ChangeType.none;
            LastProblem = "None";

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

            //retrieve from settings the last state of embedded code
            SetEmbeddedCodeButton(Properties.Settings.Default.EmbeddedCode);
            
        }

        /// @brief Return last plugin succesfully loaded o saved.
        /// @details Handy to remember last plugin directory.
        /// @version V14.07.17 - Added.
        public string GetLastPlugin
        {
            get { return m_SaveFileName; }
        }

        /// @brief Attribute for changed plugin detection.
        /// @version V14.07.17 - Added.
        private bool CodeChanged
        {
            get { return m_CodeChanged; }
            set  
            {
                m_CodeChanged = value;
                UpdateTitle();
            }
        }

        /// @brief Complete Name for plugin, including path.
        /// @version V14.07.17 - Added.
        private string SaveFileName
        {
            get
            {
                if (m_SaveFileName != null)
                    return new FileInfo(m_SaveFileName).Name;
                else return "<New plugin>";
            }
            set
            {
                m_SaveFileName = value;
                UpdateTitle();
            }
        }

        /// @brief Update title window, considering modified state.
        /// @details Considering name of the plugin and showing modified state, to tell the user 
        /// if need to save.
        private void UpdateTitle()
        {
            this.Text = ("Plugin Editor: " + SaveFileName +  (CodeChanged ? " *" : ""));
        }

        /// @brief Load a plugin from File.
        /// @note This method take care of update change state of the window. 
        /// @todo Correct method to implement new plugin system.
        public bool OpenFile(string FileName, bool displayErrors)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            XmlReader tr = XmlReader.Create(FileName, settings);
            bool ReadText = false;

            if (referencesList.Items.Count > 0) 
                referencesList.Items.Clear();   //clear out the reference list
            try
            {

                while (tr.Read())
                {
                    if (tr.NodeType == XmlNodeType.Text && ReadText)
                    {
                        //set or reset font and color
                        codeEditorView.SelectAll();
                        codeEditorView.SelectionFont = this.defaultFont;
                        codeEditorView.SelectionColor = Color.Black;
                        codeEditorView.Text = tr.Value;
                        ReadText = false;
                    }

                    switch (tr.Name.ToLower())
                    {
                        case "reference":
                            if (tr.HasAttributes)     //prevent empty element generates error
                                referencesList.Items.Add(tr.GetAttribute("name"));
                            break;
                        case "instance":
                            instanceName.Text = tr.GetAttribute("class");
                            break;
                        case "code":
                            ReadText = true;
                            break;
                    }
                }
                m_SaveFileName = FileName;
                CodeChanged = false;

                if (displayErrors)
                {
                    errorListView.Items.Clear();
                    ModuleCompiler.EnumerateErrors(EnumErrors);
                }

                return true;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(this,
                    ioe.Message,
                    "Failed to load plug-in",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }
            catch (XmlException xmle)
            {
                MessageBox.Show(this,
                    xmle.Message,
                    "Failed to load plug-in",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }
            finally
            {
                tr.Close();
            }
        }

        /// @brief Save a XML file with the plugin information.
        /// @details Take care of update change state of the window. No need to do it in 
        /// methods who call this.
        /// @todo Correct method to implement new versioning plugin system.
        public void SaveFile(string FileName, string version)
        {
            PluginPersistence.PluginData data = new PluginPersistence.PluginData();
            //fill struct with data from screen controls
            data.PluginSystemVersion = version;
            //TODO [ASB]: PluginData.PluginVersion - obtain data from screen control
            data.PluginVersion = version;
            //TODO [ASB]: PluginData.Authors - obtain data from screen control
            //string[] Authors = new string[<author list>.Lenght]
            //for(int i = 0; i < <author list>.Lenght; i++)
            //    Authors[i] = <author list>[i];
            //data.Authors = Authors;
            //TODO [ASB]: PluginData.Modifier - obtain data from screen control
            data.Modifier = "Modifier test";
            //TODO [ASB]: PluginData.DateModified - obtain data from screen control
            data.DateModified = DateTime.Today.ToString("u");
            //TODO [ASB]: PluginData.Description - obtain data from screen control
            data.Description = "Test Description";
            //TODO [ASB]: PluginData.Usage - obtain data from screen control
            data.Usage = "Test Usage";
            //TODO [ASB]: PluginData.Links - obtain data from screen control
            //string[] links = new string[<lista links>.Count];
            //for(int i = 0; i < <lista links>.Lenght; i++)
            //    links[i] = <lista links>[i];
            //data.Links = links;
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
                    //TODO [ASB] : set the code to manage multiple files on user interface
                    data.UseAuxFiles = new bool[1] { false };
                    data.AuxFiles = new string[1] { "" };
                    data.Codes = new string[1] { codeEditorView.Text };
                    //update modified state for the plugin
                    CodeChanged = !PluginPersistence.SaveXML_v1_0(FileName, data);
                    break;
                case "0.0":
                    //TODO [ASB] : set the code to manage multiple files on user interface
                    data.UseAuxFiles = new bool[1] { false };
                    data.AuxFiles = new string[1] { "" };
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
            if ((codeEditorView.TextLength > 0) && IsConsistent())
            {
                string[] refs = new string[referencesList.Items.Count];
                int i = 0;

                errorListView.Items.Clear();
                foreach (string s in referencesList.Items)
                    refs[i++] = s;

                if (ModuleCompiler.LoadModule(codeEditorView.Text, instanceName.Text, refs, null) != null)
                    MessageBox.Show("Script compiled without errors.", "Plugin Editor - Check source.");
                else
                {
                    ModuleCompiler.EnumerateErrors(EnumErrors);
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

        /// @brief Show dialog to load a file with plugin information.
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
                if (m_SaveFileName != null)
                    dialog.InitialDirectory = Path.GetDirectoryName(m_SaveFileName);   //retrieve from last plugin edited
                else
                    if (Properties.Settings.Default.LastPlugin.Length > 0)
                        //retrieve from global last plugin
                        dialog.InitialDirectory = 
                            Path.GetDirectoryName(Properties.Settings.Default.LastPlugin);   

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    OpenFile(dialog.FileName, false);
                }
            }
        }

        /// @brief Show dialog to save a plugin information into file, using GEAR plugin format.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if ((m_SaveFileName != null) & (m_FormatVersion != null))
                SaveFile(m_SaveFileName, m_FormatVersion);
            else
                SaveAsButton_Click(sender, e);

            UpdateTitle();   //update title window
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
            if (m_SaveFileName != null)
                //retrieve from last plugin edited
                dialog.InitialDirectory = Path.GetDirectoryName(m_SaveFileName);   
            else
                if (Properties.Settings.Default.LastPlugin.Length > 0)
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
                UpdateTitle();   //update title window
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

        /// @todo Document method PluginEditor.ErrorView_SelectedIndexChanged() 
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void ErrorView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (errorListView.SelectedIndices.Count < 1)
                return;

            int index = errorListView.SelectedIndices[0];

            ListViewItem lvi = errorListView.Items[index];

            int line = Convert.ToInt32(lvi.SubItems[1].Text) - 1;
            int column = Convert.ToInt32(lvi.SubItems[2].Text) - 1;

            int i = 0;

            while (line != codeEditorView.GetLineFromCharIndex(i++)) ;

            i += column;

            codeEditorView.SelectionStart = i;
            codeEditorView.ScrollToCaret();
            codeEditorView.Select();

            return;
        }

        /// @brief Add a reference from the `ReferenceName`text box.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        private void addReferenceButton_Click(object sender, EventArgs e)
        {
            if ((referenceName.Text != null) && (referenceName.Text.Length > 0))
            {
                referencesList.Items.Add(referenceName.Text);
                referenceName.Text = "";
                CodeChanged = true;
            }
        }

        /// @brief Check syntax on the C# source code
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @since V14.07.03 - Added.
        /// @note Experimental highlighting. Probably changes in the future.
        // Sintax highlighting
        private void syntaxButton_Click(object sender, EventArgs e)
        {
            int restore_pos = codeEditorView.SelectionStart;    //remember last position
            changeDetectEnabled = false;    //not enable change detection
            // Foreach line in input,
            // identify key words and format them when adding to the rich text box.
            Regex r = new Regex("\\n", RegexOptions.Compiled);
            String[] lines = r.Split(codeEditorView.Text);
            codeEditorView.SelectAll();
            codeEditorView.Enabled = false;
            foreach (string l in lines)
            {
                ParseLine(l);
            }
            codeEditorView.SelectionStart = restore_pos;    //restore last position
            codeEditorView.ScrollToCaret();                 //and scroll to it
            codeEditorView.Enabled = true;
            changeDetectEnabled = true; //restore change detection
        }

        /// @brief Auxiliary method to check syntax.
        /// Examines line by line, parsing reserved C# words.
        /// @param[in] line Text line from the source code.
        /// @since V14.07.03 - Added.
        /// @note Experimental highlighting. Probably changes in the future.
        // Parse line for sintax highlighting.
        private void ParseLine(string line)
        {
            Regex r = new Regex("([ \\t{}();:])", RegexOptions.Compiled);
            String[] tokens = r.Split(line);
            System.Drawing.Font fontRegular = this.defaultFont;
            System.Drawing.Font fontBold = new Font(fontRegular, FontStyle.Bold);

            foreach (string token in tokens)
            {
                // Set the token's default color and font.
                codeEditorView.SelectionColor = Color.Black;
                codeEditorView.SelectionFont = fontRegular;

                // Check for a comment.
                if (token == "//" || token.StartsWith("//"))
                {
                    // Find the start of the comment and then extract the whole comment.
                    int index = line.IndexOf("//");
                    string comment = line.Substring(index, line.Length - index);
                    codeEditorView.SelectionColor = Color.Green;
                    codeEditorView.SelectionFont = fontRegular;
                    codeEditorView.SelectedText = comment;
                    break;
                }

                // Check whether the token is a keyword.
                String[] keywords = {
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
                for (int i = 0; i < keywords.Length; i++)
                {
                    if (keywords[i] == token)
                    {
                        // Apply alternative color and font to highlight keyword.
                        codeEditorView.SelectionColor = Color.Blue;
                        codeEditorView.SelectionFont = fontBold;
                        break;
                    }
                }
                codeEditorView.SelectedText = token;
            }
            codeEditorView.SelectedText = "\n";
        }

        /// @brief Update change state for code text box.
        /// It marks as changed, to prevent unaverted loses at closure of the window.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version V14.07.17 - Added.
        private void codeEditorView_TextChanged(object sender, EventArgs e)
        {
            if (changeDetectEnabled)
            {
                CodeChanged = true;
                LastChange = ChangeType.code;
            }
        }

        /// @brief Update change state for instance name.
        /// When the text of the text box changes, marks the code as modified, to 
        /// prevent unaverted loses at closure of the window.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version V14.07.17 - Added.
        private void instanceName_TextChanged(object sender, EventArgs e)
        {
            CodeChanged = true;
            LastChange = ChangeType.name;
        }

        /// @brief Update the name on the text box after leave the control.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version V14.07.17 - Added.
        private void instanceName_Leave(object sender, EventArgs e)
        {
            instanceName.Text = instanceName.Text.Trim();   //trim spaces at both ends
        }


        /// @brief Inform user if there inconsistency in class name declared.
        /// If the class name isn't the same that in class declaration in code, show the user a message,
        /// and show the problem in code text box or class name text box.
        /// @version V14.07.17 - Added.
        private bool IsConsistent()
        {
            int start = 0, len = 0;
            //Test if there is inconsistency in class name product of the change in this control...
            if (DetectDiffClassName(instanceName.Text, codeEditorView.Text, ref start, ref len))
            {
                //...there is inconsistency
                string Problem = "";
                if ((instanceName.TextLength != 0) && (codeEditorView.TextLength != 0))
                {
                    switch (LastChange)
                    {
                        case ChangeType.none:
                            Problem = "Lasting problem: " + LastProblem;
                            break;
                        case ChangeType.code:
                            Problem = "Class name not found in changed code class definition.";
                            LastProblem = Problem;
                            break;
                        case ChangeType.name:
                            Problem = "Class Name changed but not found in code class definition:\n" +
                                "    \"class <name> : PluginBase\".";
                            LastProblem = Problem;
                            break;
                    }
                    MessageBox.Show(
                        "Problem detected: class name \"" + instanceName.Text +
                            "\" inconsistent with code.\n" + Problem,
                        "Plugin Editor - Validation.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    if ((LastChange == ChangeType.name) || (LastChange == ChangeType.code))
                    {
                        bool Selected = false;      //for detect if the pattern of class declaration was encountered
                        if (len != 0)
                        {
                            codeEditorView.SelectionStart = start;
                            codeEditorView.SelectionLength = len;
                            Selected = true;        //signal that it was encountered
                        }
                        if (LastChange == ChangeType.name)
                        {
                            instanceName.SelectAll();
                            instanceName.Focus();
                        }
                        else
                        {
                            if (!Selected)      //if not detected the pattern of class declararion
                                codeEditorView.SelectAll();    //select all
                            codeEditorView.Focus();
                        }
                    }
                }
                return false;   //not consistent
            }
            else
            {
                LastChange = ChangeType.none;
                return true;    //the class name definition is consistent 
            }
        }

        /// @brief Detect if class name is not defined the same in code text
        /// This search in code a definition as "class <nameClass> : PluginBase" coherent
        /// with the content "<nameClass>" on class name text box.
        /// @param[in] name `string` with the class name
        /// @param[in] code `string` with the c# code
        /// @param[out] startPos Return the start position of class definition suspect.
        /// @param[out] _length Return the lenght of class definition 'suspect' if found; =0 if not found.
        /// @returns Differences encountered (=true) of class name are ok in both sides (=false).
        private bool DetectDiffClassName(string name, string code, ref int startPos, ref int _length)
        {
            //look for class definition inside the code, with the name given
            Regex r = new Regex(@"\bclass\s+" + name + @"\s*\:\s*PluginBase\b",
                RegexOptions.Compiled);
            Match m = r.Match(code);    //try to find matches in code text
            if (!m.Success) //if not found
            {
                //Look for a 'suspect' for class definition to show it to user later.
                //This time the pattern "[@]?[_]*[A-Z|a-z|0-9]+[A-Z|a-z|0-9|_]*" represent a C# identifier
                Regex f = new Regex(@"\bclass\s+[@]?[_]*[A-Z|a-z|0-9]+[A-Z|a-z|0-9|_]*\s*\:\s*PluginBase\b",
                    RegexOptions.Compiled);
                Match n = f.Match(code);
                if (n.Success)  //if a match is found
                {               
                    startPos = n.Index;
                    _length = n.Length;
                }
                else     //match not found
                {
                    startPos = 0;
                    _length = 0;    //check this on caller for no match found.
                }
            }
            return (!m.Success);
        }

        /// @brief Event handler for closing plugin window.
        /// If code, references or class name have changed and them are not saved, a Dialog is 
        /// presented to the user to proceed or abort the closing.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `FormClosingEventArgs` class with a list of argument to the event call.
        /// @version V14.07.17 - Added.
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
        /// @param fileName Filename to show in dialog
        /// @returns Boolean to close (true) or not (false)
        /// @version V14.07.17 - Added.
        private bool CloseAnyway(string fileName)
        {
            //dialog to not lost changes
            DialogResult confirm = MessageBox.Show(
                "Are you sure to close plugin \"" + fileName + "\" without saving?\nYour changes will lost.",
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
        /// @version 14.12.12 - Added.
        private void embeddedCode_Click(object sender, EventArgs e)
        {
            SetEmbeddedCodeButton(embeddedCode.Checked);
            //remember setting
            Properties.Settings.Default.EmbeddedCode = embeddedCode.Checked;
        }

        /// @brief Update the name & tooltip text depending on each state.
        /// @param[in] newValue Value to set.
        /// @version 14.12.12 - Added.
        private void SetEmbeddedCodeButton(bool newValue)
        {
            embeddedCode.Checked = newValue;
            if (embeddedCode.Checked)
            {
                embeddedCode.Text = "Embedded";
                embeddedCode.ToolTipText = "Embedded code in plugin file";
            }
            else
            {
                embeddedCode.Text = "Separated";
                embeddedCode.ToolTipText = "Code in separated file from plugin";
            }
        }

        /// @brief Add the selected Author or Link to the about properties list.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version 14.12.12 - Added.
        private void addAuthorLinkButton_Click(object sender, EventArgs e)
        {
            if ((authorLinkName.Text != null) && (authorLinkName.Text.Length > 0) && (listProperties.SelectedIndices.Count > 0))
            {
                ListViewItem SelectedItem = null;    //selected item reference
                foreach(ListViewItem item in listProperties.SelectedItems)
                {
                    SelectedItem = item;    //should be only one, so get the last
                };
                //get the group the selected item belongs
                ListViewGroup group = SelectedItem.Group;
                //filter only groups for properties with many items allowed (>1)
                if ((group.Name == "Authors") || (group.Name == "Links"))
                {
                    if (SelectedItem.ForeColor == System.Drawing.SystemColors.InactiveCaption)
                    {
                        SelectedItem.Text = authorLinkName.Text;
                        //set color to normal text
                        SelectedItem.ForeColor = System.Drawing.SystemColors.WindowText;
                    }
                    else 
                    { 
                        //create new item with the text given and same group as item selected
                        ListViewItem newItem = new ListViewItem(authorLinkName.Text, group);
                        //set color to normal text
                        newItem.ForeColor = System.Drawing.SystemColors.WindowText;
                        //add to the list
                        listProperties.Items.Add(newItem);
                    }
                    //clear the text box as we had inserted that text in the about property
                    authorLinkName.Text = "";
                    //mark as changed
                    CodeChanged = true;
                }
            }
        }

        /// @brief Remove the selected author or Link of the about properties list.
        /// Also update change state for the plugin module, marking as changed.
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version 14.12.12 - Added.
        private void removeAuthorLinkButton_Click(object sender, EventArgs e)
        {
            if (listProperties.SelectedItems.Count > 0)
            {
                string ResetText = null;
                ListViewItem ItemToDelete = null;   //selected item reference
                foreach (ListViewItem item in listProperties.SelectedItems)
                {
                    ItemToDelete = item;     //should be only one, so get the last
                };
                //get the group where the selected item belongs
                ListViewGroup group = ItemToDelete.Group;
                //set default names for properties with many items allowed (>1)
                switch (group.Name)
                {
                    case "Authors":
                        ResetText = "Your name";
                        break;
                    case "Links":
                        ResetText = "Web Link to more information";
                        break;
                    default:    //do nothing for properties with only one item allowed
                        break;
                }
                if (ResetText != null)
                {
                    ListView.ListViewItemCollection ItemsInGroup = group.Items; 
                    if (ItemsInGroup.Count > 1) //if there are many items
                        listProperties.Items.Remove(ItemToDelete);  //delete it
                    else
                    {   
                        //instead reset the text to default
                        ItemToDelete.Text = ResetText;
                        ItemToDelete.ForeColor = System.Drawing.SystemColors.InactiveCaption;
                    }
                    //mark as changed
                    CodeChanged = true;
                }
            }
        }

        /// @brief .
        /// @param[in] sender Object who called this on event.
        /// @param[in] e `EventArgs` class with a list of argument to the event call.
        /// @version 14.12.12 - Added.
        private void listProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void splitContainerMain_DoubleClick(object sender, EventArgs e)
        {

        }

        private void splitContainerCodeErr_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }


    }
}
