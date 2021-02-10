namespace Asteroids
{
    partial class NameDialog
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
            this._tbxName = new System.Windows.Forms.TextBox();
            this._btnNameOK = new System.Windows.Forms.Button();
            this._lblPlayerName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _tbxName
            // 
            this._tbxName.Location = new System.Drawing.Point(85, 13);
            this._tbxName.Name = "_tbxName";
            this._tbxName.Size = new System.Drawing.Size(182, 20);
            this._tbxName.TabIndex = 0;
            // 
            // _btnNameOK
            // 
            this._btnNameOK.Location = new System.Drawing.Point(102, 40);
            this._btnNameOK.Name = "_btnNameOK";
            this._btnNameOK.Size = new System.Drawing.Size(75, 23);
            this._btnNameOK.TabIndex = 1;
            this._btnNameOK.Text = "Ok";
            this._btnNameOK.UseVisualStyleBackColor = true;
            // 
            // _lblPlayerName
            // 
            this._lblPlayerName.AutoSize = true;
            this._lblPlayerName.Location = new System.Drawing.Point(12, 16);
            this._lblPlayerName.Name = "_lblPlayerName";
            this._lblPlayerName.Size = new System.Drawing.Size(67, 13);
            this._lblPlayerName.TabIndex = 2;
            this._lblPlayerName.Text = "Player Name";
            // 
            // NameDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 69);
            this.Controls.Add(this._lblPlayerName);
            this.Controls.Add(this._btnNameOK);
            this.Controls.Add(this._tbxName);
            this.Name = "NameDialog";
            this.Text = "Enter Your Name!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _tbxName;
        private System.Windows.Forms.Button _btnNameOK;
        private System.Windows.Forms.Label _lblPlayerName;
    }
}