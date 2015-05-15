/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * ModuleCompiler.cs
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

/// @file 
/// ModuleCompiler.cs is the new name of old file ModuleLoader.cs. This is more appropiate
/// as a description of what it does.
/// @since v15.03.26 - Changed the file name from ModuleLoader.cs.

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Gear.PluginSupport
{
    public delegate void ErrorEnumProc(System.CodeDom.Compiler.CompilerError e);

    /// @brief Compile a PluginBase Module to memory, returning eventual errors.
    static class ModuleCompiler
    {
        /// @brief Collection for error list on compile a dynamic plugin.
        static private CompilerErrorCollection m_Errors;    

        /// @brief ModuleCompiler Constructor.
        /// @details Clear error list by default.
        static ModuleCompiler()
        {
            m_Errors = null;
        }

        /// @brief Enumerate errors from the compiling process.
        /// @param[in] proc Method to invoke for each error.
        static public void EnumerateErrors(ErrorEnumProc proc)
        {
            if (m_Errors == null)
                return;

            foreach (CompilerError e in m_Errors)
                proc(e);
        }

        /// @brief Dynamic compiling & loading for a plugin.
        /// @details Try to dynamically compile a module for the plugin, based on supplied C# code
        /// and other C# modules referenced. If the compiling fails, it gives a list of errors, 
        /// intended to be showed in the plugin view.
        /// @param[in] code C# Source code based on PluginBase class, to implement your plugin.
        /// @param[in] module Class name of the plugin.
        /// @param[in] references String array with auxiliary references used by your plugin. 
        /// See notes for defaults used.
        /// @param[in] obj Reference to a PropellerCPU of this instance, to be passed as a 
        /// parameter to the constructor of the new plugin class instance.
        /// @param[in] pluginBaseClass Type of the target class to compile.
        /// @returns New Plugin class instance compiled (on success), or NULL (on fail).
        /// @throws Any compiling exception is detected and thrown again to the caller of this method.
        /// @note There are some references already added, so you don't need to include on your plugins: 
        /// @li `using System;` @li `using System.Data;` @li `using System.Drawing;`
        /// @li `using System.Windows.Forms;` @li `using System.Xml;`
        /// 
        /// @version v15.03.26 - added parameter pluginBaseClass.
        static public object LoadModule(string code, string module, string[] references, 
            object obj, Type pluginBaseClass)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.IncludeDebugInformation = false;
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";

            cp.ReferencedAssemblies.Add(System.Windows.Forms.Application.ExecutablePath);

            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            foreach (string s in references)
                cp.ReferencedAssemblies.Add(s);

            CompilerResults results = provider.CompileAssemblyFromSource(cp, code);

            if (results.Errors.HasErrors | results.Errors.HasWarnings)
            {
                m_Errors = results.Errors;
                return null;
            }

            //compile plugin with parameters
            try
            {
                object target = results.CompiledAssembly.CreateInstance(
                    module,                                         //name of class
                    false,                                          //=false: case sensitive
                    BindingFlags.Public | BindingFlags.Instance,    //flags to delimit the candidates
                    null,                                           //default binder object
                    new object[] { obj },                           //parameter lists
                    null,                                           //default culture
                    null                                            //default activation object
                );
                if (target == null)
                {
                    CompilerError c = new CompilerError("", 0, 0, "CS0103",
                        "The name '" + module + "' does not exist in the current context." +
                        " Does the class name is the same that is declared in c# code?");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }
                else if (!PluginSystem.InstanceOneOfValidClasses(target))
                {
                    CompilerError c = new CompilerError("", 0, 0, "CS0029",
                        "Cannot implicitly convert type '" + target.GetType().FullName +
                        "' to '" + pluginBaseClass.FullName + "'");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }

                m_Errors = null;
                return target;
            }
            catch (Exception e)
            {
                throw new Exception("Exception generated when compiling plugin class into memory. " + "Message: \"" + e.Message + "\"", e);
            }
        }
    
    }

    /// @brief Provides information about the plugin system.
    /// @since v15.03.26 - Added.
    public static class PluginSystem 
    {
        /// @brief Type of plugin base class for current 
        public static Type GetPluginBaseClass(string pluginSystemVersion)
        {
            if (!string.IsNullOrEmpty(pluginSystemVersion))
                switch (pluginSystemVersion)
                {
                    case "1.0":
                        return typeof(PluginBase);
                    case "0.0":
#pragma warning disable 618
                        return typeof(PluginBaseV0_0);
#pragma warning restore 618
                    default:
                        throw new Exception(string.Format(
                            "Plugin system version '{0}' not recognized", pluginSystemVersion));
                }
            else throw new Exception("Parameter pluginSystemVersion is null or empty, on " +
                "calling of 'PluginData.pluginBaseClass(string pluginSystemVersion)'!");
        }

        /// @brief Determine if the object passed as parameter is on of the valid Plugin 
        /// base classes
        public static bool InstanceOneOfValidClasses(object obj)
        {
            if (obj == null)
                return false;
            else
                return (
                    (obj is PluginBase) ||
#pragma warning disable 618
                    (obj is PluginBaseV0_0) 
#pragma warning restore 618
                    );
        }
    
    }

}