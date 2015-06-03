using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public virtual Boolean AllowHotKeys { get { return true; } }

        /// @brief Attribute to allow the window to be closed (default) or not (like cog windows).
        /// @remarks Not to be used in Plugin Editor by user plugins.
        public virtual Boolean IsClosable { get { return true; } }

        /// @brief Identify a plugin as user (=true) or system (=false).
        /// @remarks Not to be used in Plugin Editor by user plugins.
        /// @since V15.03.26 - Added.
        public virtual Boolean IsUserPlugin { get { return true; } }

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
