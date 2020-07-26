namespace JFrameworkInstaller
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Info = new System.Windows.Forms.Label();
            this.Check = new System.Windows.Forms.Button();
            this.Uninstall = new System.Windows.Forms.Button();
            this.Install = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Info
            // 
            this.Info.BackColor = System.Drawing.Color.Silver;
            this.Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Info.Location = new System.Drawing.Point(9, 154);
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(653, 18);
            this.Info.TabIndex = 0;
            this.Info.Text = "Installation Unknown";
            this.Info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Info.Click += new System.EventHandler(this.Info_Click);
            // 
            // Check
            // 
            this.Check.BackColor = System.Drawing.Color.Aqua;
            this.Check.Location = new System.Drawing.Point(12, 70);
            this.Check.Name = "Check";
            this.Check.Size = new System.Drawing.Size(650, 23);
            this.Check.TabIndex = 1;
            this.Check.Text = "Check Install";
            this.Check.UseVisualStyleBackColor = false;
            this.Check.Click += new System.EventHandler(this.Check_Click);
            // 
            // Uninstall
            // 
            this.Uninstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Uninstall.Location = new System.Drawing.Point(12, 41);
            this.Uninstall.Name = "Uninstall";
            this.Uninstall.Size = new System.Drawing.Size(650, 23);
            this.Uninstall.TabIndex = 2;
            this.Uninstall.Text = "Uninstall Mod";
            this.Uninstall.UseVisualStyleBackColor = false;
            // 
            // Install
            // 
            this.Install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Install.Location = new System.Drawing.Point(12, 12);
            this.Install.Name = "Install";
            this.Install.Size = new System.Drawing.Size(650, 23);
            this.Install.TabIndex = 3;
            this.Install.Text = "Install Mod";
            this.Install.UseVisualStyleBackColor = false;
            this.Install.Click += new System.EventHandler(this.Install_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 181);
            this.Controls.Add(this.Install);
            this.Controls.Add(this.Uninstall);
            this.Controls.Add(this.Check);
            this.Controls.Add(this.Info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "JFramework Installer";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Info;
        private System.Windows.Forms.Button Check;
        private System.Windows.Forms.Button Uninstall;
        private System.Windows.Forms.Button Install;
    }
}

