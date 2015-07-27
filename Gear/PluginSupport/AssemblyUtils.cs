using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gear.PluginSupport
{
    /// @todo document class Gear.PluginSupport.AssemblyUtils
    public abstract class AssemblyUtils
    {
        public static DateTime GetFileDateTime(string fileName)
        {
            return File.GetLastWriteTime(fileName);
        }

        public static string TimeStampForFile(DateTime dat)
        {
            return dat.ToUniversalTime().ToString("yyyyMMddTHHmmssK");
        }

        public static string TimeStampUniversalFormat(DateTime dat)
        {
            return dat.ToUniversalTime().ToString("u");
        }

        /// @brief Use the given DateTime to generate build and revision for building an 
        /// executable or assembly.
        /// @details Calculate the difference from .NET epoch (Jan 1 2000 00:00:00), in days 
        /// between the given parameter date and seconds from last midnight. The build number 
        /// is equal to the number of days since January 1, 2000 local time, and revision is 
        /// equal to the number of seconds since midnight local time, divided by 2.
        /// @note Source: <a href="http://blog.codinghorror.com/determining-build-date-the-hard-way/">
        /// Determining Build Date the hard way</a>
        /// @param dat Date to calculate. It should be in Local Time.
        /// @returns A string for build and revision in format "<build>.<revision>".
        /// @throws Exception Generic exception in the case the parameter have unspecified 
        /// Kind (nor Local nor Utc).
        //public static string TimeStampDotnetEpoch(DateTime dat)
        //{
        //    //convert the datetime to local time (as specified for .NET)
        //    switch (dat.Kind)
        //    {
        //        case DateTimeKind.Local:
        //            break;
        //        default:
        //        case DateTimeKind.Utc:
        //            dat = dat.ToLocalTime();
        //            break;
        //        case DateTimeKind.Unspecified:
        //            throw new Exception(
        //                string.Format("Time '{0}' don't have DateTimeKind specified. " +
        //                    "It is unsafe to assume local.",
        //                dat.ToString("u")));
        //    }
        //    //create the epoch
        //    DateTime dotnetEpoch = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
        //    TimeSpan diff = dat.Subtract(dotnetEpoch);
        //    int daysSince2000 = diff.Days;
        //    int secondsSinceLastMidnight =
        //        (diff.Subtract(new TimeSpan(daysSince2000, 0, 0, 0)).Seconds / (int)2);
        //    return string.Format("{0}.{1}", daysSince2000, secondsSinceLastMidnight);
        //}

        /// @brief Retrieve an assembly attribute from GEAR executable.
        /// @param clAttr Type of the class attribute to retrieve , to obtain its information from 
        /// the current assembly.
        /// @returns If successful, returns a list of strings with their value, if fail, Null. 
        /// @since v15.03.26 - Added.
        public static string[] GetGEARAssemblyAttribute(Type clAttr)
        {
            Assembly assemb = Assembly.GetExecutingAssembly();
            object[] attribs = assemb.GetCustomAttributes(clAttr, false);

            if (attribs.Length == 0)
                return null;
            else
            {
                List<string> strList = new List<string>();
                foreach (object o in attribs)
                {
                    switch (clAttr.Name)
                    {
                        case "AssemblyCompanyAttribute":
                            var a = o as AssemblyCompanyAttribute;
                            if (a != null)
                                strList.Add(a.Company);
                            break;
                        case "AssemblyProductAttribute":
                            var b = o as AssemblyProductAttribute;
                            if (b != null)
                                strList.Add(b.Product);
                            break;
                        default:
                            strList.Add(o.ToString());
                            break;
                    }
                }
                return strList.ToArray();
            }
        }

        /// @brief Completes the version numbers, stripping to the digits given as parameter, 
        /// or completing with zeros if version is smaller than it.
        /// @param version Version string to complete as Assembly standards.
        /// @param digits How many digits will use.
        /// @returns A complete version string.
        //public static string CompleteVersion(string version, uint digits)
        //{
        //    string[] parts = version.Split('.');
        //    string completed = System.String.Empty;
        //    for (int i = 0; i < digits; i++)
        //    {
        //        if ((parts.Length <= i) || (string.IsNullOrEmpty(parts[i])))
        //            completed += "0";
        //        else
        //            completed += parts[i];
        //        completed += ((i < (digits - 1)) ? "." : String.Empty);
        //    }
        //    return completed;
        //}

        /// @brief Generate the name of the compiled plugin.
        /// @param module Name of the plugin module.
        /// @param pluginSystemVersion Version of the plugin system to add to the plugin.
        /// @param extension Extension to add to the name. If it is null, none will be added.
        /// @returns Name of the compiled plugin.
        /// @since v15.03.26 - Added.
        //public static string CompiledPluginName(string module, string pluginSystemVersion,
        //    string extension)
        //{
        //    return string.Format("{0}-PlgnSysV{1}{2}",
        //        module,
        //        pluginSystemVersion.Replace(".", "_"),
        //        ((string.IsNullOrEmpty(extension)) ? string.Empty :
        //            (!extension.Contains(".")) ? ("." + extension) : extension));
        //}

        /// @brief Generate the full name for a compiled plugin.
        /// @param timeOfBuild The time of build.
        /// @param module Name of the plugin module.
        /// @param pluginVersion The plugin version.
        /// @param pluginSystemVersion Version of the plugin system.
        /// @returns Full name of a compiled plugin as it should be retrieved from an assembly.
        //public static string CompiledPluginFullName(DateTime timeOfBuild, string module, 
        //    string pluginVersion, string pluginSystemVersion)
        //{
        //    string fullName = string.Concat(
        //        //name of compiled module
        //        CompiledPluginName(module, pluginSystemVersion, ".dll"), ", ",
        //        //version
        //        "Version=", CompleteVersion(pluginVersion, 2), ".", 
        //            TimeStampDotnetEpoch(timeOfBuild), ", ",
        //        //culture
        //        "Culture=neutral, ",
        //        //key
        //        "PublicKeyToken=null" );
        //    return fullName;
        //}

    }
}
