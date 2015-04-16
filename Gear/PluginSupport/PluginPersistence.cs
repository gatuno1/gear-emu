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
        public bool[] UseAuxFiles;
        /// @brief Name of external file with C# code of the plugin.
        public string[] AuxFiles;
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
            if (!Data.UseAuxFiles[0])
                instance.AppendChild(xmlDoc.CreateTextNode(Data.Codes[0]));
            else 
                instance.SetAttribute("ref",Data.AuxFiles[0]);
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
            XmlDocumentType doctype = xmlDoc.CreateDocumentType("plugin", null, "Resourses/plugin_v1.0.dtd", null);
            xmlDoc.AppendChild(doctype);
            //Main element - plugin
            XmlElement root = xmlDoc.CreateElement("plugin");
            root.SetAttribute("plugin_system_version", Data.PluginSystemVersion);
            xmlDoc.AppendChild(root);
            //level 1 element - metadata
            instance = xmlDoc.CreateElement("metadata");
            instance.SetAttribute("version", Data.PluginVersion);
            root.AppendChild(instance);
            {
                //level 2 elements - author
                if (Data.Authors != null)
                {
                    foreach (string s in Data.Authors)
                    {
                        childElement = xmlDoc.CreateElement("author");
                        instance.AppendChild(childElement);
                        if (s.Length > 0)
                        {
                            textElement = xmlDoc.CreateTextNode("");
                            childElement.AppendChild(textElement);
                            cdata = xmlDoc.CreateCDataSection(s);
                            childElement.AppendChild(cdata);
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
                    textElement = xmlDoc.CreateTextNode("");
                    childElement.AppendChild(textElement);
                    cdata = xmlDoc.CreateCDataSection(Data.Modifier);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - date_modified
                childElement = xmlDoc.CreateElement("date_modified");
                instance.AppendChild(childElement);
                if (Data.DateModified.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode("");
                    cdata = xmlDoc.CreateCDataSection(Data.DateModified);
                    childElement.AppendChild(textElement);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - cultural_reference
                childElement = xmlDoc.CreateElement("cultural_reference");
                instance.AppendChild(childElement);
                if (Data.CulturalReference.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode("");
                    cdata = xmlDoc.CreateCDataSection(Data.CulturalReference);
                    childElement.AppendChild(textElement);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - description
                childElement = xmlDoc.CreateElement("description");
                instance.AppendChild(childElement);
                if (Data.Description.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode("");
                    cdata = xmlDoc.CreateCDataSection(Data.Description);
                    childElement.AppendChild(textElement);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - usage
                childElement = xmlDoc.CreateElement("usage");
                instance.AppendChild(childElement);
                if (Data.Usage.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode("");
                    cdata = xmlDoc.CreateCDataSection(Data.Usage);
                    childElement.AppendChild(textElement);
                    childElement.AppendChild(cdata);
                }
                //level 2 elements - link
                if (Data.Links != null)
                {
                    foreach (string s in Data.Links)
                    {
                        childElement = xmlDoc.CreateElement("link");
                        instance.AppendChild(childElement);
                        if (s.Length > 0)
                        {
                            textElement = xmlDoc.CreateTextNode("");
                            cdata = xmlDoc.CreateCDataSection(s);
                            childElement.AppendChild(textElement);
                            childElement.AppendChild(cdata);
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
            instance = xmlDoc.CreateElement("instance");
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
            //level 1 elements - code
            if (Data.UseAuxFiles != null)
                for (int i = 0; i < Data.UseAuxFiles.Rank; i++)
                {
                    instance = xmlDoc.CreateElement("code");
                    root.AppendChild(instance);
                    if (!Data.UseAuxFiles[i])   //code embedded in XML file?
                    {
                        textElement = xmlDoc.CreateTextNode("");
                        instance.AppendChild(textElement);
                        instance.SetAttribute("order", Convert.ToString(i + 1));
                        cdata = xmlDoc.CreateCDataSection(Data.Codes[i]);
                        instance.AppendChild(cdata);
                    }
                    else      //code written to a separate file
                    {
                        //write the reference to the .CS file
                        instance.SetAttribute("ref", Path.GetFileName(Data.AuxFiles[i]));
                        instance.SetAttribute("order", Convert.ToString(i + 1));
                        //save the code to a .CS file (same name, different extension)
                        File.WriteAllText(Data.AuxFiles[i], Data.Codes[i], Encoding.UTF8);
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
            //TODO [ASB] : complete code to load XML v0
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            try
            {
                //Open a XML reader with the file name and settings given
                XmlReader XR = XmlReader.Create(filenameXml, settings);
                //read the XML, if it is possible
                while (XR.Read())
                {
                    switch (XR.NodeType)
                    {
                        case XmlNodeType.Attribute: //
                            break;
                        case XmlNodeType.CDATA:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.Document:
                            break;
                        case XmlNodeType.DocumentFragment:
                            break;
                        case XmlNodeType.Element:   //
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.EndEntity:
                            break;
                        case XmlNodeType.Entity:
                            break;
                        case XmlNodeType.EntityReference:
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.DocumentType:
                        case XmlNodeType.Notation:
                        case XmlNodeType.None:
                        case XmlNodeType.XmlDeclaration:
                        default:
                            break;
                    }
                }
                XR.Close();
            }
            catch (Exception)
            {

                throw;
            }



            return false;   //@todo delete this after complete de method code
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
            //TODO [ASB] : complete code to load XML v1
            return false;
        }

    }
}