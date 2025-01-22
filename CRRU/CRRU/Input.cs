using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRRU
{
    internal class Input
    {
        string data = "";
        public Input(string variableName)
        {
            inputBox inputBox = new inputBox();
            inputBox.Text = "Input";
            inputBox.label1.Text = "ป้อนค่า " + variableName;
            inputBox.ShowDialog();
            data = inputBox.UserInput;
        }
        public string getData()
        {
            return data;
        }
    }
}
