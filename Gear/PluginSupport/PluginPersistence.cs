/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * PluginPersistence.cs
 * Provides the reflection base and compiler components for plugins.
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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Gear.PluginSupport
{
    /// @brief Custom XML resolver, to locate the DTD definition file in the appropriate folder.
    /// @details Instead of search of DTD file in the location of XML plugin file, this redirects 
    /// to search in the base directory of the GEAR executable.
    /// @since v15.03.26 - Added.
    class DTDResolver : System.Xml.XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri temp = new Uri(AppDomain.CurrentDomain.BaseDirectory, UriKind.Absolute);
            Uri outVal = base.ResolveUri(temp, relativeUri);
            return outVal;
        }
    }

    public enum PluginDataErrorType
    {
        none = 0,           //!< @brief There is no error.
        notFoundDTD,        //!< @brief DTD not found.
        openFile,           //!< @brief Can't open XML file.
        nonDTDComplaint,    //!< @brief The XML is not valid against corresponding DTD.
        other = 100         //!< @brief Other cause.
    }

    /// @brief exception class when not valid plugin data detected.
    public class NotValidPluginDataException : Exception
    {
        private PluginDataErrorType ErrorType;
        private string Operation;

        public NotValidPluginDataException(PluginDataErrorType errType, string operationDesc)
        {
            ErrorType = errType;
            Operation = operationDesc;
        }

        public override string  Message
        {
            get
            {
                string prolog = null;
                switch (ErrorType)
                {
                    case PluginDataErrorType.none:
                        break;
                    case PluginDataErrorType.notFoundDTD:
                    case PluginDataErrorType.openFile:
                    case PluginDataErrorType.nonDTDComplaint:
                        prolog = "Error '";
                        break;
                    case PluginDataErrorType.other:
                        prolog = "Exception catch:'";
                        break;
                }
                if (ErrorType != PluginDataErrorType.none)
                    return string.Concat(prolog, ErrorType.ToString(), "' ", Operation, ".");
                else
                    return "No error detected, but NotValidPluginData Exception called on " + Operation;
            }
        }
    }

    /// @brief Class to hold metadata of the plugin
    /// @since v15.03.26 - Added.
    [DefaultPropertyAttribute("PluginVersion")]
    public class PluginMetadata
    {
         /// @brief Version of the plugin itself.
        [CategoryAttribute("About")]
        [DescriptionAttribute("Version number of the plugin.")]
        [DisplayName("Plugin Version")]
        [DefaultValueAttribute("1.0")]
        public string PluginVersion { get; set; }

        /// @brief List of authors.
        [CategoryAttribute("About")]
        [DescriptionAttribute("The name of original author of the plugin.")]
        public string[] Authors { get; set; }

        /// @brief Last author of modifications.
        [CategoryAttribute("Modify")]
        [DescriptionAttribute("The name of the last modifier.")]
        public string Modifier { get; set; }

        /// @brief Date of modifications,
        [CategoryAttribute("Modify")]
        [DescriptionAttribute("When the last modification was made.")]
        [DisplayName("Date Modified")]
        public string DateModified { get; set; }
        
        /// @brief To store the cultural reference of dates.
        [BrowsableAttribute(false)]
        public string CulturalReference { get; set; }
        
        /// @brief Release notes (version modifications).
        [CategoryAttribute("Modify")] 
        [DescriptionAttribute("Description of changes of this version of the plugin.")]
        [DisplayName("Release Notes")]
        public string ReleaseNotes { get; set; }

        /// @brief Description of the plugin.
        [CategoryAttribute("About")] 
        [DescriptionAttribute("What the plugin does.")]
        public string Description { get; set; }

        /// @brief Guides to use the plugin.
        [CategoryAttribute("About")] 
        [DescriptionAttribute("How this plugin is supposed to be used.")]
        public string Usage { get; set; }

        /// @brief Links supporting the plugin.
        [CategoryAttribute("About")] 
        [DescriptionAttribute("Web links for more information.")]
        [DisplayName("Web Links")]
        public string[] Links { get; set; }

        /// @brief Default constructor.
        public PluginMetadata() 
        { 
            //use current value
            CulturalReference = CultureInfo.CurrentCulture.Name;
        }

        /// @brief Copy constructor
        /// @param original The original source of metadata data.
        public PluginMetadata(PluginMetadata original)
        {
            this.Clone(original);
        }

        /// @brief Copy the data of the parameter metadata.
        /// @param other The source of metadata data.
        public void Clone(PluginMetadata other)
        {
            this.PluginVersion = other.PluginVersion;
            this.Authors = (string[])other.Authors.Clone();
            this.Modifier = other.Modifier;
            this.DateModified = other.DateModified;
            this.CulturalReference = other.CulturalReference;
            this.ReleaseNotes = other.ReleaseNotes;
            this.Description = other.Description;
            this.Usage = other.Usage;
            this.Links = (string[])other.Links.Clone();
        }
    }

    /// @brief Class to hold data of the plugin.
    /// @since v15.03.26 - Added.
    public class PluginData
    {
        /// @brief Version of plugin system.
        /// @note Will be a version, if only is a valid plugin (attribute PluginData::isValid = true).
        public string PluginSystemVersion;
        /// @brief Metadata properties for the plugin.
        public PluginMetadata metaData;
        //public string PluginVersion;
        /// @brief Version of the plugin itself.
        public string PluginVersion
        {
            get { return metaData.PluginVersion; }
        }
        /// @brief Description of the plugin.
        public string Description
        {
            get { return metaData.Description; } 
        }
        //public string[] Authors;            //!< @brief List of authors.
        //public string Modifier;             //!< @brief Last author of modifications.
        //public string DateModified;         //!< @brief Date of modifications,
        //public string CulturalReference;    //!< @brief To store the cultural reference of dates.
        //public string ReleaseNotes;         //!< @brief Release notes (version modifications).
        //public string Description;          //!< @brief Description of the plugin.
        //public string Usage;                //!< @brief Guides to use the plugin.
        //public string[] Links;              //!< @brief Links supporting the plugin.

        public string InstanceName;         //!< @brief Class name of the plugin definition.
        public string[] References;         //!< @brief Auxiliary references to compile the plugin.

        /// @brief Flag to write the code in external file or embedded in XML file.
        public bool[] UseExtFiles;
        /// @brief Name of external file with C# code of the plugin.
        public string[] ExtFiles;
        /// @brief Text of the C# code of the plugin.
        public string[] Codes;
        /// @brief Name of the main XML plugin file.
        public string MainFile;
        /// @brief Name of the assembly file in case of had been compiled to file
        public string AssemblyFile;
        /// @brief Assembly full name for the plugin file.
        public string AssemblyFullName
        {
            get
            {
                return AssemblyUtils.CompiledPluginFullName(
                    AssemblyUtils.GetFileDateTime(this.MainFile), 
                    this.InstanceName, 
                    this.metaData.PluginVersion, 
                    this.PluginSystemVersion);
            }
        }
        /// @brief Hold the result for a validity test of related XML file, based on DTD.
        /// @note If the plugin is valid, the version number for the plugin system will exist 
        /// (attribute PluginData::PluginSystemVersion), nevertheless it would be 0.0 (the 
        /// default one).
        bool isValid;
        /// @brief Error type found in validation
        PluginDataErrorType errorType;
        /// @brief List of errors in the validation of XML file.
        public List<string> ValidationErrors;

        /// @brief default constructor for PluginData
        /// @version v15.03.26 - Added.
        public PluginData()
        {
            this.isValid = true;
            this.errorType = PluginDataErrorType.none;
            ValidationErrors = new List<string>();
            AssemblyFile = null;
            metaData = new PluginMetadata();
        }

        /// @brief Add an error to the list.
        /// @param errorText Text of the error.
        /// @since v15.03.26 - Added.
        public void AddError(string errorText)
        {
            if (!string.IsNullOrEmpty(errorText))
                ValidationErrors.Add(errorText);
        }

        /// @brief Determine if the plugin allow only one instance o not.
        /// @details Use reflection to load and evaluate the member PluginCommon.SingleInstanceAllowed
        /// @throws NotValidPluginDataException 
        /// @returns True if plugin allow only one instance, of False if more than one is allowed.
        public bool OnlySingleInstance()
        {
            //bool value;
            if (isValid) 
            {
                if (!string.IsNullOrEmpty(MainFile))
                {
                    //determine the file name to compile the plugin
                    string nameToCompile = AssemblyUtils.CompiledPluginName(
                        InstanceName, 
                        PluginSystemVersion, 
                        ".dll");
                    //create a temporally app domain
                    AppDomain testDomain = AppDomain.CreateDomain(InstanceName + "TestDomain");
                    if (AssemblyFile == null)
                    {
                        if (!CompileToFile(nameToCompile))
                            throw new NotValidPluginDataException(
                                PluginDataErrorType.openFile,
                                string.Concat(" trying to compile file '", nameToCompile, "'"));
                    }
                    //try to load the plugin from the assembly file
                    //ObjectHandle pluginHandle = 
                    //    testDomain.CreateInstanceFrom(MainFile, InstanceName);
                    //if (pluginTest != null)
                    //    value = pluginTest.SingleInstanceAllowed;
                    //else throw new NotValidPluginData(
                    //        PluginDataErrorType.openFile,
                    //        string.Concat("couldn't invoke '", InstanceName,
                    //            ".SingleInstanceAllowed' member with Reflection ",
                    //            "on PluginData.OnlySingleInstance()"));
                    //unload the test domain
                    AppDomain.Unload(testDomain);
                    return true;    //temp statement
                }
                else throw new NotValidPluginDataException(PluginDataErrorType.openFile,
                    "because file name is empty on PluginData.OnlySingleInstance()");
            }
            else throw new NotValidPluginDataException(
                PluginDataErrorType.openFile,
                string.Concat("because Plugin data for '", InstanceName, 
                    "' is not valid, on PluginData.OnlySingleInstance()"));
        }

        /// @brief Compile the plugin to a file.
        /// @returns True if the compile was successful, false otherwise.
        /// @since v15.03.26 - Added.
        private bool CompileToFile(string fileName)
        {
            //TODO ASB - complete the PluginData.CompileToFile() method
            //ModuleCompiler.chachePath
            //delete the following:
            return true;    //temp statement
        }

        /// @brief Handle the error in the validation, saving the messages and setting 
        /// the validation state.
        /// @param[in] sender Reference to the object where the exception was raised.
        /// @param[in] args Class with the validation details event.
        /// @since v15.03.26 - Added.
        public void ValidatePluginXMLEventHandler(object sender, ValidationEventArgs args)
        {
            isValid = false;
            if (args.Exception.GetType() == typeof(System.Xml.Schema.XmlSchemaException) )
                this.errorType = PluginDataErrorType.notFoundDTD;
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
        /// @returns True if the XML is valid against DTD definition, False if not.
        /// @pre This implementation assumes the existence of attribute plugin_system_version
        /// linked to existence of DTD declaration in the XML plugin file. In old XML plugin 
        /// format (v0.0), there is not defined DTD declaration like 
        /// @code{.xml} <!DOCTYPE plugin SYSTEM "Resources\plugin_v1.0.dtd"> @endcode 
        /// or attribute for 
        /// @code{.xml} <plugin plugin_system_version="1.0"> @endcode 
        /// XML element, so both conditions are failed together. In V1.0 XML plugin format, 
        /// both conditions must be fulfilled together for a valid plugin.
        /// @since v15.03.26 - Added.
        private bool ValidatePluginXML(XmlReader XR)
        {
            //read the XML, if it is possible
            while (XR.Read())
            {
                //look for the element 'plugin' in the XML
                if ((XR.NodeType == XmlNodeType.Element) && (XR.Name == "plugin"))
                {
                    //check if readable and having attributes
                    if ((XR.ReadState == ReadState.Interactive) && XR.HasAttributes)
                        //if I can move to it, it is readable
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
        /// @post This implementation assumes the existence of attribute plugin_system_version
        /// linked to existence of DTD declaration in the XML plugin file. In old XML plugin 
        /// format (v0.0), there is not defined DTD declaration like 
        /// @code{.xml} <!DOCTYPE plugin SYSTEM "Resources\plugin_v1.0.dtd"> @endcode 
        /// or attribute for 
        /// @code{.xml} <plugin plugin_system_version="1.0"> @endcode 
        /// XML element, so both conditions are failed together. In V1.0 XML plugin format, 
        /// both conditions must be fulfilled together for a valid plugin.
        /// @since v15.03.26 - Added.
        public bool ValidatePluginFile(string filenameXml)
        {
            //set the main file containing the plugin definition
            this.MainFile = filenameXml;
            //Settings to be used to validate the XML
            // reference from https://msdn.microsoft.com/en-us/library/vstudio/z2adhb2f%28v=vs.100%29.aspx
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;   //need to look for DTD definition
            settings.ValidationType = ValidationType.DTD;   //validate XML against DTD
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidatePluginXMLEventHandler);
            XmlUrlResolver myResolver = new DTDResolver();
            myResolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            settings.XmlResolver = myResolver;
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            
            try
            {
                //Open a XML reader with the file name and settings given
                XmlReader XR = XmlReader.Create(MainFile, settings);
                //validate the file as it is, expecting it have embedded DTD definition
                if (ValidatePluginXML(XR)) 
                    return true;
                else switch (this.PluginSystemVersion)
                {   //case of not valid XML, so let's see the plugin system version detected
                    case null:
                        //we have to try adding default DTD definition to the XML source, because
                        // the 0.0 version didn't have DTD specification on it.
                        ValidationErrors.Clear();   //clear the warnings from failed validation
                        this.isValid = true;		//resetting the initial status to test for validity
                        this.errorType = PluginDataErrorType.none;
                        // Add the DTD definition to XML with a XmlReader
                        // reference from http://stackoverflow.com/questions/470313/net-how-to-validate-xml-file-with-dtd-without-doctype-declaration
                        // and from http://stackoverflow.com/questions/10514198/validate-xml-against-dtd-from-string
                        XmlDocument doc = new XmlDocument();
                        XmlReaderSettings settingsNoDTD = settings.Clone();
                        settingsNoDTD.DtdProcessing = DtdProcessing.Ignore;
                        settingsNoDTD.ValidationType = ValidationType.None;
                        //reopen the XmlReader with no DTD validation
                        XR = XmlReader.Create(MainFile, settingsNoDTD);
                        doc.Load(XR);
                        doc.XmlResolver = myResolver;
                        doc.InsertBefore(
                            doc.CreateDocumentType("plugin", null, @"Resources\plugin_v0.0.dtd", null),
                            doc.DocumentElement);
                        //save to a memory stream
                        MemoryStream mem = new MemoryStream();
                        doc.Save(mem);
                        mem.Position = 0;   //restart the position of the stream
                        //we will read from the memory stream, with the original settings
                        XR = XmlReader.Create(mem, settings);
                        //validate from the XML source with default DTD definition
                        if (ValidatePluginXML(XR))
                            return true;
                        else
                        {
                            this.errorType = PluginDataErrorType.nonDTDComplaint;
                            return false;
                        }
                    case "1.0": //this means the plugin is not valid, because 1.0 version must have
                                // DTD explicit on it!
                        this.errorType = PluginDataErrorType.nonDTDComplaint;
                        return false;
                    default:    //for others future version, we don't know how to interpret it yet
                        this.errorType = PluginDataErrorType.nonDTDComplaint;
                        return false; 
                }
            }
            catch (FileNotFoundException e)
            {
                this.isValid = false;
                this.errorType = PluginDataErrorType.openFile;
                ValidationErrors.Add(
                    string.Format("File \"{0}\" not found, when validating original XML file at '{1}'",
                    e.FileName,
                    e.TargetSite));
                return false;
            }
            catch (ArgumentNullException)
            {
                this.isValid = false;
                this.errorType = PluginDataErrorType.openFile;
                ValidationErrors.Add(
                    "No filename was given, when validating original XML file.");
                return false;
            }
            catch (Exception e)
            {
                this.isValid = false;
                this.errorType = PluginDataErrorType.other;
                ValidationErrors.Add(
                    string.Format("Error found: \"{0}\"", e.Message));
                return false;
            }
        }
    }

    
    /// @brief Methods to save and retrieve plugin from files, managing version of plugin system.
    /// @details To manage version of plugin system enables to have different signatures of 
    /// methods or create new properties in plugins to evolve the system, trying to have
    /// compatibility with old plugins.
    /// @since v15.03.26 - Added.
    static class PluginPersistence
    {

        /// @brief Save plugin data to XML as plugin system version 0.0.
        /// @param[in] filenameXml File name in XML format, version 0.0
        /// @param[in] Data Metadata of the plugin.
        /// @returns State of saving, as success (=true), or failure (=false).
        /// @since v15.03.26 - Added.
        static public bool SaveDatoToXML_v0_0(string filenameXml, PluginData Data)
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
            return true;
        }

        /// @brief Save a plugin data to XML as plugin system version 1.0.
        /// @param[in] filenameXml File name in XML format, version 1.0
        /// @param[in] Data Metadata of the plugin.
        /// @returns State of saving, success (=true) or fail (=false).
        /// @since v15.03.26 - Added.
        static public bool SaveDatoToXML_v1_0(string filenameXml, PluginData Data)
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
            XmlDocumentType doctype = xmlDoc.CreateDocumentType("plugin", null, @"Resources\plugin_v1.0.dtd", null);
            xmlDoc.AppendChild(doctype);
            //Main element - plugin
            XmlElement root = xmlDoc.CreateElement("plugin");
            root.SetAttribute("plugin_system_version", Data.PluginSystemVersion);
            root.SetAttribute("version", Data.metaData.PluginVersion);
            xmlDoc.AppendChild(root);
            //level 1 element - metadata
            instance = xmlDoc.CreateElement("metadata");
            root.AppendChild(instance);
            {
                //level 2 elements - author
                if (Data.metaData.Authors != null)
                {
                    bool isValidAuth, isFirst = true;
                    foreach (string Author in Data.metaData.Authors)
                    {
                        isValidAuth = !string.IsNullOrEmpty(Author);
                        if (isFirst | isValidAuth)
                        {
                            childElement = xmlDoc.CreateElement("author");
                            instance.AppendChild(childElement);
                            isFirst = false;
                            if (isValidAuth)
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
                if (Data.metaData.Modifier.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.metaData.Modifier);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - date_modified
                childElement = xmlDoc.CreateElement("date_modified");
                instance.AppendChild(childElement);
                if (Data.metaData.DateModified.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.metaData.DateModified);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - cultural_reference
                childElement = xmlDoc.CreateElement("cultural_reference");
                instance.AppendChild(childElement);
                if (Data.metaData.CulturalReference.Length > 0)
                {
                    textElement = xmlDoc.CreateTextNode(Data.metaData.CulturalReference);
                    childElement.AppendChild(textElement);
                }
                //level 2 element - release_notes
                childElement = xmlDoc.CreateElement("release_notes");
                instance.AppendChild(childElement);
                if (Data.metaData.ReleaseNotes.Length > 0)
                {
                    cdata = xmlDoc.CreateCDataSection(Data.metaData.ReleaseNotes);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - description
                childElement = xmlDoc.CreateElement("description");
                instance.AppendChild(childElement);
                if (Data.metaData.Description.Length > 0)
                {
                    cdata = xmlDoc.CreateCDataSection(Data.metaData.Description);
                    childElement.AppendChild(cdata);
                }
                //level 2 element - usage
                childElement = xmlDoc.CreateElement("usage");
                instance.AppendChild(childElement);
                if (Data.metaData.Usage.Length > 0)
                {
                    cdata = xmlDoc.CreateCDataSection(Data.metaData.Usage);
                    childElement.AppendChild(cdata);
                }
                //level 2 elements - link
                if (Data.metaData.Links != null)
                {
                    bool isValidLink, isFirst = true; 
                    foreach (string link in Data.metaData.Links)
                    {
                        isValidLink = !string.IsNullOrEmpty(link);
                        if (isFirst | isValidLink)
                        {
                            childElement = xmlDoc.CreateElement("link");
                            instance.AppendChild(childElement);
                            isFirst = false;
                            if (isValidLink)
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
            return true;
        }

        /// @brief Extract data of plugin system version 0.0 from XML.
        /// @param[in] filenameXml File name in XML format, version 0.0
        /// @param[in,out] Data Metadata of the plugin.
        /// @returns State of loading: True if it was successful (also filling Data parameter 
        /// with the plugin information), or False if didn't (in this case Data parameter
        /// only should have updated IsValid attribute).
        /// @since v15.03.26 - Added.
        static public bool GetDataFromXML_v0_0(string filenameXml, ref PluginData Data)
        {
            //Settings to read the XML
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
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
                            if (!XR.IsEmptyElement)
                                //Save the name to associate it to the content
                                lastElement.Push(XR.Name);
                            if ((XR.ReadState == ReadState.Interactive) && XR.HasAttributes)
                            {
                                if (XR.MoveToFirstAttribute())
                                    do
                                    {
                                        switch (XR.Name)
                                        {
                                            case "class":
                                                Data.InstanceName = XR.Value;
                                                break;
                                            case "name":
                                                //add to the list of references
                                                referencesTmp.Add(XR.Value);
                                                break;
                                            default:
                                                break;
                                        }
                                    } while (XR.MoveToNextAttribute());
                            }
                            break;
                        case XmlNodeType.EndElement:
                            lastElement.Pop();  //delete it
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
                else
                    Data.References = new string[1];
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
                else
                {
                    //create empty arrays to avoid null references
                    Data.UseExtFiles = new bool[1];
                    Data.UseExtFiles[0] = false;
                    Data.ExtFiles = new string[1];
                    Data.Codes = new string[1];
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

        /// @brief Extract data of plugin system version 1.0 from XML.
        /// @param[in] filenameXml File name in XML format, version 1.0
        /// @param[in,out] Data Metadata of the plugin.
        /// @returns State of loading: True if it was successful (also filling Data parameter 
        /// with the plugin information), or False if didn't (in this case Data parameter
        /// only should have updated IsValid attribute).
        /// @note The date conversion to current locale should be made in front end. Here it is
        /// only extracted and stored in Data.DateModified member without conversion.
        /// @since v15.03.26 - Added.
        static public bool GetDataFromXML_v1_0(string filenameXml, ref PluginData Data)
        {
            //Settings to read the XML
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.IgnoreComments                 = true;
            settings.IgnoreProcessingInstructions   = true;
            settings.IgnoreWhitespace               = true;
            settings.ValidationEventHandler += 
                new ValidationEventHandler(Data.ValidatePluginXMLEventHandler);
            XmlUrlResolver resolver = new DTDResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            settings.XmlResolver = resolver;
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
                Dictionary<int, bool>   useExtFilesTmp     = new Dictionary<int, bool>();
                Dictionary<int, string> extFilesTmp        = new Dictionary<int, string>();
                Dictionary<int, string> codesTmp           = new Dictionary<int, string>();
                Dictionary<int, int>    orderCodesReverse  = new Dictionary<int, int>();
                int codeQty = 0;    //count the code sections encountered
                //read the data from XML, assigning to corresponding PluginData member
                while (XR.Read())
                {
                    switch (XR.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!XR.IsEmptyElement)
                                //Save the name to associate the content with the section
                                lastElement.Push(XR.Name);  
                            if (XR.Name == "code")
                                codeQty++;              //increment the code sections
                            if ((XR.ReadState == ReadState.Interactive) && XR.HasAttributes)
                            {
                                if (XR.MoveToFirstAttribute())
                                    do 
                                    {
                                        switch (XR.Name)
                                        {
                                            case "version":
                                                Data.metaData.PluginVersion = XR.Value;
                                                break;
                                            case "ref":
                                                extFilesTmp.Add(codeQty, XR.Value);
                                                useExtFilesTmp.Add(codeQty, true);
                                                break;
                                            case "order":
                                                orderCodesReverse.Add(int.Parse(XR.Value), 
                                                    codeQty);
                                                break;
                                            default:
                                                break;
                                        }
                                    } while (XR.MoveToNextAttribute());
                            }
                            break;

                        case XmlNodeType.EndElement:
                            //delete the name of section, because it ended in XML file
                            lastElement.Pop();  
                            break;

                        case XmlNodeType.Text:
                            switch (lastElement.Peek())
                            {
                                case "author":
                                    authorsTmp.Add(XR.Value);
                                    break;
                                case "modified_by":
                                    Data.metaData.Modifier = XR.Value;
                                    break;
                                case "date_modified":
                                    Data.metaData.DateModified = XR.Value;
                                    break;
                                case "cultural_reference":
                                    Data.metaData.CulturalReference = XR.Value;
                                    break;
                                case "release_notes":
                                    Data.metaData.ReleaseNotes = XR.Value;
                                    break;
                                case "description":
                                    Data.metaData.Description = XR.Value;
                                    break;
                                case "usage":
                                    Data.metaData.Usage = XR.Value;
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
                                    codesTmp.Add(codeQty, XR.Value);    //add to the list of codes
                                    useExtFilesTmp.Add(codeQty, false);
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case XmlNodeType.CDATA:
                            switch (lastElement.Peek())
                            {
                                case "release_notes":
                                    Data.metaData.ReleaseNotes = XR.Value;
                                    break;
                                case "description":
                                    Data.metaData.Description = XR.Value;
                                    break;
                                case "usage":
                                    Data.metaData.Usage = XR.Value;
                                    break;
                                case "link":
                                    linksTmp.Add(XR.Value); //add to the list of links
                                    break;
                                case "code":
                                    codesTmp.Add(codeQty, XR.Value);    //add to the list of codes
                                    useExtFilesTmp.Add(codeQty, false);
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
                //construct the array of authors
                if (authorsTmp.Count > 0)
                    Data.metaData.Authors = authorsTmp.ToArray();
                else
                    Data.metaData.Authors = new string[1];
                //convert date to current culture
                DateTimeFormatInfo readInfo = 
                    new CultureInfo(Data.metaData.CulturalReference, false).DateTimeFormat;
                DateTimeFormatInfo currInfo = 
                    System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                Data.metaData.DateModified = 
                    Convert.ToDateTime(Data.metaData.DateModified, readInfo).ToString(currInfo.ShortDatePattern);
                //construct the array of links
                if (linksTmp.Count > 0)
                    Data.metaData.Links = linksTmp.ToArray();
                else
                    Data.metaData.Links = new string[1];
                //construct the array of references
                if (referencesTmp.Count > 0)
                    Data.References = referencesTmp.ToArray();
                else
                    Data.References = new string[1];
                //construct the array for codes, external files & etc, ordering by the 
                // specified order of the XML file
                if (codeQty > 0)
                {
                    int idx = 0; bool boolVal; string strVal;
                    Data.UseExtFiles = new bool[codeQty];
                    Data.ExtFiles    = new string[codeQty];
                    Data.Codes       = new string[codeQty];
                    for (int i=1; i <= codeQty; i++)
                    {
                        //retrieve by index corresponding with original order in XML
                        if (orderCodesReverse.TryGetValue(i, out idx))
                        {
                            if (useExtFilesTmp.TryGetValue(idx, out boolVal))
                                Data.UseExtFiles[i-1] = boolVal;
                            if (boolVal && extFilesTmp.TryGetValue(idx, out strVal))
                                Data.ExtFiles[i-1] = strVal;
                            if (!boolVal && codesTmp.TryGetValue(idx, out strVal))
                                Data.Codes[i-1] = strVal;
                        }
                    }
                }
                else
                {
                    //create empty arrays to avoid null references
                    Data.UseExtFiles = new bool[1];
                    Data.ExtFiles = new string[1];
                    Data.Codes = new string[1];
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

    }
}