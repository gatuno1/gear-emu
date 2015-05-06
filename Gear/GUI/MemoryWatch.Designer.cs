namespace Gear.GUI
{
    partial class MemoryWatch
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.listWatches = new System.Windows.Forms.ListView();
            this.colHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderSymbol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderAddr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderVisualizer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listWatches
            // 
            this.listWatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHeaderType,
            this.colHeaderSymbol,
            this.colHeaderAddr,
            this.colHeaderValue,
            this.colHeaderVisualizer,
            this.colHeaderAction});
            this.listWatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listWatches.GridLines = true;
            this.listWatches.Location = new System.Drawing.Point(0, 0);
            this.listWatches.MultiSelect = false;
            this.listWatches.Name = "listWatches";
            this.listWatches.ShowGroups = false;
            this.listWatches.ShowItemToolTips = true;
            this.listWatches.Size = new System.Drawing.Size(150, 150);
            this.listWatches.TabIndex = 0;
            this.listWatches.UseCompatibleStateImageBehavior = false;
            this.listWatches.View = System.Windows.Forms.View.Details;
            // 
            // colHeaderType
            // 
            this.colHeaderType.Text = "Type";
            // 
            // colHeaderSymbol
            // 
            this.colHeaderSymbol.Text = "Symbol";
            // 
            // colHeaderAddr
            // 
            this.colHeaderAddr.Text = "Address";
            // 
            // colHeaderValue
            // 
            this.colHeaderValue.Text = "Value";
            // 
            // colHeaderVisualizer
            // 
            this.colHeaderVisualizer.Text = "Vis";
            // 
            // colHeaderAction
            // 
            this.colHeaderAction.Text = "";
            // 
            // MemoryWatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listWatches);
            this.Name = "MemoryWatch";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listWatches;
        private System.Windows.Forms.ColumnHeader colHeaderType;
        private System.Windows.Forms.ColumnHeader colHeaderSymbol;
        private System.Windows.Forms.ColumnHeader colHeaderAddr;
        private System.Windows.Forms.ColumnHeader colHeaderValue;
        private System.Windows.Forms.ColumnHeader colHeaderVisualizer;
        private System.Windows.Forms.ColumnHeader colHeaderAction;
    }
}
