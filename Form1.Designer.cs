namespace ImageTransmitter
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.inputButton = new System.Windows.Forms.Button();
            this.inputDialog = new System.Windows.Forms.OpenFileDialog();
            this.sendButton = new System.Windows.Forms.Button();
            this.cutButton = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.portsBox = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new ImageTransmitter.MyPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // inputButton
            // 
            this.inputButton.Location = new System.Drawing.Point(12, 12);
            this.inputButton.Name = "inputButton";
            this.inputButton.Size = new System.Drawing.Size(123, 25);
            this.inputButton.TabIndex = 0;
            this.inputButton.Text = "Load Image";
            this.inputButton.UseVisualStyleBackColor = true;
            this.inputButton.Click += new System.EventHandler(this.InputButtonClick);
            // 
            // inputDialog
            // 
            this.inputDialog.FileName = "inputDialog";
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(270, 12);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(123, 25);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButtonClick);
            // 
            // cutButton
            // 
            this.cutButton.Location = new System.Drawing.Point(141, 12);
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(123, 25);
            this.cutButton.TabIndex = 2;
            this.cutButton.Text = "Cut";
            this.cutButton.UseVisualStyleBackColor = true;
            this.cutButton.Click += new System.EventHandler(this.CutButtonClick);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.Parity = System.IO.Ports.Parity.Odd;
            // 
            // portsBox
            // 
            this.portsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portsBox.FormattingEnabled = true;
            this.portsBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            this.portsBox.Location = new System.Drawing.Point(421, 14);
            this.portsBox.Name = "portsBox";
            this.portsBox.Size = new System.Drawing.Size(105, 21);
            this.portsBox.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(0, 0);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageMouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageMouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageMouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.portsBox);
            this.Controls.Add(this.cutButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.inputButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button inputButton;
        private System.Windows.Forms.OpenFileDialog inputDialog;
        private MyPictureBox pictureBox1;
        private System.Drawing.Bitmap image;

        private bool mouseIsDown = false;
        private System.Drawing.Point clickPoint;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button cutButton;
        private System.IO.Ports.SerialPort serialPort1;

        private const int imageSizeLimit = 100;
        private System.Windows.Forms.ComboBox portsBox;
    }
}

