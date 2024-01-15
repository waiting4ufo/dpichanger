using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace dpiChanger
{
    public partial class FrmMain : Form
    {

        private class JpegFormat{
            public FileStream fs;
            public int fmt;                      // 1:JFIF ; 2:EXIF
            public long blockLen;           //APP0 or APP1の長さ(識別子以降からの長さ）
            public long identifierOffset;  //識別子の位置（識別子含む） JFIF.ここ / EXIF..ここ
            public string errMsg;
        }//end class

        /// <summary>
        ///  X, Y方向の解像度値の保存位置情報
        /// </summary>
        private class ResolutionInfo
        {
            public short type;
            public int   cnt;
            public int   pos;
        }//end class

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dpiValue.Value = 96;
            directroyBox.Text = Environment.CurrentDirectory;

            clearMsg();
        }

        private void directroyBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = directroyBox.Text;
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                directroyBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            button1.Enabled = false;

            clearMsg();

            try
            {
                //指定ディレクトリ内のファイルを取得
                string[] files = Directory.GetFiles(directroyBox.Text, "*.jpg", SearchOption.AllDirectories);

                int fileCnt = 0;
                int okFileCnt = 0;

                bool isOkFlg;

                foreach (string file in files){
                    string msg = changeDpi(file, (Int16)dpiValue.Value, out isOkFlg);

                    fileCnt++;

                    if (isOkFlg)
                        okFileCnt++;

                    showMsg(0, fileCnt + "  " + msg);

                    Application.DoEvents();
                }//end foreach

                int ngFileCnt = fileCnt - okFileCnt;

                string fileCntMsg;
                fileCntMsg = string.Format("画像数:[{0,7}] DPI変更数:[{1,7}] フォーマット異常数:[{2,7}]", fileCnt, okFileCnt, ngFileCnt);

                showMsg(0, "");
                showMsg(0, "Processing have done!  " + fileCntMsg);
            } catch (Exception ex)
            {
                showMsg(0, ex.ToString());
            }//end try

            button1.Enabled = true;
        }

        private void showMsg(int level, string msg)
        {
            listBox.Items.Add(msg);
            listBox.TopIndex = listBox.Items.Count - 1;
        }

        private void clearMsg()
        {
            listBox.Items.Clear();
            listBox.Items.Add("DPI値と画像フォルダを指定し（ドラッグ＆ドロップ可能）、「実行」ボタンを押してください。");
            listBox.Items.Add("");
        }

        /// <summary>
        /// DPI値変更
        /// </summary>
        /// <param name="jpgfile">変更する画像ファイル名</param>
        /// <param name="dpi">変更するDPI値</param>
        /// <param name="isOkFlg">true:正常; false:異常発生</param>
        /// <returns></returns>
        private string changeDpi(string jpgfile, Int16 dpi, out bool isOkFlg)
        {
            string retMsg = "";
            string procSta = "";

            isOkFlg = false;

            try
            {
                //ファイルストリームを開いて、一連の操作を行う
                using (FileStream fs = new FileStream(jpgfile, FileMode.Open, FileAccess.ReadWrite))
                {

                    byte[] tmp = new byte[4];

                    fs.Read(tmp, 0, 2);
                    fs.Seek(-2, SeekOrigin.End);
                    fs.Read(tmp, 2, 2);

                    if (!(tmp[0] == 0xFF && tmp[1] == 0xD8 &&
                         tmp[2] == 0xFF && tmp[3] == 0xD9))  //JPGファイルではない
                    {
                        retMsg = jpgfile + " [Not a JPG file]";
                        return retMsg;
                    }//end if

                    //フォーマット特定
                    JpegFormat jpegFmt = new JpegFormat();
                    jpegFmt.fs = fs;
                    if (getFormat(ref jpegFmt) != 0)
                        throw new Exception(jpegFmt.errMsg);

                    if (jpegFmt.fmt == 1)  //  JPEG/JFIF
                    {
                        procSta = changeJFIFDpi(jpegFmt, dpi);
                    }
                    else if (jpegFmt.fmt == 2)  //  JPEG/EXIF
                    {
                        procSta = changeEXIFDpi(jpegFmt, dpi);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }//end using
            
            }
            catch(FileNotFoundException ex)
            {
                retMsg = jpgfile + "     [ファイル読み込み失敗。]";
                return retMsg;
            }
            catch(Exception ex)
            {
                string errMsg = ex.Message;
                if (String.IsNullOrEmpty(errMsg))
                    retMsg = jpgfile + "     [Not a JPG Format]";
                else
                    retMsg = jpgfile + "     " + errMsg;

                return retMsg;
            }//end try

            //create message
            retMsg = jpgfile + "     " + procSta;

            if (procSta.Equals("OK"))
                isOkFlg = true;

            return retMsg;
        }

        /// <summary>
        /// JPEGファイルのフォーマットを取得する。
        /// 
        /// </summary>
        /// <param name="jpegFmt">フォーマット</param>
        /// <returns>0:正常; 0以外：異常</returns>
        private int getFormat(ref JpegFormat jpegFmt)
        {
            byte[] buff2 = new byte[2];
            byte[] buff5 = new byte[5];
            byte[] buff10 = new byte[10];

            jpegFmt.fs.Seek(2, SeekOrigin.Begin);
            jpegFmt.fs.Read(buff2, 0, 2);

            if(buff2[0] == 0xFF && buff2[1] == 0xE0)  //APP0
            {
                jpegFmt.fmt = 1;  //JFIF

                //APP0長さ取得
                jpegFmt.fs.Read(buff2, 0, 2);
                Array.Reverse(buff2);
                jpegFmt.blockLen = BitConverter.ToInt16(buff2, 0);

                //次のマーク位置
                long nextMarkerPos = jpegFmt.fs.Position + jpegFmt.blockLen - 2;

                //JFIFフォーマットチェック
                jpegFmt.fs.Read(buff5, 0, 5);
                if (!(buff5[0] == 'J' && buff5[1] == 'F' && buff5[2] == 'I' && buff5[3] == 'F' && buff5[4] == 0x00))
                {
                    jpegFmt.errMsg = "JPEG/JFIFフォーマット異常";
                    return 1;
                }//end if

                jpegFmt.identifierOffset = jpegFmt.fs.Position;
                jpegFmt.blockLen -= 2 + 5;  //長さ自身の2バイトと識別子5バイトを引く

                //次にEXIFマーカがあるかどうか探してみる。
                //APP0マークの次の最初１個目のマークまで確認する。
                //次のマークがAPP1の場合、EXIFフォーマットとみなす。
                //次のマークがAPP1でない場合、もう探さない（本画像はJFIFとみなす）
                jpegFmt.fs.Seek(nextMarkerPos, SeekOrigin.Begin);
                jpegFmt.fs.Read(buff2, 0, 2);
                if(buff2[0] == 0xFF && buff2[1] == 0xE1)
                {
                    jpegFmt.fmt = 2;  //EXIFに変更

                    //APP1の長さ取得
                    jpegFmt.fs.Read(buff2, 0, 2);
                    Array.Reverse(buff2);
                    jpegFmt.blockLen = BitConverter.ToInt16(buff2, 0);

                    //EXIFフォーマットチェック
                    jpegFmt.fs.Read(buff10, 0, 6);
                    if (!(buff10[0] == 'E' && buff10[1] == 'x' && buff10[2] == 'i' && buff10[3] == 'f' && buff10[4] == 0x00 && buff10[5] == 0x00))
                    {
                        jpegFmt.errMsg = "JPEG/EXIFフォーマット異常(JFIF->EXIF)";
                        return 1;
                    }//end if

                    jpegFmt.identifierOffset = jpegFmt.fs.Position;
                    jpegFmt.blockLen -= 2 + 6;  //長さ自身の2バイトと識別子6バイトを引く
                }//end if
                    
            }
            else if(buff2[0] == 0xFF && buff2[1] == 0xE1)  //APP1
            {
                jpegFmt.fmt = 2;  //EXIF

                //APP1長さ取得
                jpegFmt.fs.Read(buff2, 0, 2);
                Array.Reverse(buff2);
                jpegFmt.blockLen = BitConverter.ToInt16(buff2, 0);

                //EXIFフォーマットチェック
                jpegFmt.fs.Read(buff10, 0, 6);
                if(!(buff10[0] == 'E' && buff10[1] == 'x' && buff10[2] == 'i' && buff10[3] == 'f' && buff10[4] == 0x00 && buff10[5] == 0x00))
                {
                    jpegFmt.errMsg = "JPEG/EXIFフォーマット異常";
                    return 1;
                }//end if

                jpegFmt.identifierOffset = jpegFmt.fs.Position;
                jpegFmt.blockLen -= 2 + 6;  //長さ自身の2バイトと識別子6バイトを引く
            }
            else
            {
                jpegFmt.errMsg = "内部のフォーマット異常";
                return 1;
            }//end if

            return 0;
        }

        /// <summary>
        /// JPEG/JFIFフォーマット画像のdpi値を変更する。
        /// 
        /// </summary>
        /// <param name="jpegFmt">フォーマット管理</param>
        /// <param name="dpiValue">dpi値</param>
        /// <returns>実行状態</returns>
        private string changeJFIFDpi(JpegFormat jpegFmt, Int16 dpiValue)
        {
            string ret = "OK";

            byte[] dpiBytes = new byte[2];
            dpiBytes = BitConverter.GetBytes(dpiValue);
            Array.Reverse(dpiBytes);                     // JFIFはMotorola固定なので、変換(C#はIntel）

            long offset = jpegFmt.identifierOffset + 2;  //version 2byte

            jpegFmt.fs.Seek(offset, SeekOrigin.Begin);
            jpegFmt.fs.WriteByte(0x01);  // 1:pixel/inch固定
            jpegFmt.fs.Write(dpiBytes, 0, 2);
            jpegFmt.fs.Write(dpiBytes, 0, 2);

            return ret;
        }

        /// <summary>
        /// JPEG/EXIFフォーマット画像のdpi値を変更する。
        /// 
        /// </summary>
        /// <param name="jpegFmt">ファイルストリーム</param>
        /// <param name="dpiValue">dpi値</param>
        /// <returns>実行状態</returns>
        private string changeEXIFDpi(JpegFormat jpegFmt, Int16 dpiValue)
        {
            string ret = "OK";

            byte[] buff2 = new byte[2];
            byte[] buff4 = new byte[4];

            int bigEndian = 1;  //1:Intel /2:Motorola  (e.g. 0x1234 -> I:0x34,0x12 ; M:0x12,0x34)

            //TIFF有効性判定
            jpegFmt.fs.Seek(jpegFmt.identifierOffset, SeekOrigin.Begin);
            jpegFmt.fs.Read(buff4, 0, 4);
            if(buff4[0] == 'I' && buff4[1] == 'I' && buff4[2] == 0x2A && buff4[3] == 0x00)  //Intel式
            {
                bigEndian = 1;
            }
            else if (buff4[0] == 'M' && buff4[1] == 'M' && buff4[2] == 0x00 && buff4[3] == 0x2A)  //Motorola式
            {
                bigEndian = 2;
            }
            else
            {
                ret = "EXIF/TIFFフォーマット異常";
                return ret;
            }//end if

            //次のIFDブロック位置取得
            jpegFmt.fs.Seek(4, SeekOrigin.Current);

            //IFD個数取得
            jpegFmt.fs.Read(buff2, 0, 2);

            //Motorola式時はビッチ配列を逆転換(C#内部はIntel式)
            if (bigEndian == 2)
                Array.Reverse(buff2);

            int itmCntOfIFD0 = BitConverter.ToInt16(buff2, 0);

            //X方向、Y方向の解像度情報を格納する
            //ResolutionInfo[] resolutionInfo = new ResolutionInfo[2];
            List<ResolutionInfo> resolutionInfoLst = new List<ResolutionInfo>(2);

            //X/Y解像度は常にIFD0ブロック内にあるので、IFD0ブロックのみ調べる。
            //IFD0ブロック内の各要素は12バイト固定
            //  マーク 　名称　　　　タイプ　　　　　　cnt　　備考
            //  0x011A  XResolution  unsigned rational  1       def: 1/72 inch 
            //  0x011B  YResolution  unsigned rational  1
            //  0x0128  ResolutionUnit unsigned short   1       XResloution/YResloutionの単位. 1:無し;2:inch;3:cm
            // DPIの設定なので、単位には常にinchを設定する。
            for(int iLoop = 0; iLoop < itmCntOfIFD0; iLoop++)
            {
                //IFD0内のタグ名取得
                jpegFmt.fs.Read(buff2, 0, 2);
                if (bigEndian == 1)
                    Array.Reverse(buff2);

                if((buff2[0] == 0x01 && buff2[1] == 0x1A) ||  //XResolution
                   (buff2[0] == 0x01 && buff2[1] == 0x1B))    //YResolution
                {
                    ResolutionInfo ri = new ResolutionInfo();

                    //type取得
                    jpegFmt.fs.Read(buff2, 0, 2);
                    if (bigEndian == 2)
                        Array.Reverse(buff2);

                    ri.type = BitConverter.ToInt16(buff2, 0);

                    //コンポーネント数取得
                    jpegFmt.fs.Read(buff4, 0, 4);
                    if (bigEndian == 2)
                        Array.Reverse(buff4);

                    ri.cnt = BitConverter.ToInt32(buff4, 0);

                    //解像度フォーマットチェック
                    if(ri.type != 5 || ri.cnt != 1)  //フォーマット異常
                    {
                        ret = "JPEG/EXIF DPI値のフォーマット異常[type:" + ri.type + " cnt:" + ri.cnt + "]";
                        return ret;
                    }//end if

                    //解像度記録位置取得
                    jpegFmt.fs.Read(buff4, 0, 4);
                    if (bigEndian == 2)
                        Array.Reverse(buff4);

                    ri.pos = BitConverter.ToInt32(buff4, 0);

                    resolutionInfoLst.Add(ri);
                }
                else if(buff2[0] == 0x01 && buff2[1] == 0x28)  //ResolutionUnit
                {
                    //type取得
                    jpegFmt.fs.Read(buff2, 0, 2);
                    if (bigEndian == 2)
                        Array.Reverse(buff2);

                    short type = BitConverter.ToInt16(buff2, 0);
                    /*
                    if(type == 5 || type == 8 || type == 12)  //これらは8バイトのタイプ
                    {
                        ret = "";
                        return ret;
                    }//end if
                    */

                    //コンポーネント数取得
                    jpegFmt.fs.Read(buff4, 0, 4);
                    if (bigEndian == 2)
                        Array.Reverse(buff4);

                    int cnt = BitConverter.ToInt32(buff4, 0);  //必ず1個　チェック要？

                    //ResolutionUnitの値は常にinch(2)にする
                    int resolutionUnit = 2;
                    byte[] resolutionUnitBuff = BitConverter.GetBytes(resolutionUnit);
                    if (bigEndian == 2)
                    {
                        //Array.Reverse(resolutionUnitBuff);
                        //type=3は2バイトなので、
                        resolutionUnitBuff = new byte[4]{ 0, 2, 0, 0 };
                    }//end if                        

                    //書き込む
                    jpegFmt.fs.Write(resolutionUnitBuff, 0, 4);
                }
                else
                {
                    //次のマークの先頭位置へ
                    jpegFmt.fs.Seek(10, SeekOrigin.Current);
                }//end if
            }//end for

            //解像度の情報がない場合
            if(resolutionInfoLst.Count == 0)
            {
                ret = "JPEG/EXIF 該当画像にDPI情報がありません。";
                return ret;
            }//end if
            
            //解像度変更処理

            //ForTest 解像度を取得してみる
            foreach(ResolutionInfo ri in resolutionInfoLst)
            {
                //解像度データの位置へ
                jpegFmt.fs.Seek(jpegFmt.identifierOffset + ri.pos, SeekOrigin.Begin);

                //DPIの小数点対応
                int dpiBunbo = 10000;
                int dpiBunshi = dpiBunbo * dpiValue;

                //dpi分子書き込み
                byte[] dpiBunshiBytes = BitConverter.GetBytes(dpiBunshi);
                if (bigEndian == 2) //Motorola式
                    Array.Reverse(dpiBunshiBytes);

                jpegFmt.fs.Write(dpiBunshiBytes, 0, 4);

                //dpi分母書き込み
                byte[] dpiBunboBytes = BitConverter.GetBytes(dpiBunbo);
                if (bigEndian == 2)
                    Array.Reverse(dpiBunboBytes);

                jpegFmt.fs.Write(dpiBunboBytes, 0, 4);

            }//end foreach

            return ret;
        }

        #region Drag & Drop機能関連
        /// <summary>
        /// ドラッグ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directroyBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// ドロップ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directroyBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] strs = (string[])e.Data.GetData(DataFormats.FileDrop);

            //マルチ指定に対応無し。常に最初１個目を取得する。
            string file = strs[0];

            if (Directory.Exists(file))
                directroyBox.Text = file;
            else
            {
                directroyBox.Text = Path.GetDirectoryName(file);
            }//end if
        }

        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            directroyBox_DragEnter(sender, e);
        }

        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            directroyBox_DragDrop(sender, e);
        }
        
        private void FrmMain_DragEnter(object sender, DragEventArgs e)
        {
            directroyBox_DragEnter(sender, e);
        }

        private void FrmMain_DragDrop(object sender, DragEventArgs e)
        {
            directroyBox_DragDrop(sender, e);
        }

        #endregion

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }//end class
}//end namespace
