using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

using Gear.EmulationCore;

namespace Gear.PluginSupport
{
    /// @brief Common ancestor for every plugin.
    /// @since v15.03.26 - Added.
    public abstract class PluginCommon : System.Windows.Forms.UserControl
    {
        /// @brief Title of the tab window.
        public abstract string Title { get; }

        /// @brief Attribute to allow key press detecting on the plugin. 
        /// @note Mirror's: allows hot keys to be disabled for a plugin.
        /// @note Source: <a href="http://forums.parallax.com/showthread.php/100380-More-GEAR-Improved-Emulation-of-the-Propeller">
        /// Mirror Post for Version V08_10_16 in propeller forums</a>
        public virtual bool AllowHotKeys { get { return true; } }

        /// @brief Attribute to allow the window to be closed (default) or not (like cog windows).
        /// @remarks Not to be used in Plugin Editor by user plugins.
        public virtual bool IsClosable { get { return true; } }

        /// @brief Identify a plugin as user (=true) or system (=false).
        /// @remarks Not to be used in Plugin Editor by user plugins.
        /// @since V15.03.26 - Added.
        public virtual bool IsUserPlugin { get { return true; } }

        /// @brief Attribute to allow a single instance (=true) or multiple (=false).
        /// @since V15.03.26 - Added.
        public abstract bool SingleInstanceAllowed { get; }

        /// @brief Gets the full name of the assembly where the plugin is instantiated.
        /// @since V15.03.26 - Added.
        public string assemblyFullName { get; private set; }

        /// @brief Default constructor.
        /// @since V15.03.26 - Added.
        public PluginCommon()
        {
            assemblyFullName = typeof(PluginCommon).AssemblyQualifiedName;
        }

        /// @brief Event when the chip is reset.
        /// Handy to reset plugin's components or data, to their initial states.
        public virtual void OnReset() { }
        
        /// @brief Event when some pin changed and is informed to the plugin.
        /// @note Asterisk's: occurs every time a pin has changed states. PinState tells you if 
        /// either the propeller or another component has set the pin Hi or Lo, or if the pin is 
        /// floating.
        /// @note Source: <a href="http://forums.parallax.com/showthread.php/91084-GEAR-Propeller-Debugging-Environment?p=625629&viewfull=1#post625629">
        /// API GEAR described on GEAR original Post</a>
        /// @param[in] time Time in seconds.
        /// @param[in] pins Array of pins with the current state.
        public virtual void OnPinChange(double time, PinState[] pins) { }

        /// @brief Event to repaint the plugin screen (if used).
        /// @note Asterisk's: occurs when the GUI has finished executing a emulation 'frame' 
        /// (variable number of clocks). Force is always true (this means that the call wants to 
        /// 'force' an update, this is provided so you can pass a false for non-forced repaints).
        /// @note Source: <a href="http://forums.parallax.com/showthread.php/91084-GEAR-Propeller-Debugging-Environment?p=625629&viewfull=1#post625629">
        /// API GEAR described on GEAR original Post</a>
        /// @param[in] force Flag to indicate the intention to force the repaint.
        public virtual void Repaint(bool force) { }

        //public abstract override bool Equals(object other);

        //public abstract bool Equals(PluginCommon other);

    }

    /// @brief Provides information about the plugin system.
    /// @since v15.03.26 - Added.
    public static class PluginSystem
    {
        /// @brief Determine Type of plugin base class corresponding to the given plugin system version.
        /// @param pluginSystemVersion Version of the plugin system to use.
        /// @returns Type of the plugin base class.
        /// @since v15.03.26 - Added.
        public static Type GetPluginBaseType(string pluginSystemVersion)
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

        /// @brief Determine if the object passed as parameter is one of the allowed Plugin 
        /// base classes.
        /// @param obj Instance of the class to examine its validity.
        /// @returns If a valid class (=true), or invalid (=false).
        /// @since v15.03.26 - Added.
        public static bool InstanceOneOfValidClasses(object obj)
        {
            if (obj == null)
                return false;
            else
#pragma warning disable 618
                return ( (obj is PluginBase) || (obj is PluginBaseV0_0));
#pragma warning restore 618
                
        }

        /// @brief Replace base class for proper inheritance, for to use with v0.0 plugin 
        /// system plugins.
        /// @param codeText Original text to insert into.
        /// @returns Modified text code.
        /// @since v15.03.26 - Added.
        public static string ReplaceBaseClassV0_0(string codeText)
        {
            // Search Plugin class declaration for V0.0 plugin system compatibility.
            Regex ClassNameToChange = new Regex(
                @"(\bclass\s+)(?<Class>[@]?[_]*[A-Z|a-z|0-9]+[A-Z|a-z|0-9|_]*)(\s*\:\s*)" +
                @"(?<BaseClass>PluginBase\b)",
                RegexOptions.Compiled);
            // Replace "PluginBase" with "PluginBaseV0_0" to compile with correct class.
            return ClassNameToChange.Replace(codeText, "$1${Class}$2PluginBaseV0_0", 1);
        }

