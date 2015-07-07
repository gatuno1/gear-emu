using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gear.Properties;

namespace Gear.GUI
{
    /// @brief Form to edit program properties
    /// @since v15.03.26 - Added. 
    public partial class AppPropertiesEditor : Form
    {
        // TODO [ASB] : see PropertyGrid Control tutorial - https://msdn.microsoft.com/en-us/library/aa302326.aspx
        // also in https://msdn.microsoft.com/en-us/library/aa302334.aspx
        // and http://www.codeproject.com/Articles/22717/Using-PropertyGrid
        public AppPropertiesEditor()
        {
            InitializeComponent();
            GearPropertyGrid.SelectedObject = Settings.Default;
        }
    }
}
