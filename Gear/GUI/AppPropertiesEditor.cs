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
        /// @param sender Sender object to this event.
        /// @param e Arguments to this event.
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
                    //remember old value
                    object oldValue = prop.GetValue(Settings.Default);
                    //set the new value
                    if (prop.CanResetValue(Settings.Default))
                        prop.ResetValue(Settings.Default);
                    else
                        prop.SetValue(
                            Settings.Default, 
                            Convert.ChangeType(attr.Value, prop.PropertyType) );
                    //call the notification event
                    GearPropertyGrid_PropertyValueChanged(sender, new PropertyValueChangedEventArgs(
                        GearPropertyGrid.SelectedGridItem, oldValue));
                }
                GearPropertyGrid.Refresh();
            }
        }

        /// @brief Event when a property had changed its value, used to update copies of the 
        /// property values used in other forms.
        /// @param s Sender object to this event.
        /// @param e Arguments to this event, including the old value.
        private void GearPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyDescriptor prop;    //to get the underlying property
            if (e.ChangedItem.GridItemType == GridItemType.Property &&
                !(prop = e.ChangedItem.PropertyDescriptor).IsReadOnly)
            {
                switch (prop.Name)
                {
                    case "UpdateEachSteps":
                        if (this.ParentForm.IsMdiContainer & this.ParentForm.HasChildren)
                            foreach(Form f in this.ParentForm.MdiChildren)
                            {
                                var emu = f as Emulator;
                                if (emu != null)
                                {
                                    emu.stepInterval = (uint)prop.GetValue(Settings.Default);
                                }
                            }
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
