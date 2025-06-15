namespace Giles_Chen_test_1
{
    partial class OrderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridViewMenu = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridViewMenu).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewMenu
            // 
            dataGridViewMenu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewMenu.Location = new Point(1269, 657);
            dataGridViewMenu.Name = "dataGridViewMenu";
            dataGridViewMenu.Size = new Size(141, 84);
            dataGridViewMenu.TabIndex = 0;
            dataGridViewMenu.CellContentClick += dataGridView1_CellContentClick;
            // 
            // Form6
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1439, 753);
            Controls.Add(dataGridViewMenu);
            Name = "Form6";
            Text = "Form6";
            Load += Form6_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewMenu).EndInit();
            ResumeLayout(false);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 这里可以添加处理点击事件的逻辑
            // 暂时可以保留为空方法，避免错误
        }



        #endregion

        private DataGridView dataGridViewMenu;
    }
}