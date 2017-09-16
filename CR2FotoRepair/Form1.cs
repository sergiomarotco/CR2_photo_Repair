using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CR2_photo_Repair
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //d.Filter = "CR2 photos (*.cr2)|*.cr2|All files (*.*)|*.*";
            d.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();            
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                Goodfoto = d.FileName;
            }
        }

        private string Goodfoto = @"C:\default.cr2";
        private string folder = @"C:\";
        private List<string> badFiles;
        //private byte[] GoodfotoArray;

        private void Button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog F = new FolderBrowserDialog()
            {
                Description = "Choose a folder with *.cr2 photos to be restored",
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = F.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Directory.Exists(F.SelectedPath))
                {
                    folder = F.SelectedPath;
                }
                else
                {
                    MessageBox.Show("A non-existent folder is selected");
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            badFiles = new List<string>();
            badFiles.AddRange(System.IO.Directory.GetFiles(folder, "*.cr2"));
            byte[] GoodfotoArray = File.ReadAllBytes(Goodfoto);
            byte[] GoodfotoWithoutRight=new byte[1];
  /**/     // GoodfotoArray = new byte[] { 109, 110, 111, 255, 216, 255, 196, 112, 113, 114,115 };
            for (int j = 0; j < GoodfotoArray.Length; j++)
            {
                if (GoodfotoArray[j] == 255)
                    if (GoodfotoArray[j + 1] == 216)
                        if (GoodfotoArray[j + 2] == 255)
                            if (GoodfotoArray[j + 3] == 196)
                            {
                                GoodfotoWithoutRight = new byte[j];
                                Array.Copy(GoodfotoArray, 0, GoodfotoWithoutRight, 0, j);
                                break;
                            }
            }
            int NotRepairedFoto = 0;
            string NotRepairedFotostring = "";
            string newfilename = "";
            for (int i = 0; i < badFiles.Count; i++)
            {
                try
                {
                    FileInfo ff = new FileInfo(badFiles[i]);
                    Directory.CreateDirectory(ff.Directory.ToString() + @"\\repaired2");
                    newfilename = ff.Directory + "\\" + ff.Name.Split('.')[0] + "..JPG";
                    File.Copy(badFiles[i], newfilename, true);
                    badFiles[i] = newfilename;
                    byte[] BadfotoWithoutLeft = new byte[1];
                    byte[] foto = File.ReadAllBytes(badFiles[i]);
                    /**/            //foto = new byte[] { 209,210, 211, 255, 216, 255, 196, 255, 216, 255, 196, 212, 213,214,215 };
                    List<int> jIndexes = new List<int>();
                    for (int j = 0; j < foto.Length; j++)
                    {
                        if (foto[j] == 255)
                            if (foto[j + 1] == 216)
                                if (foto[j + 2] == 255)
                                    if (foto[j + 3] == 196)
                                        jIndexes.Add(j);

                            jIndexes.Add(j);
                    }
                    if (jIndexes.Count != 0)
                    {
                        NotRepairedFoto++;
                        BadfotoWithoutLeft = new byte[foto.Length - jIndexes[jIndexes.Count - 1]];
                        Array.Copy(foto, jIndexes[jIndexes.Count - 1], BadfotoWithoutLeft, 0, foto.Length - jIndexes[jIndexes.Count - 1]);
                        byte[] NewFoto = new byte[BadfotoWithoutLeft.Length + GoodfotoWithoutRight.Length];
                        Array.Copy(GoodfotoWithoutRight, 0, NewFoto, 0, GoodfotoWithoutRight.Length);
                        Array.Copy(BadfotoWithoutLeft, 0, NewFoto, GoodfotoWithoutRight.Length, BadfotoWithoutLeft.Length);
                        File.WriteAllBytes(newfilename, NewFoto);
                        File.Move(newfilename, ff.Directory + "\\repaired2\\" + ff.Name.Split('.')[0] + "......CR2");
                        File.Delete(newfilename);
                        newfilename = ff.Directory + "\\repaired2\\" + ff.Name.Split('.')[0] + "......CR2";
                        File.WriteAllBytes(newfilename, NewFoto);
                    }
                    else
                    { File.Delete(ff.Directory + "\\" + ff.Name.Split('.')[0] + "......CR2"); NotRepairedFotostring += ff.FullName + Environment.NewLine; }
                }
                catch (Exception ee) { MessageBox.Show(ee.Message + Environment.NewLine + Environment.NewLine + ee.ToString()); }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.icon;
            textBox1.Text = Goodfoto;
            textBox2.Text = folder;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ru.icons8.com");
        }
    }
}
