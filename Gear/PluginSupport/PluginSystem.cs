using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gear.EmulationCore;

namespace Gear.PluginSupport
{
    /// @brief Common ancestor for every plugin.
    /// @since v15.03.26 - Added.
    public class PluginCommon : System.Windows.Forms.UserControl
    {
        public virtual void PresentChip() { }

#pragma warning disable 618
        public virtual void PresentChip(Propeller host) { }
#pragma warning restore 618

        public virtual string Title { get { return "Plugin Base"; } }

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
#pragma warning disable 618
                return ( (obj is PluginBase) || (obj is PluginBaseV0_0));
#pragma warning restore 618
                
        }

    }
}
