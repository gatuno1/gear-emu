using System;
using System.ComponentModel;
using System.Configuration;
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

        private void OKButton_Click(object sender, EventArgs e)
        {
            //save to disk
            Properties.Settings.Default.Save();
            foreach(Form f in ParentForm.OwnedForms)
            {

            }
            this.Close();
        }

        /// @brief Reset the property to its default value.
        /// @param sender
        /// @param e 
        private void ResetButton_Click(object sender, EventArgs e)
        {
            PropertyDescriptor prop;    //to get the underlying property
            //check if a property is selected and if it is writeable
            if (GearPropertyGrid.SelectedGridItem.GridItemType == GridItemType.Property && 
                !(prop = GearPropertyGrid.SelectedGridItem.PropertyDescriptor).IsReadOnly)
            {
                //try to get the default value of the property
                DefaultSettingValueAttribute attr =
                    prop.Attributes[typeof(DefaultSettingValueAttribute)] 
                    as DefaultSettingValueAttribute;
                if (attr != null)  //if exist
                {
                    if (prop.CanResetValue(Settings.Default[prop.Name]))
                        prop.ResetValue(Settings.Default[prop.Name]);
                    else
                        prop.SetValue(
                            Settings.Default, 
                            Convert.ChangeType(attr.Value, prop.PropertyType) );
                }
                GearPropertyGrid.Refresh();
            }
        }
    }
}
