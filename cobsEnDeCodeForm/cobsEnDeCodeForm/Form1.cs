using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] test = new byte[12];
            byte[] resultEn = new byte[0];
            byte[] resultDc = new byte[0];
            test[0] = 0x45;
            test[1] = 0x00;
            test[2] = 0x00;
            test[3] = 0x2c;
            test[4] = 0x4c;
            test[5] = 0x79;
            test[6] = 0x00;
            test[7] = 0x00;
            test[8] = 0x40;
            test[9] = 0x06;
            test[10] = 0x4f;
            test[11] = 0x37;
            

            resultEn = COBSCodec.encode(test);
            resultDc = COBSCodec.decode(resultEn);

            //resultEn = COBSCodecC.  COBSCodec.encode(test);
            //resultDc = COBSCodec.decode(resultEn);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] hexDataIn = new byte[0];

            hexDataIn = StringToByteArrayFastest(textDataIn.Text);

            string hex = BitConverter.ToString(COBSCodec.encode(hexDataIn)).Replace("-", string.Empty);


            textEncoded.Text = hex;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] hexDataIn = new byte[0];
            hexDataIn = StringToByteArrayFastest(textEncoded.Text);

            string hex = BitConverter.ToString(COBSCodec.decode(hexDataIn)).Replace("-", string.Empty);
            textDecode.Text = hex;

        }
        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

   


    }




}