        /// @brief Complete the declaration of PropellerCPU class, to enable the compilation
        /// of v0.0 plugin system plugins.
        /// @details In a V0.0 plugin, the class PropellerCPU was named only 'Propeller'. 
        /// To successfully compile the plugin, is necessary to complete the proper name.
        /// @dontinclude PluginV1.0-ReplacePropellerClassV0_0.cs
        /// As example, a plugin v0.0 like:
        /// @skip ...
        /// @until }
        /// After the replacement it will be as:
        /// @skip ...
        /// @until }
        /// @param codeText Original text to insert into.
        /// @returns Modified text code.
        /// @since v15.03.26 - Added.
        public static string ReplacePropellerClassV0_0(string codeText)
        {
            // Search Plugin class declaration for V0.0 plugin system compatibility.
            Regex ClassNameToChange = new Regex(@"(\bPropeller\b)", RegexOptions.Compiled);
            // Replace "Propeller" with "PropellerCPU" as needed in V1.0 host.
            return ClassNameToChange.Replace(codeText,"$1CPU");
        }

        /// @brief Generate the code to add to include version information to generate an 
        /// Assembly for the plugin.
        /// @details Transforms the plugin code, adding attributes to be inserted into the 
        /// compiled assembly.
        /// @dontinclude PluginV1.0-InsertAssemblyDetails.cs
        /// As example, a plugin v0.0 like:
        /// @skip using
        /// @until }
        /// After the replacement it will be as:
        /// @skip using
        /// @until }
        /// @param codeText Original text to insert into.
        /// @param buildDate Date Time to be used like timestamp into the compiled plugin.
        /// @param module Name of the plugin module.
        /// @param description Description of the plugin.
        /// @param version Version of plugin, to insert into the assembly.
        /// @returns Modified text code.
        /// @note Adds information into the assembly for attributes: 
        /// @li `AssemblyCompany` @li `AssemblyVersion` @li `AssemblyProduct` @li `AssemblyTitle`
        /// @li `AssemblyDescription`
        /// @since v15.03.26 - Added.
        public static string InsertAssemblyDetails(string codeText, DateTime buildDate,
            string module, string description, string version)
        {
            string aux = codeText;
            //
            Regex Includes = new Regex(@"(\busing\b\s+\w+[.|\w+]*;)(\n)",
                RegexOptions.Compiled);
            if (Includes.IsMatch(aux))
            {
                bool srFound = false;
                MatchCollection coll = Includes.Matches(codeText);
                //traverse all the includes to check if Reflection is included already
                for (int i = 0; i < coll.Count; i++)
                {
                    if (coll[i].Value.Contains("System.Reflection;"))
                        srFound = true;
                }
                //text of attributes to add information into Assembly
                //names from link: "Setting Assembly Attributes"
                //https://msdn.microsoft.com/en-us/library/4w8c1y2s%28v=vs.110%29.aspx
                string infoAttributes = string.Concat(
                    //AssemblyCompany
                    string.Concat("[assembly:AssemblyCompany(\"",
                        AssemblyUtils.GetGEARAssemblyAttribute(typeof(AssemblyCompanyAttribute))[0],
                        "\")] "),
                    //AssemblyVersion
                    string.Concat("[assembly:AssemblyVersion(\"",
                        AssemblyUtils.CompleteVersion(version, 2), ".",
                        AssemblyUtils.TimeStampDotnetEpoch(buildDate), "\")] "),
                    //AssemblyProduct
                    string.Concat("[assembly:AssemblyProduct(\"",
                        AssemblyUtils.GetGEARAssemblyAttribute(typeof(AssemblyProductAttribute))[0],
                        "\")] "),
                    //AssemblyTitle
                    string.Concat("[assembly:AssemblyTitle(\"",
                        ((module.Contains("plugin")) ? module : module + " plugin"), "\")] "),
                    //AssemblyDescription
                    string.Concat("[assembly:AssemblyDescription(\"", description, "\")]"));
                
                //insert the using statement (if not already exist), and the attributes
                aux = Includes.Replace(
                    codeText,                       //original text
                    string.Concat("${1} ",          //replace pattern
                        ((!srFound) ? "using System.Reflection; " : string.Empty),
                        infoAttributes, "${2}"),
                    1,                              //only the last match
                    coll[coll.Count - 1].Index);    //start of the last match
            }
            return aux;
        }



    }
}
