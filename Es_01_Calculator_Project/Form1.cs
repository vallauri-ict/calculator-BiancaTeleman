using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Es_01_Calculator_Project
{
    public partial class FormMain : Form
    {
        //private char[,] bottoni = new char[6,4];
        public struct strutturabottoni
        {
            public char Content;
            public bool isBold;
            public bool isNumber;
            public bool isDecimalSeparator;
            public bool isPlusMinusSign;

            public strutturabottoni(char Content, bool isBold, bool isNumber=true,bool isDecimalSeparator=true,bool isPlusMinusSign=false)
            {
                this.Content = Content;
                this.isBold = isBold;
                this.isNumber = isNumber;
                this.isDecimalSeparator = isDecimalSeparator;
                this.isPlusMinusSign = isPlusMinusSign;
            }
            public override string ToString()
            {
                return Content.ToString();
            }

        };
        private strutturabottoni[,] button =
        {
            {new strutturabottoni('%',false),new strutturabottoni('ɶ',false),new strutturabottoni('c',false),new strutturabottoni('C',false), },
            {new strutturabottoni(' ',false),new strutturabottoni(' ',false),new strutturabottoni(' ',false),new strutturabottoni('÷',false) },
            {new strutturabottoni('7',false,false),new strutturabottoni('8',false,false),new strutturabottoni('9',false,false),new strutturabottoni('x',false) },
            {new strutturabottoni('4',false,false),new strutturabottoni('5',false,false),new strutturabottoni('6',false,false),new strutturabottoni('-',false) },
            {new strutturabottoni('1',false,false),new strutturabottoni('2',false,false),new strutturabottoni('3',false,false),new strutturabottoni('+',false) },
            {new strutturabottoni('±',false,false,false,true),new strutturabottoni('0',false,false),new strutturabottoni(',',false,true,true),new strutturabottoni('=',false) },

        };
        public FormMain()
        {
            InitializeComponent();
        }
        private RichTextBox txt;
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            Makebuttons(button);
            MakeresultsBox();
        }
        private void MakeresultsBox()
        {
            txt = new RichTextBox();
            txt.ReadOnly = true;
            txt.SelectionAlignment = HorizontalAlignment.Right;
            txt.Font = new Font("Segoe UI", 22);
            txt.Width = this.Width - 16;
            txt.Height = 50;
            txt.Top = 20;
            txt.Text = "0";
            this.Controls.Add(txt);
        }
        private void Makebuttons(strutturabottoni[,] bottoni)
        {
            int buttonWidth = 78;
            int buttonHeight = 50;
            int posx = 0;
            int posy = 131;

            for (int i = 0; i < bottoni.GetLength(0); i++)
            {
                for (int j = 0; j < bottoni.GetLength(1); j++)
                {
                    strutturabottoni bs = bottoni[i, j];
                    Button newbutton = new Button();
                    newbutton.Text = bottoni[i, j].Content.ToString();
                    newbutton.Width = buttonWidth;
                    newbutton.Height = buttonHeight;
                    newbutton.Left = posx;
                    newbutton.Top = posy;
                    newbutton.Tag = bs;
                    newbutton.Click += Button_Click;
                    this.Controls.Add(newbutton);
                    posx += buttonWidth;
                }
                posx = 0;
                posy += buttonHeight;

            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            
            Button clickedButton = (Button)sender;
            strutturabottoni bs = (strutturabottoni)clickedButton.Tag;
            //MessageBox.Show("Button: "+clickedButton.Text);
        
            if(!bs.isNumber)
            {
                if(txt.Text=="0")
                {
                    txt.Text = "";
                }
                txt.Text += clickedButton.Text;
            }
            else
            {
                if(bs.isDecimalSeparator)
                {
                    if(!txt.Text.Contains(bs.Content))
                    {
                        txt.Text += clickedButton.Text;
                    }
                }
                if(bs.isPlusMinusSign)
                {
                    if (!txt.Text.Contains("-"))
                    {
                        txt.Text = "-"+txt.Text;
                    }
                    else
                    {
                        txt.Text = txt.Text.Replace("-", "");
                    }
                }
            }
        }

    }
}
