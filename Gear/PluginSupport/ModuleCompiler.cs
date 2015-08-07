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

    public class ModuleCompiler : MarshalByRefObject
    {
        /// @brief Collection for error list on compile a dynamic plugin.
        private CompilerErrorCollection errorsCollection;
        /// @brief data of plugin to compile
        private PluginDataStruct m_pluginData;
        /// @brief StaticModuleCompiler Constructor.
        /// @details Clear error list by default.
        public ModuleCompiler(PluginDataStruct pluginData)
        {
            errorsCollection = null;
            m_pluginData = pluginData;
        }

        /// @brief Compiles in memory an assembly containing the plugin.
        /// @param compiledName 
        public Assembly CompileToMemory(string compiledName)
        {
            CompilerParameters cp = new CompilerParameters(
                new[] { "System.Windows.Forms.dll", "System.dll", "System.Data.dll", "System.Drawing.dll", "System.Xml.dll" },  //references added by default
                compiledName,   //name of the compiled assembly
                false);         //false = no debug
            cp.ReferencedAssemblies.Add(System.Windows.Forms.Application.ExecutablePath);
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.CompilerOptions = "/optimize";
            cp.WarningLevel = 4;    //to do not consider C00618 warning (obsolete PluginBaseV0_0 class)
            cp.MainClass = "Gear.PluginSupport." + m_pluginData.InstanceName;
            //traverse list adding not null nor empty texts
            foreach (string s in m_pluginData.References)
                if (!string.IsNullOrEmpty(s))
                    cp.ReferencedAssemblies.Add(s);
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            //compile the assembly
            CompilerResults results = provider.CompileAssemblyFromFile(cp, m_pluginData.Codes);
            //check if there are errors
            if (results.Errors.HasErrors | results.Errors.HasWarnings)
            {
                errorsCollection = results.Errors;
                return null;
            }
            return results.CompiledAssembly;
        }
    }

    /// @brief Compile a PluginBase Module to memory, returning eventual errors.
    static class StaticModuleCompiler
    {
        /// @brief path to compiling cache
        static public readonly string chachePath = @".\plugincache\";
        
        /// @brief Collection for error list on compile a dynamic plugin.
        static private CompilerErrorCollection m_Errors;    

        /// @brief StaticModuleCompiler Constructor.
        /// @details Clear error list by default.
        static StaticModuleCompiler()
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

        /// @brief Get the date/time of the build of the Assembly file.
        /// @param AssemblyFilePath Path to the file of the assembly to extract date/time.
        /// @returns DateTime of the Assembly or null if it couldn't be retrieved.
        /// @since v15.03.26 - Added.
        static private DateTime RetrieveLinkerTimestamp(string AssemblyFilePath)
        {
            //offset for the linker time stamp from the PE header of the assembly
            // from http://stackoverflow.com/questions/1600962/displaying-the-build-date
            // also see https://upload.wikimedia.org/wikipedia/commons/7/70/Portable_Executable_32_bit_Structure_in_SVG.svg
            const int c_PeHeaderOffset = 60;        //beginning of PEHeader
            const int c_LinkerTimestampOffset = 8;  //offset from PEHeader
            byte[] b = new byte[2048];              //buffer for the beggini
            Stream s = null;

            try
            {
                s = new System.IO.FileStream(AssemblyFilePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            catch
            {
                return new DateTime();
            }
            finally
            {
                if (s != null)
                    s.Close();
            }

            //Get the pointer to PE Header
            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            //Get the TimeDateStamp
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }
        
        /// @brief Dynamic compiling & loading for a plugin.
        /// @details Try to dynamically compile a module for the plugin, based on supplied 
        /// C# code and other C# modules referenced, using Reflexion. If the compiling fails,  
        /// it gives a list of errors, intended to be showed in the plugin view.
        /// @param[in] codeTexts C# Source code based on PluginBase class, to implement the plugin.
        /// @param[in] sourceFiles Source file (with path).
        /// @param[in] module Class name of the plugin.
        /// @param[in] references String array with auxiliary references used by your plugin. 
        /// See notes for defaults used.
        /// @param[in] objParams Reference to a PropellerCPU of this instance, to be passed 
        /// as a parameter to the constructor of the new plugin class instance.
        /// @param[in] pluginSystemVersion Witch plugin system version to compile.
        /// @returns New Plugin class instance compiled (on success), or NULL (on fail).
        /// @note There are some references already added, so you don't need to include on your 
        /// plugins: 
        /// @li `using System;` @li `using System.Data;` @li `using System.Drawing;`
        /// @li `using System.Windows.Forms;` @li `using System.Xml;`
        /// @version v15.03.26 - Modified the logic.
        static public PluginCommon LoadModule(string[] codeTexts, string[] sourceFiles, 
            string module, string[] references, object objParams, string pluginSystemVersion)
        {
            const string chachePath = @".\plugincache\";
            string compiledName = 
                AssemblyUtils.CompiledPluginName(module, pluginSystemVersion, ".dll");
            CompilerParameters cp = new CompilerParameters( 
                new[] { "System.Windows.Forms.dll", "System.dll", "System.Data.dll", 
                        "System.Drawing.dll", "System.Xml.dll" },
                chachePath + compiledName, 
                false);

            if (!Directory.Exists(chachePath))
                Directory.CreateDirectory(chachePath);
            cp.TempFiles = new TempFileCollection(chachePath, false);  //set directory
            cp.ReferencedAssemblies.Add(System.Windows.Forms.Application.ExecutablePath);
            cp.IncludeDebugInformation = false;
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";
            cp.WarningLevel = 4;    //to do not consider C00618 warning (obsolete PluginBaseV0_0 class)
            cp.MainClass = "Gear.PluginSupport." + module;
            //traverse list adding not null nor empty texts
            foreach (string s in references)    //traverse list adding not null or empty texts
                if (!string.IsNullOrEmpty(s))
                    cp.ReferencedAssemblies.Add(s);

            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            try
            {
                //compile source codes into a memory assembly
                CompilerResults results = provider.CompileAssemblyFromSource(cp, codeTexts);

                if (results.Errors.HasErrors | results.Errors.HasWarnings)
                {
                    m_Errors = results.Errors;
                    return null;
                }
//#if DEBUG
//                //write the code to file to enable debug
//                for (int i = 0; i < codeTexts.Length; i++ )
//                    File.WriteAllText(
//                        codefile.Replace(".cs", string.Format(".{0}.cs",i)), 
//                        codeTexts[i] );
//#endif
                PrintAssembliesLoaded(AppDomain.CurrentDomain, results.CompiledAssembly, true);
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

                PrintAssembliesLoaded(AppDomain.CurrentDomain, results.CompiledAssembly, false);
                if (target == null)
                {
                    CompilerError c = new CompilerError(string.Empty, 0, 0, "CS0103",
                        "The name '" + module + "' does not exist in the current context." +
                        " Does the class name is the same that is declared in c# code?");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }
                else if (!PluginSystem.InstanceOneOfValidClasses(target))
                {
                    CompilerError c = new CompilerError(string.Empty, 0, 0, "CS0029",
                        "Cannot implicitly convert type '" + target.GetType().FullName +
                        "' to '" + PluginSystem.GetPluginBaseType(pluginSystemVersion).FullName + "'");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }

                m_Errors = null;
                return (PluginCommon)target;
            }
            catch (Exception e)
            {
                CompilerError c = new CompilerError(string.Empty, 0, 0, "Runtime",
                    string.Format("Plugin '{0}' - {1}", module, e.Message));
                m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                return null;
            }
        }

        /// <summary>
        /// Compiles the module.
        /// </summary>
        /// <param name="codeTexts">The code texts.</param>
        /// <param name="module">The module.</param>
        /// <param name="references">The references.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <param name="pluginSystemVersion">The plugin system version.</param>
        /// <returns></returns>
        static public PluginCommon CompileModule(string[] codeTexts, string module, 
            string[] references, object objParams, string pluginSystemVersion)
        {
            string compiledName = AssemblyUtils.CompiledPluginName(module, pluginSystemVersion, 
                 string.Concat("-", AssemblyUtils.TimeStampForFile(DateTime.Now), ".dll"));
            //string compiledName = CompiledPluginName(module, pluginSystemVersion, 
            //        string.Concat("-", Path.GetRandomFileName().Substring(0, 8), ".dll"));
            CompilerParameters cp = 
                new CompilerParameters( new[] { "System.Windows.Forms.dll", "System.dll", 
                    "System.Data.dll", "System.Drawing.dll", "System.Xml.dll" },
                     chachePath + compiledName, 
#if DEBUG
                true);
#else
                false);
#endif
            cp.TempFiles = new TempFileCollection(chachePath, false);  //set directory
            cp.ReferencedAssemblies.Add(System.Windows.Forms.Application.ExecutablePath);
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.CompilerOptions = "/optimize";
            cp.WarningLevel = 4;    //to do not consider C00618 warning (obsolete PluginBaseV0_0 class)
            cp.MainClass = "Gear.PluginSupport." + module;
            //traverse list adding not null nor empty texts
            foreach (string s in references)    
                if (!string.IsNullOrEmpty(s))
                    cp.ReferencedAssemblies.Add(s);
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            try
            {
                if (!Directory.Exists(chachePath))
                    Directory.CreateDirectory(chachePath);
                else
                {
                    try
                    {
                        Directory.Delete(chachePath, true);
                        Directory.CreateDirectory(chachePath);
                    }
                    catch(Exception e) 
                    {
                        CompilerError c = 
                            new CompilerError(string.Empty, 0, 0, "Pre-build", e.Message);
                        m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                        return null;
                    }
                }
                //write the codes to compile later
                string[] sourceFiles = new string[codeTexts.Length];
                for (int i = 0; i < codeTexts.Length; i++)
                {
                    sourceFiles[i] = string.Format(chachePath + "{0}-{1}.cs", 
                        AssemblyUtils.CompiledPluginName(module, pluginSystemVersion, 
                        string.Empty), i);
                    File.WriteAllText(sourceFiles[i], codeTexts[i]);
                }
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = System.Environment.CurrentDirectory;
                adSetup.ApplicationName = "Plugin space App";
                adSetup.ShadowCopyFiles = "true";
                adSetup.CachePath = chachePath;
                AppDomain pluginDomain = AppDomain.CreateDomain("pluginDomain", null, adSetup);

                //compile the assembly
                CompilerResults results = provider.CompileAssemblyFromFile(cp, sourceFiles);
                if (results.Errors.HasErrors | results.Errors.HasWarnings)
                {
                    m_Errors = results.Errors;
                    return null;
                }
                string compiledAssembly = results.CompiledAssembly.FullName;
                //PrintAssembliesLoaded(AppDomain.CurrentDomain, results.CompiledAssembly, true);

                //byte[] rawAssembly = loadFile(@".\cache\" + compiledName);
                //Assembly assemblyPlugin = pluginDomain.Load(rawAssembly, null);
                //PrintAssembliesLoaded(pluginDomain, assemblyPlugin, true);

                // TODO ASB: see links to help about running in a different AppDomain
                //http://stackoverflow.com/questions/5380246/loading-services-from-other-dll-and-run-them-isolated/5380317#5380317
                //http://stackoverflow.com/questions/599731/use-the-serializable-attribute-or-subclassing-from-marshalbyrefobject
                //explore how to use MarshalByRefObject: http://www.softwareinteractions.com/blog/2010/2/7/loading-and-unloading-net-assemblies.html
                //http://stackoverflow.com/questions/1687245/use-appdomain-to-load-unload-external-assemblies
                object target = pluginDomain.CreateInstanceAndUnwrap(
                    compiledAssembly,                               //string assemblyName
                    module,                                         //string typeName
                    false,                                          //bool ignoreCase
                    BindingFlags.Public | BindingFlags.Instance,    //BindingFlags bindingAttr
                    null,                                           //Binder binder
                    (objParams != null) ?                           //object[] args
                        new object[] { objParams } :
                        null,
                    null,                                           //CultureInfo culture
                    null);                                          //object[] activationAttributes
                PrintAssembliesLoaded(pluginDomain, results.CompiledAssembly, false);
                if (target == null)
                {
                    CompilerError c = new CompilerError(string.Empty, 0, 0, "CS0103",
                        "The name '" + module + "' does not exist in the current context." +
                        " Does the class name is the same that is declared in c# code?");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }
                else if (!PluginSystem.InstanceOneOfValidClasses(target))
                {
                    CompilerError c = new CompilerError(string.Empty, 0, 0, "CS0029",
                        "Cannot implicitly convert type '" + target.GetType().FullName +
                        "' to '" + PluginSystem.GetPluginBaseType(pluginSystemVersion).FullName + "'");
                    m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                    return null;
                }

                m_Errors = null;
                return (PluginCommon)target;
            }
            catch (Exception e)
            {
                CompilerError c = new CompilerError(string.Empty, 0, 0, "Runtime",
                    string.Format("Plugin '{0}' - {1}", module, e.Message));
                m_Errors = new CompilerErrorCollection(new CompilerError[] { c });
                return null;
            }
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        static private byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }

        // TODO ASB: delete me when debug of plugin compiled is ended.
        static public void PrintAssembliesLoaded(AppDomain ap, Assembly Assem, bool overwrite)
        {
            //temporally debug code to find assemblies loaded in the current domain
            Assembly[] all = ap.GetAssemblies();
            StreamWriter f = (overwrite == true) ?
                File.CreateText(".\\temp.txt") :
                File.AppendText(".\\temp.txt");
            all = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in all)
            {

                f.WriteLine(a.FullName + " : \"" + RetrieveLinkerTimestamp(a.Location) + "\""); 
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
