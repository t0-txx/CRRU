using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CRRU
{
    public partial class CRRU : Form
    {
        int errorCode = 0;
        List<Variable> variableList = new List<Variable>();

        public CRRU()
        {
            InitializeComponent();
            //string expression = "50<50";
            //var answer = new DataTable().Compute(expression,null);
            //MessageBox.Show(answer + "");

            //MessageBox.Show(convertExpression("a+30"));
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close?", "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                this.Close();
        }

        private void bNew_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Focus();
        }

        private void bRun_Click(object sender, EventArgs e)
        {
            errorCode = 0;
            variableList = new List<Variable>();
            richTextBox2.Clear();

            for (int i = 0; i < richTextBox1.Lines.Count(); i++)
            {
                string command = richTextBox1.Lines[i];

                //if (command.Length >= 6 && command.ToLower().Substring(0, 6).Equals("input "))
                if (command.ToLower().StartsWith("input "))
                {
                    executeInput(command);
                }
                //if (command.Length >= 7 && command.ToLower().Substring(0, 7).Equals("output "))
                else if (command.ToLower().StartsWith("output "))
                {
                    executeOutput(command, i);
                }
                else if (command.ToLower().StartsWith("if "))
                {
                    executeIf(command);
                }
                else if (command.Contains("="))
                {
                    executeProcess(command);
                }
                //else
                //{
                //    richTextBox2.SelectionColor = Color.Red;
                //    richTextBox2.AppendText("Error at line " + (i + 1));
                //    richTextBox2.SelectionColor = Color.Black;
                //    richTextBox2.AppendText(Environment.NewLine);
                //}
            }
        }

        private Boolean isConstant(string data)
        {
            try
            {
                double number = double.Parse(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Boolean isOpertor(string data)
        {
            data = data.Trim();
            //char[] otplist = { '+', '-', '*', '/' };
            //for (int i = 0; i < otplist.Length; i++)
            //{
            //    if (data[0] == otplist[i])
            //        return true;
            //}
            //return false;
            string otplist = "+-*/";
            if (otplist.IndexOf(data[0]) >= 0)
                return true;
            return false;
        }

        private Boolean isOpertor2(string data)
        {
            data = data.Trim();

            string[] operators = { ">", ">=", "<", "<=", "=", "<>" };

            if (operators.Contains(data))
                return true;

            return false;
        }

        private Boolean isString(string data)
        {
            if (data[0] == '"' && data[data.Length - 1] == '"')
            {
                return true;
            }
            else if (data[0] == '"' || data[data.Length - 1] == '"')
            {
                errorCode = 1;
            }
            //else 
            //{
            //    errorCode = 1;
            //}
            //else if (data.Length > 0 && !data.Contains("\""))
            //{
            //    errorCode = 2;
            //}
            return false;
        }

        private void showMessageError(int errorCode, int code)
        {
            richTextBox2.SelectionColor = Color.Red;
            switch (errorCode)
            {
                case 1: richTextBox2.AppendText("ระบุข้อความไม่ถูกต้อง " + code); break;
                    //case 2: richTextBox2.AppendText("ไม่มีเครื่องหมาย " + code); break;
            }
            richTextBox2.AppendText(Environment.NewLine);
        }

        private void executeInput(string command)
        {
            string[] vName = command.Substring(6).Split(',');
            //string[] Array = vName.Split(',');

            for (int j = 0; j < vName.Length; j++)
            {
                string input = new Input(vName[j].Trim()).getData();
                Variable v = new Variable();
                v.Name = vName[j].Trim();
                v.Value = input;
                variableList.Add(v);
                Debug.WriteLine(input);
            }
        }

        private void executeOutput(string command, int i)
        {
            string vOut = command.Substring(7).Trim();

            string[] outputParts = vOut.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string result = "";
            int code = 0;
            for (int j = 0; j < outputParts.Length; j++)
            {
                code++;
                outputParts[j] = outputParts[j].Trim();
                if (isConstant(outputParts[j]))
                {
                    result += outputParts[j];
                }
                //if (cleanedPart.StartsWith("\"") && cleanedPart.EndsWith("\""))
                else if (isString(outputParts[j]))
                {
                    outputParts[j] = outputParts[j].Substring(1, outputParts[j].Length - 2);
                    result += outputParts[j];
                }
                else if (errorCode != 0)
                {
                    showMessageError(errorCode, (i + 1));
                }
                else if (variableList.Count == 0)
                {
                    return;
                }

                for (int k = 0; k < variableList.Count; k++)
                {
                    if (variableList[k].Name == outputParts[j])
                    {
                        result += variableList[k].Value;
                        break;
                    }
                }
            }

            if (errorCode == 0)
            {
                richTextBox2.AppendText(result);
                richTextBox2.AppendText(Environment.NewLine);
            }
        }

        private void executeProcess(string command)
        {
            string[] parts = command.Split('=');
            if (parts.Length == 2)
            {
                string variableName = parts[0].Trim();
                string expression = parts[1].Trim();

                expression = convertExpression(parts[1].Trim());

                //MessageBox.Show(expression);

                //foreach (var variable in variableList)
                //{
                //    if (expression.Contains(variable.Name))
                //    {
                //        expression = expression.Replace(variable.Name, variable.Value);
                //    }
                //}

                try
                {
                    var result = new DataTable().Compute(expression, null);

                    Variable variable = variableList.FirstOrDefault(v => v.Name == variableName);
                    if (variable == null)
                    {
                        variable = new Variable() { Name = variableName };
                        variableList.Add(variable);
                    }
                    variable.Value = result.ToString();
                }
                catch (Exception ex)
                {
                    richTextBox2.SelectionColor = Color.Red;
                    richTextBox2.AppendText($"Error: Invalid expression ({ex.Message})");
                    richTextBox2.AppendText(Environment.NewLine);
                }
            }
            else
            {
                richTextBox2.SelectionColor = Color.Red;
                richTextBox2.AppendText("Error: Invalid assignment syntax");
                richTextBox2.AppendText(Environment.NewLine);
            }
        }

        private void executeIf(string command)
        {
            string condition = convertExpressionIF(command.Substring(3).Trim());

            //foreach (var variable in variableList)
            //{
            //    if (condition.Contains(variable.Name))
            //    {
            //        condition = condition.Replace(variable.Name, variable.Value);
            //    }
            //}

            MessageBox.Show(condition);

            try
            {
                var answer = new DataTable().Compute(condition, null);

                if ((bool)answer)
                {
                    richTextBox2.SelectionColor = Color.Blue;
                    richTextBox2.AppendText("True");
                    richTextBox2.AppendText(Environment.NewLine);
                }
                else
                {
                    richTextBox2.SelectionColor = Color.Red;
                    richTextBox2.AppendText("False");
                    richTextBox2.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                richTextBox2.SelectionColor = Color.Red;
                richTextBox2.AppendText($"Error: Invalid ({ex.Message})");
                richTextBox2.AppendText(Environment.NewLine);
            }
        }

        private string convertExpression(string exp) 
        {
            string[] token = System.Text.RegularExpressions.Regex.Split(exp, @"(?=[+\-*/^()])|(?<=[+\-*/^()])");
            //string[] token = System.Text.RegularExpressions.Regex.Split(exp, @"(?=[>|>=|<|<=|=|<>])|(?<=[>|>=|<|<=|=|<>])");
            string expression = "";
            for (int i = 0; i < token.Length;i++) 
            {
                if (isConstant(token[i]) || isOpertor(token[i])) 
                {
                    expression = expression + token[i];
                }
                else 
                {
                    for (int j = 0; j < variableList.Count; j++)
                    {
                        if (variableList[j].Name == token[i])
                        {
                            expression += variableList[j].Value;
                            MessageBox.Show(variableList[j].Value);
                        }
                    }
                }
            }
            return expression;
        }

        private string convertExpressionIF(string exp)
        {
            //string[] token = System.Text.RegularExpressions.Regex.Split(exp, @"(?=[+\-*/^()])|(?<=[+\-*/^()])");
            string[] token = System.Text.RegularExpressions.Regex.Split(exp, @"(?=[>|>=|<|<=|=|<>])|(?<=[>|>=|<|<=|=|<>])");
            string expression2 = "";
            for (int i = 0; i < token.Length; i++)
            {
                if (isConstant(token[i]) || isOpertor2(token[i]))
                {
                    expression2 = expression2 + token[i];
                }
                else
                {
                    for (int j = 0; j < variableList.Count; j++)
                    {
                        if (variableList[j].Name == token[i])
                        {
                            expression2 += variableList[j].Value;
                        }
                    }
                }
            }
            return expression2;
        }
    }
}

class Variable {
    public string Name;
    public string Value;
}