namespace Asteroids
{
    partial class GameMenu
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
            this.label1 = new System.Windows.Forms.Label();
            this._BtnStart = new System.Windows.Forms.Button();
            this.LV_HighScores = new System.Windows.Forms.ListView();
            this._HighScore = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to Asteroid Left and Right control rotation Space to Fire your gun";
            // 
            // _BtnStart
            // 
            this._BtnStart.Location = new System.Drawing.Point(13, 211);
            this._BtnStart.Name = "_BtnStart";
            this._BtnStart.Size = new System.Drawing.Size(214, 23);
            this._BtnStart.TabIndex = 3;
            this._BtnStart.Text = "Start";
            this._BtnStart.UseVisualStyleBackColor = true;
            // 
            // LV_HighScores
            // 
            this.LV_HighScores.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._HighScore,
            this._Name});
            this.LV_HighScores.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_HighScores.HideSelection = false;
            this.LV_HighScores.Location = new System.Drawing.Point(0, 44);
            this.LV_HighScores.Name = "LV_HighScores";
            this.LV_HighScores.Size = new System.Drawing.Size(238, 161);
            this.LV_HighScores.TabIndex = 4;
            this.LV_HighScores.UseCompatibleStateImageBehavior = false;
            this.LV_HighScores.View = System.Windows.Forms.View.Details;
            // 
            // _HighScore
            // 
            this._HighScore.Text = "High Score";
            this._HighScore.Width = 115;
            // 
            // _Name
            // 
            this._Name.Text = "Player Name";
            this._Name.Width = 91;
            // 
            // GameMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 239);
            this.Controls.Add(this.LV_HighScores);
            this.Controls.Add(this._BtnStart);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GameMenu";
            this.Text = "Asteroids";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _BtnStart;
        private System.Windows.Forms.ListView LV_HighScores;
        private System.Windows.Forms.ColumnHeader _Name;
        private System.Windows.Forms.ColumnHeader _HighScore;
    }
}