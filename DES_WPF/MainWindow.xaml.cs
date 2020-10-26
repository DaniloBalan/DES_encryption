using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.VisualBasic;
using System.Drawing;

using Microsoft.Win32;
using System.IO;

namespace DES_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       

        private void radiobutton_decrypt_Checked(object sender, RoutedEventArgs e)
        {
            Solution_taken_en.Fill = Brushes.Green;
            if (keyword.Text.Length == 8)
            { 
                
                start_button.Visibility = Visibility.Visible;
                Wrong_key_length.Visibility = Visibility.Hidden;
                Check_lable.Content = "s";
            }
            else
            {
                Key_entered_el.Fill = Brushes.Red;
                start_button.Visibility = Visibility.Hidden;
                Wrong_key_length.Visibility = Visibility.Visible;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Solution_taken_en.Fill = Brushes.Green;
            if (keyword.Text.Length == 8)
            {
 
                start_button.Visibility = Visibility.Visible;
                Wrong_key_length.Visibility = Visibility.Hidden;
                Check_lable.Content = "s";
            }
            else
            {
                Key_entered_el.Fill = Brushes.Red;
                start_button.Visibility = Visibility.Hidden;
                Wrong_key_length.Visibility = Visibility.Visible;
            }
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (keyword.Text.Length != 8)
            {
                Key_entered_el.Fill = Brushes.Red;
            }

            Path_entered_el.Fill = Brushes.Yellow;
            OpenFileDialog OD = new OpenFileDialog();
            OD.Filter = "All Files|*";
            OD.FileName = "";

            if (OD.ShowDialog() == DialogResult.HasValue)
            {
                Filepath.Text = "";
                Path_entered_el.Fill = Brushes.Red;
            }
            else
                Filepath.Text = OD.FileName;

            
        }

        private void filepath_TextChanged(object sender, TextChangedEventArgs e)
        {
            

            if (Filepath.Text.Length==0)
            {
                Path_entered_el.Fill = Brushes.Red;
            }
            else
            {
                Path_entered_el.Fill = Brushes.Green;
                if (Check_lable.Content=="s" && Key_checked.Content=="s")
                {
                    start_button.Visibility = Visibility.Visible;
                }
            }
            if (keyword.Text.Length == 8)
            {
                start_button.Visibility = Visibility.Visible;  
            }
            
            else
            {
                Key_entered_el.Fill = Brushes.Red;
            }
        }

        private void keyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            Key_entered_el.Fill = Brushes.Yellow;

            if (keyword.Text.Length == 8)
            {
                Key_entered_el.Fill = Brushes.Green;
                Wrong_key_length.Visibility = Visibility.Hidden;
                Key_checked.Content = "s";
                if (Check_lable.Content=="s")
                {
                start_button.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Key_checked.Content = "";
                start_button.Visibility = Visibility.Hidden;
                Wrong_key_length.Visibility = Visibility.Visible;
            }
            
        } 
        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            DES des = new DES();
            Blocks blocks = new Blocks();

            bool encryptOrDecrypt = false;

            string filepath = Filepath.Text;
            string keyValue = keyword.Text;
            string currentDir = "";
            string currentName = "";
            string workGroup_1 = $@"{currentDir}\{currentName}";
            string workGroup_2 = "";
            string workGroup_3 = "";
            string endpointText = "";
            string message, title, defaultValue;
            object fileName;

            int mLength = 0;
            int zeroBits = 0;
            int length = 0;
            int count = 0;
            int ammoutOfRounds = 16;

            try
            {
                currentDir = Path.GetDirectoryName(filepath);
                currentName = Path.GetFileName(filepath);
            }
            catch (Exception)
            {
                MessageBox.Show("No such file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] binaryBits = blocks.GetBinaryBits(filepath);
            byte[] key = blocks.GetKey(keyValue);
            byte[] currentBlock = new byte[64];
            byte[,] blockText = blocks.DoBlocks(binaryBits, out mLength, out zeroBits);
            byte[,] addedBlock = blocks.AddBlcok(blockText, zeroBits, mLength);
            byte[] endpointBlock;
            byte[] correctedMatrix;
            byte[] readyBytes;

            


            if (radiobutton_encrypt.IsChecked == true)
            {
                encryptOrDecrypt = true;
                length = mLength + 1;
            }
            else if (radiobutton_decrypt.IsChecked == true)
            {
                encryptOrDecrypt = false;
                length = mLength;
            }
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (radiobutton_encrypt.IsChecked == true)
                        currentBlock[j] = addedBlock[i, count];
                    else if (radiobutton_decrypt.IsChecked == true)
                        currentBlock[j] = blockText[i, count];
                    count++;
                }
                count = 0;
                des.Rounds(ammoutOfRounds, key, currentBlock, encryptOrDecrypt, out endpointBlock);
                foreach (byte t in endpointBlock)
                    endpointText += t.ToString();
            }
            if (radiobutton_decrypt.IsChecked == true)
            {
                try
                {
                    correctedMatrix = blocks.DoCorrection(endpointText);
                    endpointText = "";
                    foreach (byte t in correctedMatrix)
                        endpointText += t.ToString();
                }
                catch (Exception)
                {
                    
                    Interaction.MsgBox("Wrong key word!!!", MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "Error");
                    return;
                }
            }
            readyBytes = blocks.ReverseByte(endpointText);

            if (radiobutton_encrypt.IsChecked == true)
            {
                message = "Please input filename:";
                title = "DES encryption";
                defaultValue = "Encrypted message.txt";
                fileName = Interaction.InputBox(message, title, defaultValue);
                workGroup_2 = $@"{currentDir}\{(string)fileName}";
                if ((string)fileName != "")
                    File.WriteAllBytes(workGroup_2, readyBytes);
                MessageBox.Show($"File has been encrypted successfully!\nFilepath:{workGroup_2}", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (radiobutton_decrypt.IsChecked == true)
            {
                message = "Please input new filename:";
                title = "DES encryption";
                defaultValue = "Decrypted message.txt";
                fileName = Interaction.InputBox(message, title, defaultValue);
                workGroup_3 = $@"{currentDir}\{(string)fileName}";
                if ((string)fileName != "")
                    File.WriteAllBytes(workGroup_3, readyBytes);
                MessageBox.Show($"File has been decrypted successfully!\nFilepath:{workGroup_3}", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }




        public partial class Blocks
        {
            public byte[] GetBytes(string filepath)
            {
                byte[] bits;
                bits = File.ReadAllBytes(filepath);
                return bits;
            }

            public byte[] GetBinaryBits(string filepath)
            {
                byte[] byteText = GetBytes(filepath);
                string str = "";
                byte[] binaryBits;
                foreach (byte i in byteText)
                {
                    str += Convert.ToString(i, 2).PadLeft(8, '0');
                }
                binaryBits = new byte[str.Length];
                for (int i = 0; i < str.Length; i++)
                    binaryBits[i] = Convert.ToByte(str[i].ToString());
                return binaryBits;
            }

            public byte[,] DoBlocks(byte[] binaryText, out int mLength, out int zeroByte)
            {
                mLength = 0;
                if (binaryText.Length != 64)
                    zeroByte = 64 - (binaryText.Length % 64);
                else
                    zeroByte = 0;
                int counter = 0;
                if (binaryText.Length % 64 == 0)
                    mLength = binaryText.Length / 64;
                else if (binaryText.Length % 64 != 0)
                    mLength = (binaryText.Length / 64) + 1;

                byte[,] byteMatrix = new byte[mLength, 64];

                for (int i = 0; i < mLength; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        if (counter < binaryText.Length)
                            byteMatrix[i, j] = binaryText[counter];
                        counter++;
                    }
                }
                return byteMatrix;
            }

            public byte[,] AddBlcok(byte[,] byteMatrix, int zeroByte, int mLength)
            {
                byte[,] newByteMatrix = new byte[mLength + 1, 64];
                for (int i = 0; i < mLength; i++)
                    for (int j = 0; j < 64; j++)
                        newByteMatrix[i, j] = byteMatrix[i, j];
                string addBytes = "";
                addBytes = Convert.ToString(zeroByte, 2).PadLeft(64, '0');
                for (int i = 0; i < 64; i++)
                    newByteMatrix[mLength, i] = Convert.ToByte(addBytes[i].ToString());
                return newByteMatrix;
            }

            public byte[] GetKey(string strKey)
            {
                string binaryText = "";
                byte[] bytes;
                byte[] key64 = new byte[64];
                bytes = Encoding.UTF8.GetBytes(strKey);

                foreach (byte i in bytes)
                    binaryText += Convert.ToString(i, 2).PadLeft(8, '0');

                for (int i = 0; i < key64.Length; i++)
                    key64[i] = Byte.Parse(binaryText[i].ToString());
                return key64;
            }

            public byte[] DoCorrection(string endpointText)
            {
                byte[] openText = new byte[endpointText.Length];
                for (int i = 0; i < openText.Length; i++)
                    openText[i] = Convert.ToByte(endpointText[i].ToString());

                string ammountOfCorrectionBytes = "";
                string AmmountOfCorrectionBytes = "";
                int correctionBytes = 0;
                byte[] temp = new byte[64];
                byte[] temp_1 = new byte[64];
                for (int i = 0, j = openText.Length - 64; i < 64; i++, j++)
                    temp[i] = openText[j];
                for (int i = 0; i < 64; i++)
                    ammountOfCorrectionBytes += temp[i].ToString();
                for (int i = 0; i < ammountOfCorrectionBytes.Length; i++)
                    temp_1[i] = Convert.ToByte(ammountOfCorrectionBytes[i].ToString());
                for (int i = 0; i < ammountOfCorrectionBytes.Length; i++)
                    AmmountOfCorrectionBytes += temp_1[i].ToString();
                correctionBytes = Convert.ToInt32(AmmountOfCorrectionBytes, 2);
                byte[] correctedMatrix = new byte[openText.Length - correctionBytes - 64];
                for (int i = 0; i < correctedMatrix.Length; i++)
                    correctedMatrix[i] = openText[i];
                return correctedMatrix;
            }


            public byte[] ReverseByte(string endpointText)
            {
                byte[] endpointMatrix = new byte[endpointText.Length];
                byte[] currentMatrix = new byte[8];
                byte[] outputBytes = new byte[endpointText.Length / 8];
                string str = "";
                int counter = 0;
                int outputCounter = 0;
                int tmp = 0;

                for (int i = 0; i < endpointMatrix.Length; i++)
                    endpointMatrix[i] = Convert.ToByte(endpointText[i].ToString());

                for (int i = 0; i <= endpointMatrix.Length; i++)
                {
                    if (i % 8 == 0 && i != 0)
                    {
                        counter = 0;
                        for (int j = 0; j < 8; j++)
                            str += Convert.ToString(currentMatrix[j]);
                        tmp = Convert.ToInt32(str, 2);
                        outputBytes[outputCounter] = (byte)tmp;
                        outputCounter++;
                        str = "";
                    }
                    if (i == endpointMatrix.Length)
                        break;
                    currentMatrix[counter] = endpointMatrix[i];
                    counter++;
                }
                return outputBytes;
            }
        }

        public partial class DES
        {
            readonly int[] compression_key_perm_matrix = new int[] { 14,17,11,24, 1, 5, 3,28,15, 6,21,10,
                                                                 23,19,12 ,4,26, 8,16, 7,27,20,13, 2,
                                                                 41,52,31,37,47,55,30,40,51,45,33,48,
                                                                 44,49,39,56,34,53,46,42,50,36,29,32 };
            readonly int[] shiftMatrix = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
            readonly int[] init_perm_matrix = new int[] { 58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,
                                            62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,
                                            57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
                                            61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7 };
            readonly int[] expansion_permutation_matrix = new int[] { 32,1,2,3,4,5,4,5,6,7,8,9,
                                    8,9,10,11,12,13,12,13,14,15,16,17,
                                    16,17,18,19,20,21,20,21,22,23,24,25,
                                    24,25,26,27,28,29,28,29,30,31,32,1 };

            readonly int[,] sbox_1 = new int[4, 16] { { 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7 },
                                        { 0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8},
                                        {4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0 },
                                        { 15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13}};

            readonly int[,] sbox_2 = new int[4, 16] { { 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10 },
                                        { 3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5},
                                        {0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15 },
                                        { 13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9}};

            readonly int[,] sbox_3 = new int[4, 16] { { 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8 },
                                        { 13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1},
                                        {13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7},
                                        { 1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12}};

            readonly int[,] sbox_4 = new int[4, 16] { { 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15 },
                                        { 13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9},
                                        {10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4 },
                                        { 3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14}};

            readonly int[,] sbox_5 = new int[4, 16] { { 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9 },
                                        { 14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6},
                                        {4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14 },
                                        { 11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3}};

            readonly int[,] sbox_6 = new int[4, 16] { { 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11 },
                                        { 10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8},
                                        {9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6 },
                                        {4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13}};

            readonly int[,] sbox_7 = new int[4, 16] { { 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1 },
                                        {13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6},
                                        {1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2},
                                        {6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12}};

            readonly int[,] sbox_8 = new int[4, 16] { { 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7 },
                                        {  1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2},
                                        {7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8},
                                        { 2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11}};

            readonly private int[] pbox = new int[] { 16,7,20,21,29,12,28,17,1,15,23,26,5,18,31,10,
                                         2,8,24,14,32,27,3,9,19,13,30,6,22,11,4,25 };

            readonly private int[] fp = new int[] { 40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,
                                       38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,
                                       36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,
                                       34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25 };

            public void DenyEveryEighthBit(byte[] key64, out byte[] key56)
            {
                key56 = new byte[56];
                for (int i = 0, j = 0; i < key56.Length; j++)
                {
                    if ((j + 1) % 8 != 0)
                    {
                        key56[i] = key64[j];
                        i++;
                    }
                }
            }
            public void Split_Key_into_c_d_parts(byte[] key56, out byte[] cKey, out byte[] dKey)
            {
                cKey = new byte[28];
                dKey = new byte[28];
                for (int i = 0; i < 28; i++)
                    cKey[i] = key56[i];
                for (int j = 0, i = 28; i < 56; i++, j++)
                    dKey[j] = key56[i];
            }
            public void LeftShift(byte[] cKey, byte[] dKey, out byte[] shiftedCkey, out byte[] shiftedDkey)
            {
                shiftedCkey = new byte[28];
                shiftedDkey = new byte[28];
                byte firsCtBit = cKey[0];
                byte firsDtBit = dKey[0];

                for (int i = 0; i < 27; i++)
                    shiftedCkey[i] = cKey[i + 1];
                shiftedCkey[27] = firsCtBit;
                for (int i = 0; i < 27; i++)
                    shiftedDkey[i] = dKey[i + 1];
                shiftedDkey[27] = firsDtBit;
            }
            public void Connect_shifted_key_parts(byte[] shiftedCkey, byte[] shiftedDkey, out byte[] joinedKey)
            {
                joinedKey = new byte[56];
                for (int i = 0; i < shiftedCkey.Length; i++)
                    joinedKey[i] = shiftedCkey[i];

                for (int i = shiftedCkey.Length, j = 0; i < joinedKey.Length; i++, j++)
                    joinedKey[i] = shiftedDkey[j];
            }
            public void Compression_Permutation(byte[] joinedKey, out byte[] compressedKey)
            {
                compressedKey = new byte[48];
                int index;
                for (int i = 0; i < compressedKey.Length; i++)
                {
                    index = compression_key_perm_matrix[i] - 1;
                    compressedKey[i] = joinedKey[index];
                }
            }
            public void Key_Creation(byte[] key64, out byte[,] keyMatrix, int amountOfRounds)
            {
                keyMatrix = new byte[amountOfRounds, 48];
                byte[] key56;
                byte[] cKey;
                byte[] dKey;
                byte[] shiftedCKey = new byte[28];
                byte[] shiftedDKey = new byte[28];
                byte[] joinedKey;
                byte[] compressedKey;
                int n;
                DenyEveryEighthBit(key64, out key56);
                Split_Key_into_c_d_parts(key56, out cKey, out dKey);

                for (int q = 0; q < amountOfRounds; q++)
                {
                    n = shiftMatrix[q];
                    for (int i = 0; i < n; i++)
                    {
                        LeftShift(cKey, dKey, out shiftedCKey, out shiftedDKey);
                        cKey = shiftedCKey;
                        dKey = shiftedDKey;
                    }
                    Connect_shifted_key_parts(shiftedCKey, shiftedDKey, out joinedKey);
                    Compression_Permutation(joinedKey, out compressedKey);
                    for (int i = 0; i < compressedKey.Length; i++)
                        keyMatrix[q, i] = compressedKey[i];
                }
            }

            public void DoInitPermutation(byte[] currentBlock, out byte[] permBlock)
            {
                permBlock = new byte[64];
                int index;
                for (int i = 0; i < currentBlock.Length; i++)
                {
                    index = init_perm_matrix[i] - 1;
                    permBlock[i] = currentBlock[index];
                }
            }

            public void DivideTo_LPT_and_RPT(byte[] currentBlock, out byte[] LPT, out byte[] RPT)
            {
                LPT = new byte[32];
                RPT = new byte[32];
                for (int i = 0; i < 32; i++)
                    LPT[i] = currentBlock[i];
                for (int i = LPT.Length, j = 0; j < 32; i++, j++)
                    RPT[j] = currentBlock[i];
            }

            public void Expansion_Permutation_Enctrypt(byte[] RPT, out byte[] expandedRPT)
            {
                int index;
                expandedRPT = new byte[48];
                for (int i = 0; i < expandedRPT.Length; i++)
                {
                    index = expansion_permutation_matrix[i] - 1;
                    expandedRPT[i] = RPT[index];
                }
            }

            public void XOR_for_Encrypt(byte[] currentBlock, byte[] key, out byte[] XORedRPT)
            {
                XORedRPT = new byte[48];
                for (int i = 0; i < currentBlock.Length; i++)
                {
                    if (currentBlock[i] == 0 && key[i] == 0)
                        XORedRPT[i] = 0;
                    if (currentBlock[i] == 0 && key[i] == 1)
                        XORedRPT[i] = 1;
                    if (currentBlock[i] == 1 && key[i] == 0)
                        XORedRPT[i] = 1;
                    if (currentBlock[i] == 1 && key[i] == 1)
                        XORedRPT[i] = 0;
                }
            }

            public void S_Box_Replacement(byte[] XORed_RPT, out byte[] S_Box_RPT)
            {
                byte[,] preparedText = new byte[8, 6];
                S_Box_RPT = new byte[32];
                int counter = 0;
                int firstByte;
                int lastByte;
                int row;
                int colums;
                string box = "";
                string convert_1;
                string convert_2 = "";


                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        preparedText[i, j] = XORed_RPT[counter];
                        counter++;
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    firstByte = preparedText[i, 0];
                    lastByte = preparedText[i, 5];

                    convert_1 = firstByte.ToString() + lastByte.ToString();
                    for (int l = 1; l < 5; l++)
                        convert_2 += preparedText[i, l];
                    row = Convert.ToInt32(convert_1, 2);
                    colums = Convert.ToInt32(convert_2, 2);
                    if (i == 0)
                        box += Convert.ToString(sbox_1[row, colums], 2).PadLeft(4, '0');
                    else if (i == 1)
                        box += Convert.ToString(sbox_2[row, colums], 2).PadLeft(4, '0');
                    else if (i == 2)
                        box += Convert.ToString(sbox_3[row, colums], 2).PadLeft(4, '0');
                    else if (i == 3)
                        box += Convert.ToString(sbox_4[row, colums], 2).PadLeft(4, '0');
                    else if (i == 4)
                        box += Convert.ToString(sbox_5[row, colums], 2).PadLeft(4, '0');
                    else if (i == 5)
                        box += Convert.ToString(sbox_6[row, colums], 2).PadLeft(4, '0');
                    else if (i == 6)
                        box += Convert.ToString(sbox_7[row, colums], 2).PadLeft(4, '0');
                    else if (i == 7)
                        box += Convert.ToString(sbox_8[row, colums], 2).PadLeft(4, '0');
                    convert_1 = "";
                    convert_2 = "";
                }
                for (int i = 0; i < S_Box_RPT.Length; i++)
                    S_Box_RPT[i] = Byte.Parse(box[i].ToString());
            }

            public void P_Box_Permutation(byte[] S_Box_RPT, out byte[] p_Box_permutated)
            {
                p_Box_permutated = new byte[S_Box_RPT.Length];
                int index;
                for (int i = 0; i < pbox.Length; i++)
                {
                    index = pbox[i] - 1;
                    p_Box_permutated[i] = S_Box_RPT[index];
                }
            }
            public void XOR_LPT_for_Encrypt(byte[] LPT, byte[] S_Box_RPT, out byte[] newRPT)
            {
                newRPT = new byte[32];
                for (int i = 0; i < S_Box_RPT.Length; i++)
                {
                    if (LPT[i] == 0 && S_Box_RPT[i] == 0)
                        newRPT[i] = 0;
                    else if (LPT[i] == 0 && S_Box_RPT[i] == 1)
                        newRPT[i] = 1;
                    else if (LPT[i] == 1 && S_Box_RPT[i] == 0)
                        newRPT[i] = 1;
                    else if (LPT[i] == 1 && S_Box_RPT[i] == 1)
                        newRPT[i] = 0;
                }
            }

            public void Swap(byte[] LPT, byte[] RPT, out byte[] finalBlock)
            {
                byte[] temp = new byte[LPT.Length];
                finalBlock = new byte[64];
                temp = LPT;
                LPT = RPT;
                RPT = temp;
                for (int i = 0, j = LPT.Length; i < LPT.Length; i++, j++)
                {
                    finalBlock[i] = LPT[i];
                    finalBlock[j] = RPT[i];
                }

            }

            public void Final_Permutation(byte[] binnaryBlock, out byte[] cypheredBlock)
            {
                cypheredBlock = new byte[64];
                int index;
                for (int i = 0; i < cypheredBlock.Length; i++)
                {
                    index = fp[i] - 1;
                    cypheredBlock[i] = binnaryBlock[index];
                }
            }

            public byte[,] Rounds(int amountOfRounds, byte[] key, byte[] currentBlock, bool EncryptOrDecrypt, out byte[] cypheredBlock)
            {
                byte[] permBlock;
                byte[] LPT;
                byte[] RPT;
                byte[] expandedRPT;
                byte[] XORedRPT;
                byte[] currentKey = new byte[48];
                byte[] S_Box_RPT;
                byte[] p_Box_permutated;
                byte[,] keyMatrix;
                byte[] newRPT = new byte[32];
                byte[] prevRPT = new byte[32];
                byte[] finalBlock;
                cypheredBlock = new byte[64];

                byte[,] RPTLPT = new byte[amountOfRounds, 64];

                Key_Creation(key, out keyMatrix, amountOfRounds);
                DoInitPermutation(currentBlock, out permBlock);
                DivideTo_LPT_and_RPT(permBlock, out LPT, out RPT);
                int r = amountOfRounds - 1;
                for (int q = 0; q < amountOfRounds; q++)
                {
                    prevRPT = RPT;
                    if (EncryptOrDecrypt)
                    {
                        for (int i = 0; i < currentKey.Length; i++)
                            currentKey[i] = keyMatrix[q, i];
                    }
                    else
                    {
                        for (int i = 0; i < currentKey.Length; i++)
                            currentKey[i] = keyMatrix[r, i];
                        r--;
                    }

                    Expansion_Permutation_Enctrypt(RPT, out expandedRPT);
                    XOR_for_Encrypt(expandedRPT, currentKey, out XORedRPT);
                    S_Box_Replacement(XORedRPT, out S_Box_RPT);
                    P_Box_Permutation(S_Box_RPT, out p_Box_permutated);
                    XOR_LPT_for_Encrypt(LPT, p_Box_permutated, out newRPT);
                    LPT = prevRPT;
                    RPT = newRPT;
                    int c = 0;
                    for (int i = 0; i < 64; i++)
                    {
                        if (i == 32)
                            c = 0;
                        if (i < 32)
                            RPTLPT[q, i] = LPT[c];
                        else
                            RPTLPT[q, i] = RPT[c];
                        c++;
                    }
                }
                Swap(LPT, RPT, out finalBlock);
                Final_Permutation(finalBlock, out cypheredBlock);
                return RPTLPT;
            }

            public int[,] Chart_Difference(byte[,] RPTLPT_decrypted, byte[,] RPTLPT_encrypted, int ammountOfRounds)
            {
                int[,] diff = new int[ammountOfRounds, 1];
                int c = 0;
                for (int i = 0; i < ammountOfRounds; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        if (RPTLPT_decrypted[i, j] != RPTLPT_encrypted[i, j])
                            c++;
                    }
                    diff[i, 0] = c;
                    c = 0;
                }
                return diff;
            }
        }

        private void TextBox_original_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_Modified_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Calculate_the_difference_Click(object sender, RoutedEventArgs e)
        {
            Blocks blocks = new Blocks();
            DES des = new DES();

            string keyValue = keyword.Text;
            string originalText = TextBox_original.Text;
            string modifiedText = TextBox_Modified.Text;

            byte[] binaryOriginalBits = new byte[64];
            byte[] binaryModifiedBits = new byte[64];
            byte[] key = blocks.GetKey(keyValue);
            byte[] cyphered;
            byte[,] Original;
            byte[,] Modified;

            int amountOfRounds = 16;
            int[,] difference;

            for (int i = 0; i < binaryOriginalBits.Length; i++)
                binaryOriginalBits[i] = Convert.ToByte(originalText[i].ToString());
            for (int i = 0; i < binaryModifiedBits.Length; i++)
                binaryModifiedBits[i] = Convert.ToByte(modifiedText[i].ToString());

            Original = des.Rounds(amountOfRounds, key, binaryOriginalBits, true, out cyphered);
            Modified = des.Rounds(amountOfRounds, key, binaryModifiedBits, true, out cyphered);

            difference = des.Chart_Difference(Original, Modified, amountOfRounds);

            StreamWriter str = new StreamWriter("avalanche-effect.txt");
            for (int i = 0; i < amountOfRounds; i++)
            {
                str.Write($"Round {i + 1}: ");
                for (int j = 0; j < 64; j++)
                {
                    str.Write(Original[i, j]);
                }
                str.WriteLine();
                str.Write($"Round {i + 1}: ");
                for (int j = 0; j < 64; j++)
                {
                    str.Write(Modified[i, j]);
                }
                str.WriteLine();
                str.WriteLine($"Differense: {difference[i, 0]}");
                str.WriteLine("\n");
            }
            str.Close();
            MessageBox.Show("The difference was calculated and written to a file.");
        }
    }
}
