/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * PluginBase.cs
 * Abstract superclass for emulator plugins
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
using System.Windows.Forms;

using Gear.EmulationCore;

/// @brief Name space for Plugin support.
/// @details Contains the classes that defines the plugin system: the plugin class  
/// structure itself, the loading of plugins from XML files, the compiling and instantation 
/// of a plugin class. 
namespace Gear.PluginSupport
{
    /// @brief Base class for plugin support in the old 0.0 version.
    /// @details Define basic methods and attributes for plugins in GEAR with the legacy structure.
    /// Only to be used for compatibility with old plugins.
    /// @note See Asterisk's comments:
    /// Source: <a href="http://forums.parallax.com/showthread.php/91084-GEAR-Propeller-Debugging-Environment?p=636953&viewfull=1#post636953">
    /// Original thread on GEAR with explanation of plugin class</a>
    /// @remarks To see examples of how to use it, see the directory 'plugins' included with 
    /// the source code.
    public class PluginBaseV0_0 : UserControl
    {

        /// @brief Title of the tab window.
        public virtual string Title { get { return "Bus Module"; } }

        /// @brief Attribute to allow key press detecting on the plugin. 
        /// @note Mirror's: allows hot keys to be disabled for a plugin.
        /// @note Source: <a href="http://forums.parallax.com/showthread.php/100380-More-GEAR-Improved-Emulation-of-the-Propeller">
        /// Mirror Post for Version V08_10_16 in propeller forums</a>
        public virtual Boolean AllowHotKeys { get { return true; } }

        /// @brief Attribute to allow the window to be closed (default) or not (like cog windows).
        /// @remarks Not to be used in Plugin Editor by user plugins.
        public virtual Boolean IsClosable { get { return true; } }

        /// @brief Points to propeller instance.
        /// @note Asterisk's: Occurs once the plugin is loaded. It gives you a reference to the 
        /// propeller chip (so you can drive the pins). 
        /// @note Source: <a href="http://forums.parallax.com/showthread.php/91084-GEAR-Propeller-Debugging-Environment?p=625629&viewfull=1#post625629">
        /// API GEAR described on GEAR original Post</a>
        public virtual void PresentChip(Propeller host) { }    

        /// @brief Event when the chip is reset.
        /// Handy to reset plugin's components or data, to their initial states.
        public virtual void OnReset() { }

        /// @brief Event when a clock tick is informed to the plugin, in clock units.
        /// @param[in] time Time in seconds of the emulation.
        public virtual void OnClock(double time) { }

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

    /// @brief Synonim for PropellerCPU class, to use it with old plugin system.
    public class Propeller : PropellerCPU 
    {
        Propeller(GUI.Emulator em) : base(em) { }
    }
}

