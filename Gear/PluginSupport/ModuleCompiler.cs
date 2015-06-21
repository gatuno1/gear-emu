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
/// ModuleCompiler.cs is the new name of old file ModuleLoader.cs. This is more appropriate
/// as a description of what it does.
/// @since v15.03.26 - Changed the file name from ModuleLoader.cs.

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Gear.PluginSupport
{
    /// @brief Delegate to operate on compiler error of a plugin code compilation.
    /// @param e Compiler error to enumerate.
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
        
        /// @brief Completes the version.
        /// @param version Version string to complete as Assembly standards (w/ 4 digits).
        /// @returns A complete version string.
        static public string CompleteVersion(string version)
        {
            string[] parts = version.Split('.');
            string aux = "";
            for(int i = 0; i < 4; i++)
            {
                if ((parts.Length <= i) || (string.IsNullOrEmpty(parts[i])))
                    aux += "0";
                else
                    aux += parts[i];
                aux += ((i < 3) ? "." : "");
            }
            return aux;
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
        
        /// @brief Generate the name of the compiled plugin.
        /// @param module Name of the plugin module.
        /// @returns Name of the compiled plugin.
        static private string CompiledPluginName(string module, string pluginSystemVersion, string extension)
        {
            return string.Format("PluginSystemV{0}.{1}{2}", 
                pluginSystemVersion.Replace(".", "_"),
                module,
                (!extension.Contains(".")) ? ("." + extension) : extension);
        }

        /// @brief Dynamic compiling & loading for a plugin.
        /// @details Try to dynamically compile a module for the plugin, based on supplied 
        /// C# code and other C# modules referenced, using Reflexion. If the compiling fails,  
        /// it gives a list of errors, intended to be showed in the plugin view.
        /// @param[in] code C# Source code based on PluginBase class, to implement your plugin.
        /// @param[in] module Class name of the plugin.
        /// @param[in] references String array with auxiliary references used by your plugin. 
        /// See notes for defaults used.
        /// @param[in] objParams Reference to a PropellerCPU of this instance, to be passed 
        /// as a parameter to the constructor of the new plugin class instance.
        /// @param[in] pluginSystemVersion Witch plugin system version to compile.
        /// @returns New Plugin class instance compiled (on success), or NULL (on fail).
        /// @throws Any compiling exception is detected and thrown again to the caller of 
        /// this method.
        /// @note There are some references already added, so you don't need to include on your 
        /// plugins: 
        /// @li `using System;` @li `using System.Data;` @li `using System.Drawing;`
        /// @li `using System.Windows.Forms;` @li `using System.Xml;`
        /// @version v15.03.26 - added parameter pluginBaseClass.
        static public PluginCommon LoadModule(string[] codeTexts, string[] sourceFiles, 
            string module, string[] references, object objParams, string pluginSystemVersion, 
            string pluginVersion)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.OutputAssembly = ".\\" + CompiledPluginName(module, pluginSystemVersion, "dll");
            string aux = CompleteVersion(pluginVersion);
#if DEBUG
            cp.IncludeDebugInformation = true;
            string[] codeFileName = (string[])sourceFiles.Clone();
            for(int i = 0; i < codeFileName.Length; i++)
            {
                if (string.IsNullOrEmpty(codeFileName[i]))
                    codeFileName[i] = cp.OutputAssembly.Replace(".dll",
                        (codeFileName.Length > 1) ? string.Format("{0}.cs", i) : ".cs");
                else
                    codeFileName[i] = ".\\" + Path.GetFileName(codeFileName[i]);
                File.WriteAllText(codeFileName[i], codeTexts[i]);
            }
#else
            cp.IncludeDebugInformation = false;
#endif
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.CompilerOptions = "/optimize";
            cp.WarningLevel = 4;    //to do not consider C00618 warning (obsolete PluginBaseV0_0 class)
            cp.MainClass = "Gear.PluginSupport." + module;
            cp.ReferencedAssemblies.Add(System.Windows.Forms.Application.ExecutablePath);

            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            foreach (string s in references)    //traverse list adding not null or empty texts
                if (!string.IsNullOrEmpty(s))
                    cp.ReferencedAssemblies.Add(s);
            try
            {
                //first compile source codes
#if DEBUG
                CompilerResults results = provider.CompileAssemblyFromFile(cp, codeFileName);
#else
                CompilerResults results = provider.CompileAssemblyFromSource(cp, codeTexts);
#endif

                if (results.Errors.HasErrors | results.Errors.HasWarnings)
                {
                    m_Errors = results.Errors;
                    return null;
                }

                PrintAssembliesLoaded(results.CompiledAssembly, true);
                //then instantiate plugin class
                object target = results.CompiledAssembly.CreateInstance(
                    module,                                         //string typeName
                    false,                                          //bool ignoreCase
                    BindingFlags.Public | BindingFlags.Instance,    //BindingFlags bindingAttr
                    null,                                           //Binder binder
                    (objParams != null) ?                           //object[] args
                        new object[] { objParams } : 
                        null,                   
                    null,                                           //CultureInfo culture
                    null);                                          //object[] activationAttributes

                PrintAssembliesLoaded(results.CompiledAssembly, false);
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
                        "' to '" + PluginSystem.GetPluginBaseClass(pluginSystemVersion).FullName + "'");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }

                m_Errors = null;
                return (PluginCommon)target;
            }
            catch (Exception e)
            {
                CompilerError c = new CompilerError("", 0, 0, "Runtime",
                    string.Format("Plugin '{0}' - {1}", module, e.Message));
                m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                return null;
            }
        }
    
        static public void PrintAssembliesLoaded(Assembly Assem, bool overwrite)
        {
            //temporally debug code to find assemblies loaded in the current domain
            Assembly[] all = AppDomain.CurrentDomain.GetAssemblies();
            StreamWriter f = (overwrite == true) ?
                File.CreateText(".\\temp.txt") :
                File.AppendText(".\\temp.txt");
            all = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in all)
            {

                f.WriteLine(a.FullName + " - " + a.IsDynamic); 
                f.Flush();
                if ((a.FullName == Assem.FullName))
                {
                    if (a.Equals(Assem))
                    {
                        f.WriteLine("found itself");
                    }
                    else
                    {
                        f.WriteLine("Assembly duplicated!");
                    }
                }
            }
            f.WriteLine("================================================");
            f.Close();

        }
    }

}
