/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * PluginPersistence.cs
 * Provides the reflection base and compiler components for plugins
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace Gear.PluginSupport
{
    /// @brief Class to hold metadata of the plugin.
    /// @version v15.03.26 - Added.
    public class PluginData
    {
        /// @brief Version of plugin system.
        /// @note Will be a version, if only is a valid plugin (attribute PluginData::isValid = true).
        public string PluginSystemVersion;
        public string PluginVersion;        //!< @brief Version of the plugin itself.

        public string[] Authors;            //!< @brief List of authors.
        public string Modifier;             //!< @brief Last author of modifications.
        public string DateModified;         //!< @brief Date of modifications,
        public string CulturalReference;    //!< @brief To store the cultural reference of dates.
        public string Description;          //!< @brief Description of the plugin.
        public string Usage;                //!< @brief Guides to use the plugin.
        public string[] Links;              //!< @brief Links supporting the plugin.

        public string InstanceName;         //!< @brief Class name of the plugin definition.
        public string[] References;         //!< @brief Auxiliary references to compile the plugin.

        /// @brief Flag to write the code in external file or embedded in XML file.
        public bool[] UseExtFiles;
        /// @brief Name of external file with C# code of the plugin.
        public string[] ExtFiles;
        /// @brief Text of the C# code of the plugin.
        public string[] Codes;
        /// @brief Hold the result for a validity test of related XML file, based on DTD.
        /// @note If the plugin is valid, will be a version for the plugin system (attribute 
        /// PluginData::PluginSystemVersion), nevertheless it would be 0.0 (the default one).
        bool isValid;
        /// @brief List of errors in the validation of XML file.
        List<string> ValidationErrors;

        /// @brief default constructor for PluginData
        /// @version v15.03.26 - Added.
        public PluginData()
        {
            this.isValid = true;
            ValidationErrors = new List<string>();
        }

        public void AddError(string errorText)
        {
            if (!string.IsNullOrEmpty(errorText))
                ValidationErrors.Add(errorText);
        }

        /// @brief Handle the error in the validation, saving the messages and setting 
        /// the validation state.
        /// @param[in] sender Reference to the object where the exception was raised.
        /// @param[in] args Class with the validation details event.
        /// @version v15.03.26 - Added.
        void ValidateXMLPluginEventHandler(object sender, ValidationEventArgs args)
        {
            isValid = false;
            ValidationErrors.Add(
                string.Format("XML Plugin Validation: [{0}]{1} (Line:{2}, Position:{3}).",
                args.Severity,
                args.Message,
                args.Exception.LineNumber,
                args.Exception.LinePosition)
            );
        }

        /// @brief Validate the XML against a DTD definition, retrieving the version of it.
        /// @param[in] XR Source of a XML to read of.
        /// @param[in] settings Settings to be used to validate the XML.
        /// @returns True if the XML is valid against DTD definition, False if not.
        /// @version v15.03.26 - Added.
        bool ValidateXMLPluginSource(XmlReader XR, XmlReaderSettings settings)
        {
            //read the XML, if it is possible
            while (XR.Read())
            {
                //look for the element 'plugin' in the XML
                if ((XR.NodeType == XmlNodeType.Element) && (XR.Name == "plugin"))
                {
                    //check if readable and having attributes
                    if ((XR.ReadState == ReadState.Interactive) && XR.HasAttributes)
                        //if I can move to it, it is readeable
                        if (XR.MoveToFirstAttribute())
                            do   //loop for each attribute
                            {
                                //look for the version of plugin system attribute
                                if (XR.Name == "plugin_system_version")
                                {
                                    //if it is valid version
                                    if (!string.IsNullOrEmpty(XR.Value))
                                        this.PluginSystemVersion = XR.Value; //get version
                                }
                            } while (XR.MoveToNextAttribute());
                }
            }
            XR.Close();
            //if the XML file is not valid against embedded DTD definition,
            // a ValidateXMLEventHandler was called, setting isValid property to false.
            //Otherwise it will be true.
            return this.isValid;
        }

        /// @brief Load the XML file for the plugin to determine if is valid against 
        /// the possibles and valid DTD definition for plugins.
        /// @param[in] filenameXml File name to check validity.
        /// @returns True if the XML is valid against a DTD definition for plugins.
        /// @version v15.03.26 - Added.
        public bool ValidateXMLPluginFile(string filenameXml)
        {
            //Settings to be used to validate the XML
            // reference from https://msdn.microsoft.com/en-us/library/vstudio/z2adhb2f%28v=vs.100%29.aspx
            XmlReaderSettings settings4DTD = new XmlReaderSettings();
            settings4DTD.DtdProcessing = DtdProcessing.Parse;   //need to look for DTD definition
            settings4DTD.ValidationType = ValidationType.DTD;   //validate XML against DTD
            settings4DTD.IgnoreComments = true;
            settings4DTD.IgnoreProcessingInstructions = true;
            settings4DTD.IgnoreWhitespace = true;
            settings4DTD.ValidationEventHandler += new ValidationEventHandler(ValidateXMLPluginEventHandler);
            settings4DTD.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

            try
            {
                //Open a XML reader with the file name and settings given
                XmlReader XR = XmlReader.Create(filenameXml, settings4DTD);
                //validate the file as it is, expecting it have embedded DTD definition
                if (ValidateXMLPluginSource(XR, settings4DTD)) 
                    return true;
                else  //we have to try adding default DTD definition to the XML source
                {
                    ValidationErrors.Clear();   //clear the warnings from failed validation
					this.isValid = true;		//resetting the initial status to test for validity
                    // Add the DTD definition to XML with a XmlReader
                    // reference from http://stackoverflow.com/questions/470313/net-how-to-validate-xml-file-with-dtd-without-doctype-declaration
                    // and from http://stackoverflow.com/questions/10514198/validate-xml-against-dtd-from-string
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filenameXml);
                    doc.InsertBefore( 
                        doc.CreateDocumentType("plugin", null, @"Resources\plugin_v1.0.dtd", null),
                        doc.DocumentElement);
                    //save to a memory stream
                    MemoryStream mem = new MemoryStream();
                    doc.Save(mem);
                    //we will read from the memory stream
                    XR = XmlReader.Create(mem, settings4DTD);
                    //validate from the XML source with default DTD definition
                    return ValidateXMLPluginSource(XR, settings4DTD);
                }

            }
            catch (FileNotFoundException e)
            {
                isValid = false;
                ValidationErrors.Add(
                    string.Format("File \"{0}\" not found, when validating original XML file at '{1}'",
                    filenameXml,
                    e.Source));
                return false;
            }
            catch (ArgumentNullException)
            {
                isValid = false;
                ValidationErrors.Add(
                    "No filename  was given, when validating original XML file.");
                return false;
            }
            catch (Exception e)
            {
                isValid = false;
                ValidationErrors.Add(
                    string.Format("Error {0} found",e.Message));
                return false;
            }
        }
    }

    
    /// @brief Methods to save and retrieve plugin from files, managing version of files.
    /// @version v15.03.26 - Added.
    static class PluginPersistence
    {

        /// @brief Save a plugin to XML as version 0.0
        /// @param[in] filenameXml File name in XML format, version 0.0
        /// @param[in] Data Metadata of the plugin.
        /// @returns State of saving.
        /// @version v15.03.26 - Added.
        static public bool SaveXML_v0_0(string filenameXml, PluginData Data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //Main element - plugin
            XmlElement root = xmlDoc.CreateElement("plugin");
            xmlDoc.AppendChild(root);
            //level 1 element - instance
            XmlElement instance = xmlDoc.CreateElement("instance");
            instance.SetAttribute("class", Data.InstanceName);
            root.AppendChild(instance);
            //level 1 elements - reference
            if (Data.References != null)
            {
                foreach (string s in Data.References)
                {
                    instance = xmlDoc.CreateElement("reference");
                    instance.SetAttribute("name", s);
                    root.AppendChild(instance);
                }
            }
            else 
            {
                instance = xmlDoc.CreateElement("reference");
                root.AppendChild(instance);
            }
            //level 1 element - code
            instance = xmlDoc.CreateElement("code");
            if (!Data.UseExtFiles[0])
                instance.AppendChild(xmlDoc.CreateTextNode(Data.Codes[0]));
            else 
                instance.SetAttribute("ref",Data.ExtFiles[0]);
            root.AppendChild(instance);
            //saving XML document
            xmlDoc.Save(filenameXml);

            //TODO [ASB] : catch exceptions and return false if any.
            return true;
        }

        /// @brief Save a plugin to XML as version 1.0.
        /// @param[in] filenameXml File name in XML format, version 1.0
        /// @param[in] Data Metadata of the plugin.
        /// @returns State of saving, success (=true) or fail (=false).
        /// @version v15.03.26 - Added.
        static public bool SaveXML_v1_0(string filenameXml, PluginData Data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement instance;
            XmlElement childElement;
            XmlNode textElement;
            XmlNode cdata;
            //XML declaration of encoding
            XmlDeclaration xmlDeclatation = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(xmlDeclatation);
            //Document type element & DTD to use
            xmlDoc.XmlResolver = null;
            XmlDocumentType doctype = xmlDoc.CreateDocumentType("plugin", null, @"Resourses\plugin_v1.0.dtd", null);
            xmlDoc.AppendChild(doctype);
            //Main element - plugin
            XmlElement root = xmlDoc.CreateElement("plugin");
            root.SetAttribute("plugin_system_version", Data.PluginSystemVersion);
            root.SetAttribute("version", Data.PluginVersion);
            xmlDoc.AppendChild(root);
            //level 1 element - metadata
            instance = xmlDoc.CreateElement("metadata");
            root.AppendChild(instance);
            {
                //level 2 elements - author
                if (Data.Authors != null)
                {
                    bool isValidT, isFirst = true;
                    foreach (string Author in Data.Authors)
                    {
                        isValidT = !string.IsNullOrEmpty(Author);
                        if (isFirst | isValidT)
                        {
                            childElement = xmlDoc.CreateElement("author");
                            instance.AppendChild(childElement);
                            isFirst = false;
                            if (isValidT)
                            {
                                textElement = xmlDoc.CreateTextNode(Author);
                                childElement.AppendChild(textElement);
                            }
                        }
                    }
                }
                else
                {
                    childElement = xmlDoc.CreateElement("author");
                    instance.AppendChild(childElement);
                }
                //level 2 element - modified_by
                childElement = xmlDoc.CreateElement("modified_by");
                instance.AppendChild(childElement);
                if (Data.Modifier.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.Modifier);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - date_modified
                childElement = xmlDoc.CreateElement("date_modified");
                instance.AppendChild(childElement);
                if (Data.DateModified.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.DateModified);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - cultural_reference
                childElement = xmlDoc.CreateElement("cultural_reference");
                instance.AppendChild(childElement);
                if (Data.CulturalReference.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.CulturalReference);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - description
                childElement = xmlDoc.CreateElement("description");
                instance.AppendChild(childElement);
                if (Data.Description.Length > 0)
                {
                    cdata = xmlDoc.CreateCDataSection(Data.Description);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - usage
                childElement = xmlDoc.CreateElement("usage");
                instance.AppendChild(childElement);
                if (Data.Usage.Length > 0)
                {
                    cdata = xmlDoc.CreateCDataSection(Data.Usage);
                    childElement.AppendChild(cdata);
                }
                //level 2 elements - link
                if (Data.Links != null)
                {
                    bool isValidT, isFirst = true; 
                    foreach (string link in Data.Links)
                    {
                        isValidT = !string.IsNullOrEmpty(link);
                        if (isFirst | isValidT)
                        {
                            childElement = xmlDoc.CreateElement("link");
                            instance.AppendChild(childElement);
                            isFirst = false;
                            if (isValidT)
                            {
                                cdata = xmlDoc.CreateCDataSection(link);
                                childElement.AppendChild(cdata);
                            }
                        }
                    }
                }
                else 
                {
                    childElement = xmlDoc.CreateElement("link");
                    instance.AppendChild(childElement);
                }
            }
            //level 1 element - instance
            instance = xmlDoc.CreateElement("instance_class");
            root.AppendChild(instance);
            textElement = xmlDoc.CreateTextNode(Data.InstanceName);
            instance.AppendChild(textElement);
            //level 1 elements - reference
            if (Data.References != null)
            {
                foreach (string reference in Data.References)
                {
                    if (!string.IsNullOrEmpty(reference))
                    {
                        instance = xmlDoc.CreateElement("reference");
                        root.AppendChild(instance);
                        textElement = xmlDoc.CreateTextNode(reference);
                        instance.AppendChild(textElement);
                    }
                }
            }
            //level 1 elements - code
            if (Data.UseExtFiles != null)
                for (int i = 0; i < Data.UseExtFiles.Length; i++)
                {
                    instance = xmlDoc.CreateElement("code");
                    root.AppendChild(instance);
                    if (!Data.UseExtFiles[i])   //code embedded in XML file?
                    {
                        cdata = xmlDoc.CreateCDataSection(Data.Codes[i]);
                        instance.AppendChild(cdata);
                        if (Data.UseExtFiles.Length > 1)
                            instance.SetAttribute("order", Convert.ToString(i + 1));
                    }
                    else      //code written to a separate file
                    {
                        //write the reference to the .CS file
                        instance.SetAttribute("ref", Path.GetFileName(Data.ExtFiles[i]));
                        if (Data.UseExtFiles.Length > 1)
                            instance.SetAttribute("order", Convert.ToString(i + 1));
                        //save the code to a .CS file (same name, different extension)
                        File.WriteAllText(Data.ExtFiles[i], Data.Codes[i], Encoding.UTF8);
                    }
                }
            else
            {
                instance = xmlDoc.CreateElement("code");
                root.AppendChild(instance);
            }
            //saving XML document
            xmlDoc.Save(filenameXml);

            //TODO [ASB] : catch exceptions and return false if any.
            return true;
        }

        /// @brief Load a plugin from XML as version 0.0, filling the plugin data.
        /// @param[in] filenameXml File name in XML format, version 0.0
        /// @param[in,out] Data Metadata of the plugin.
        /// @returns State of loading: True if it was succesful (also filling Data parameter 
        /// with the plugin information), or False if didn't (in this case Data parameter
        /// only should have updated IsValid attribute).
        /// @version v15.03.26 - Added.
        static public bool ExtractFromXML_v0_0(string filenameXml, ref PluginData Data)
        {
            //Settings to read the XML
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments                 = true;
            settings.IgnoreProcessingInstructions   = true;
            settings.IgnoreWhitespace               = true;
            bool success = true;    //status to return
            try
            {
                //Open a XML reader with the file name and settings given
                XmlReader XR = XmlReader.Create(filenameXml, settings);
                Stack<string> lastElement   = new Stack<string>();
                List<string> referencesTmp  = new List<string>();
                List<string> codesTmp       = new List<string>();
                //read the data from XML, assigning to corresponding PluginData member
                while (XR.Read())
                {
                    switch (XR.NodeType)
                    {
                        case XmlNodeType.Element:
                            lastElement.Push(XR.Name);  //Save the name to associate it to the content
                            break;
                        case XmlNodeType.EndElement:
                            lastElement.Pop();  //delete it
                            break;
                        case XmlNodeType.Attribute:
                            switch (XR.Name)
	                        {
                                case "class":
                                    Data.InstanceName = XR.Value;
                                    break;
                                case "name":
                                    referencesTmp.Add(XR.Value);   //add to the list of references
                                    break;
                                default:
                                    break;
	                        }   
                            break;
                        case XmlNodeType.Text:
                            switch (lastElement.Peek())
                            {
                                case "code":
                                    codesTmp.Add(XR.Value);    //add to the list of codes
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.CDATA:
                            switch (lastElement.Peek())
                            {
                                case "code":
                                    codesTmp.Add(XR.Value);    //add to the list of codes
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                XR.Close();
                if (referencesTmp.Count > 0)   //if elements exists...
                    Data.References = referencesTmp.ToArray(); //fill the array for references
                if (codesTmp.Count > 0)    //if elements exists...
                {
                    Data.Codes = codesTmp.ToArray();   //fill the array for references
                    //construct the list for UseExtFiles & ExtFiles: not used in v0.0
                    List<bool> aux = new List<bool>();  
                    List<string> aux2 = new List<string>();
                    for (int i = 0; i < codesTmp.Count; i++ )
                    {
                        aux.Add(false);
                        aux2.Add(null);
                    }
                    //fill the arrays
                    Data.UseExtFiles = aux.ToArray();   
                    Data.ExtFiles = aux2.ToArray();
                }
            }
            catch (XmlException e)
            {
                success = false;
                Data.AddError(string.Format("Error '{0}' in XML file {1}.", 
                    e.Message, 
                    e.SourceUri));
            }
            catch (Exception e)
            {
                success = false;
                Data.AddError(e.Message);
            }
            return success;
        }

        /// @brief Load a plugin from XML as version 1.0.
        /// @param[in] filenameXml File name in XML format, version 1.0
        /// @param[in] Data Metadata of the plugin.
        /// @returns State of loading: True if it was succesful (also filling Data parameter 
        /// with the plugin information), or False if didn't (in this case Data parameter
        /// only should have updated IsValid attribute).
        /// @version v15.03.26 - Added.
        static public bool ExtractFromXML_v1_0(string filenameXml, ref PluginData Data)
        {
            //Settings to read the XML
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments                 = true;
            settings.IgnoreProcessingInstructions   = true;
            settings.IgnoreWhitespace               = true;
            bool success = true;    //status to return
            try
            {
                //Open a XML reader with the file name and settings given
                XmlReader XR = XmlReader.Create(filenameXml, settings);
                //Stack to remember the section we were reading text or cdata values
                Stack<string> lastElement = new Stack<string>();
                //dynamic lists to hold values for 1:* fields 
                List<string> authorsTmp     = new List<string>();
                List<string> linksTmp       = new List<string>();
                List<string> referencesTmp  = new List<string>();
                //dynamic dictionaries to manage code & external files
                Dictionary<int, string> useExtFilesTmp  = new Dictionary<int, string>();
                Dictionary<int, string> extFilesTmp     = new Dictionary<int, string>();
                Dictionary<int, string> codesTmp        = new Dictionary<int, string>();
                Dictionary<int, int> orderCodesTmp      = new Dictionary<int, int>();
                //read the data from XML, assigning to corresponding PluginData member
                while (XR.Read())
                {
                    switch (XR.NodeType)
                    {
                        case XmlNodeType.Element:
                            lastElement.Push(XR.Name);  //Save the name to associate it to the content
                            break;
                        case XmlNodeType.EndElement:
                            lastElement.Pop();  //delete it
                            break;
                        case XmlNodeType.Attribute:
                            switch (XR.Name)
                            {
                                case "version":
                                    Data.PluginVersion = XR.Value;
                                    break;
                                case "ref":
                                    extFilesTmp.Add(XR.Value);
                                    break;
                                case "order":
                                    orderCodesTmp.Add(int.Parse(XR.Value));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (lastElement.Peek())
                            {
                                case "author":
                                    authorsTmp.Add(XR.Value);
                                    break;
                                case "modified_by":
                                    Data.Modifier = XR.Value;
                                    break;
                                case "date_modified":
                                    Data.DateModified = XR.Value;
                                    /// @todo convert read date to current locale
                                    break;
                                case "cultural_reference":
                                    Data.CulturalReference = XR.Value;
                                    break;
                                case "description":
                                    Data.Description = XR.Value;
                                    break;
                                case "usage":
                                    Data.Usage = XR.Value;
                                    break;
                                case "link":
                                    linksTmp.Add(XR.Value); //add to the list of links
                                    break;
                                case "instance_class":
                                    Data.InstanceName = XR.Value;
                                    break;
                                case "reference":
                                    referencesTmp.Add(XR.Value);   //add to the list of references
                                    break;
                                case "code":
                                    codesTmp.Add(XR.Value);    //add to the list of codes
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.CDATA:
                            switch (lastElement.Peek())
                            {
                                case "description":
                                    Data.Description = XR.Value;
                                    break;
                                case "usage":
                                    Data.Usage = XR.Value;
                                    break;
                                case "link":
                                    linksTmp.Add(XR.Value); //add to the list of links
                                    break;
                                case "code":
                                    codesTmp.Add(XR.Value);    //add to the list of codes
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;  //do nothing
                    }
                }
                XR.Close();
                if (referencesTmp.Count > 0)   //if elements exists...
                    Data.References = referencesTmp.ToArray(); //fill the array for references
                if (codesTmp.Count > 0)    //if elements exists...
                    Data.Codes = codesTmp.ToArray();   //fill the array for references
            }
            catch (XmlException e)
            {
                success = false;
                Data.AddError(string.Format("Error '{0}' in XML file {1}.", 
                    e.Message, 
                    e.SourceUri));
            }
            catch (Exception e)
            {
                success = false;
                Data.AddError(e.Message);
            }
            return success;
        }

    }
}