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

/// @copydoc Gear.PluginSupport
namespace Gear.PluginSupport
{
    /// @brief Some Methods to save and retrieve plugin from files
    static class PluginPersistence
    {
        ///@brief
        ///
        public struct PluginData
        {
            public string PluginSystemVersion;
            public string PluginVersion;

            public string[] Authors;
            public string Modifier;
            public string DateModified;
            public string Description;
            public string Usage;
            public string[] Links;

            public string InstanceName;
            public string[] References;

            public bool[] UseAuxFiles;
            public string[] AuxFiles;
            public string[] Codes;
        }

        /// @brief Save a plugin to XML as version 0.0
        /// @returns True always
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
            foreach (string s in Data.References)
            {
                instance = xmlDoc.CreateElement("reference");
                instance.SetAttribute("name", s);
                root.AppendChild(instance);
            }
            //level 1 element - code
            instance = xmlDoc.CreateElement("code");
            if (!Data.UseAuxFiles[0])
                instance.AppendChild(xmlDoc.CreateTextNode(Data.Codes[0]));
            else 
                instance.SetAttribute("ref",Data.AuxFiles[0]);
            root.AppendChild(instance);
            //saving xml document
            xmlDoc.Save(filenameXml);

            //TODO [ASB] : catch exceptions and return false if any.
            return true;
        }

        /// @brief Save a plugin to XML as version 1.0
        /// @returns True always
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
            //level 1 element - about
            instance = xmlDoc.CreateElement("about");
            instance.SetAttribute("version", Data.PluginVersion);
            root.AppendChild(instance);
            {
                //level 2 elements - author
                if (Data.Authors != null)
                {
                    foreach (string s in Data.Authors)
                    {
                        childElement = xmlDoc.CreateElement("author");
                        textElement = xmlDoc.CreateTextNode("");
                        cdata = xmlDoc.CreateCDataSection(s);
                        instance.AppendChild(childElement);
                        childElement.AppendChild(textElement);
                        textElement.AppendChild(cdata);
                    }
                }
                else
                {
                    childElement = xmlDoc.CreateElement("author");
                    instance.AppendChild(childElement);
                }
                //level 2 element - modified_by
                childElement = xmlDoc.CreateElement("modified_by");
                textElement = xmlDoc.CreateTextNode("");
                cdata = xmlDoc.CreateCDataSection(Data.Modifier);
                instance.AppendChild(childElement);
                childElement.AppendChild(textElement);
                textElement.AppendChild(cdata);
                //level 2 element - date_modified
                childElement = xmlDoc.CreateElement("date_modified");
                textElement = xmlDoc.CreateTextNode("");
                cdata = xmlDoc.CreateCDataSection(Data.DateModified);
                instance.AppendChild(childElement);
                childElement.AppendChild(textElement);
                textElement.AppendChild(cdata);
                //level 2 element - description
                childElement = xmlDoc.CreateElement("description");
                textElement = xmlDoc.CreateTextNode("");
                cdata = xmlDoc.CreateCDataSection(Data.Description);
                instance.AppendChild(childElement);
                childElement.AppendChild(textElement);
                textElement.AppendChild(cdata);
                //level 2 element - usage
                childElement = xmlDoc.CreateElement("usage");
                textElement = xmlDoc.CreateTextNode("");
                cdata = xmlDoc.CreateCDataSection(Data.Usage);
                instance.AppendChild(childElement);
                childElement.AppendChild(textElement);
                textElement.AppendChild(cdata);
                //level 2 elements - link
                foreach (string s in Data.Links)
                {
                    childElement = xmlDoc.CreateElement("link");
                    textElement = xmlDoc.CreateTextNode("");
                    cdata = xmlDoc.CreateCDataSection(s);
                    instance.AppendChild(childElement);
                    childElement.AppendChild(textElement);
                    textElement.AppendChild(cdata);
                }
            }
            //level 1 element - instance
            instance = xmlDoc.CreateElement("instance");
            instance.SetAttribute("class", Data.InstanceName);
            root.AppendChild(instance);
            //level 1 elements - reference
            foreach (string s in Data.References)
            {
                instance = xmlDoc.CreateElement("reference");
                instance.SetAttribute("name", s);
                root.AppendChild(instance);
            }
            //level 1 elements - code
            for (int i = 0; i < Data.UseAuxFiles.Rank; i++)
            {
                instance = xmlDoc.CreateElement("code");
                if (!Data.UseAuxFiles[i])
                    instance.AppendChild(xmlDoc.CreateTextNode(Data.Codes[i]));
                else
                    instance.SetAttribute("ref", Data.AuxFiles[i]);
                root.AppendChild(instance);
            }
            //saving xml document
            xmlDoc.Save(filenameXml);

            //TODO [ASB] : catch exceptions and return false if any.
            return true;
        }

    }
}