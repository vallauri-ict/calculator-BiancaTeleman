using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Es_01_Calculator_Project
{
    public partial class FormMain : Form
    {
        //private char[,] bottoni = new char[6,4];
        public struct ButtonStruct
        {
            public char Content;
            public bool IsBold;
            public bool IsNumber;
            public bool IsDecimalSeparator;
            public bool IsPlusMinusSign;
            public bool IsOperator;
            public bool IsEqualSign;
            public bool IsSpecialOperator;
            public ButtonStruct(char content, bool isBold, bool isNumber = false, bool isDecimalSeparator = false, bool isPlusMinusSign = false, bool isOperator = false, bool isEqualSign = false,bool isSpecialOperator = false)
            {
                this.Content = content;
                this.IsBold = isBold;
                this.IsNumber = isNumber;
                this.IsDecimalSeparator = isDecimalSeparator;
                this.IsPlusMinusSign = isPlusMinusSign;
                this.IsOperator = isOperator;
                this.IsEqualSign = isEqualSign;
                this.IsSpecialOperator = isSpecialOperator;
            }
            public override string ToString()
            {
                return Content.ToString();
            }

        };
        private ButtonStruct[,] buttons =
        {
            {new ButtonStruct('%',false),new ButtonStruct('ɶ',false),new ButtonStruct('C',false),new ButtonStruct('⩤',false) },
            {new ButtonStruct('⅟',false,false,false,false,true,false,true),new ButtonStruct('^',false,false,false,false,true,false,true),new ButtonStruct('√',false,false,false,false,true,false,true),new ButtonStruct('÷',false, false, false, false, true) },
            {new ButtonStruct('7',true, true),new ButtonStruct('8',true, true),new ButtonStruct('9',true, true),new ButtonStruct('x',false, false, false, false, true) },
            {new ButtonStruct('4',true, true),new ButtonStruct('5',true, true),new ButtonStruct('6',true, true),new ButtonStruct('-',false, false, false, false, true) },
            {new ButtonStruct('1',true, true),new ButtonStruct('2',true, true),new ButtonStruct('3',true, true),new ButtonStruct('+',false ,false, false, false, true) },
            {new ButtonStruct('±',false, false, false, true),new ButtonStruct('0',true, true),new ButtonStruct(',',false, false, true),new ButtonStruct('=',false ,false, false, false, true, true) },

        };
        private RichTextBox resultBox;

        private const char ASCIIZERO = '\x0000';
        private double operand1, operand2, result;
        private char lastOperator;
        private ButtonStruct lastButtonClicked;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Makebuttons(buttons);
            MakeResultsBox();
        }
        private void MakeResultsBox()
        {
            resultBox = new RichTextBox();
            resultBox.ReadOnly = true;
            resultBox.SelectionAlignment = HorizontalAlignment.Right;
            resultBox.Font = new Font("Segoe UI", 22);
            resultBox.Width = this.Width - 16;
            resultBox.Height = 50;
            resultBox.Top = 20;
            resultBox.Text = "0";
            resultBox.TabStop = false;
            resultBox.TextChanged += ResultBox_TextChanged; // per capire quando devo andare a ridurre il carattere
            this.Controls.Add(resultBox);
        }

        private void ResultBox_TextChanged(object sender, EventArgs e)
        {
            int newSize = 22 + (15 - resultBox.Text.Length);
            if (newSize > 8 && newSize < 23)
            {
                resultBox.Font = new Font("Segoe UI", newSize);
            }
        }

        private void Makebuttons(ButtonStruct[,] bottoni)
        {
            int buttonWidth = 82;
            int buttonHeight = 60;
            int posX = 0;
            int posY = 101;

            for (int i = 0; i < bottoni.GetLength(0); i++)
            {
                for (int j = 0; j < bottoni.GetLength(1); j++)
                {
                    Button newButton = new Button();
                    newButton.Font = new Font("Segoe UI", 16);
                    ButtonStruct bs = buttons[i, j];
                    //newButton.Text = bottoni[i, j].Content.ToString();
                    newButton.Text = bs.ToString();
                    if (bs.IsBold)
                    {
                        newButton.Font = new Font(newButton.Font, FontStyle.Bold);
                    }
                    newButton.Width = buttonWidth;
                    newButton.Height = buttonHeight;
                    newButton.Left = posX;
                    newButton.Top = posY;
                    newButton.Tag = bs;
                    newButton.Click += Button_Click;
                    this.Controls.Add(newButton);
                    posX += buttonWidth;
                }
                posX = 0;
                posY += buttonHeight;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            ButtonStruct bs = (ButtonStruct)clickedButton.Tag;
            if (bs.IsNumber)
            {
                if (lastButtonClicked.IsEqualSign)
                {
                    clearAll();
                }
                if (resultBox.Text == "0" || lastButtonClicked.IsOperator)
                {
                    resultBox.Text = "";    // tolgo lo 0
                }
                resultBox.Text += clickedButton.Text;
            }
            else
            {
                if (bs.IsDecimalSeparator)
                {
                    if (!resultBox.Text.Contains(bs.Content.ToString()))
                    {
                        resultBox.Text += clickedButton.Text;
                    }
                }
                if (bs.IsPlusMinusSign)
                {
                    if (!resultBox.Text.Contains("-"))
                    {
                        resultBox.Text = "-" + resultBox.Text;
                    }
                    else
                    {
                        resultBox.Text = resultBox.Text.Replace("-", "");
                    }
                }
                else
                {
                    switch (bs.Content)
                    {
                        case 'C':
                            clearAll();
                            break;
                        case '⩤':
                            resultBox.Text = resultBox.Text.Remove(resultBox.Text.Length - 1);  // tolgo l'ultimo elemento
                            if (resultBox.Text.Length == 0 || resultBox.Text == "-0" || resultBox.Text == "-")
                            {
                                resultBox.Text = "0";
                            }
                            break;
                        default:
                            if (bs.IsOperator) manageOperators(bs);
                            break;
                    }
                }
            }
            lastButtonClicked = bs;
        }

        private string getFormattedNumber(double number)
        {
            char decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];  //va a vedere qual è la localizzazione 
                                                                                                                   //del sistema e userà quel determinato separatore decimale
            return number.ToString("N16").TrimEnd('0').TrimEnd(decimalSeparator); //trimEnd= taglio dal fondo
        }

        private void clearAll(double numberToWrite = 0)
        {
            operand1 = 0;
            operand2 = 0;
            result = 0;
            lastOperator = ASCIIZERO;
            resultBox.Text = getFormattedNumber(numberToWrite);
        }

        private void manageOperators(ButtonStruct bs)
        {
            double specialOperatorResult;
            if (bs.IsSpecialOperator)
            {
                switch (bs.Content)
                {
                    case '⅟':
                        specialOperatorResult = 1 / (Convert.ToDouble(resultBox.Text));
                        if (lastOperator != ASCIIZERO)
                        {
                            operand2 = specialOperatorResult;
                            
                        }
                        else
                        {
                            result = specialOperatorResult;
                            resultBox.Text = getFormattedNumber(result);
                        }
                        break;
                    case '^':
                        if (lastOperator != ASCIIZERO)
                        {
                            operand2 = Math.Pow(Convert.ToDouble(resultBox.Text), 2);
                            resultBox.Text = getFormattedNumber(operand2);
                        }
                        else
                        {
                            result = Math.Pow(Convert.ToDouble(resultBox.Text), 2);
                            resultBox.Text = getFormattedNumber(result);
                        }
                        break;
                    case '√':
                        if (lastOperator != ASCIIZERO)
                        {
                            operand2 = Math.Sqrt(Convert.ToDouble(resultBox.Text));
                            resultBox.Text = getFormattedNumber(operand2);
                        }
                        else
                        {
                            result = Math.Sqrt(Convert.ToDouble(resultBox.Text));
                            resultBox.Text = getFormattedNumber(result);
                        }
                        break;
                }
                lastButtonClicked = bs;
            }
            if (lastOperator == ASCIIZERO)
            {
                operand1 = double.Parse(resultBox.Text);
                lastOperator = bs.Content;
            }
            else
            {

                if (lastButtonClicked.IsOperator && !lastButtonClicked.IsEqualSign)
                {
                    if (bs.IsSpecialOperator)
                    {
                        result = operand2;
                        resultBox.Text = getFormattedNumber(result);
                        
                    }
                    else
                    {
                        lastOperator = bs.Content;
                    }

                }
                else
                {
                    
                    if (!lastButtonClicked.IsEqualSign && !bs.IsSpecialOperator)
                    {
                        operand2 = double.Parse(resultBox.Text);
                    }

                    switch (lastOperator)
                    {
                        case '+':
                            result = operand1 + operand2;
                            break;
                        case '-':
                            result = operand1 - operand2;
                            break;
                        case 'x':
                            result = operand1 * operand2;
                            break;
                        case '/':
                            result = operand1 / operand2;
                            break;
                        default:
                            break;
                    }
                    operand1 = result;
                    if (!bs.IsEqualSign && !bs.IsSpecialOperator)
                    {
                        lastOperator = bs.Content;
                        operand2 = 0;
                    }
                    
                    resultBox.Text = getFormattedNumber(result);
                }
            }
            
        }
    }
}

