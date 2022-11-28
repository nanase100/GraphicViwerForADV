using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace GraphicViewer
{
	public class folderStr
	{
		public List<string>		m_folderName			= new List<string>();
	}

	public class HistoryData {
		public string			fileName	= "";
		public string			summary		= "";
		public string			filePath	= "";
		public string			genre		= "";
		public folderStr		folderPath	= new folderStr();
		public string			copyStr		= "";

		

		public HistoryData( string a_fileName, string a_summary, string a_filePath, string a_genre, folderStr a_folderPath, string a_copyStr )
		{
			fileName	= a_fileName;
			summary		= a_summary;
			filePath	= a_filePath;
			genre		= a_genre;
			folderPath	= a_folderPath;
			copyStr		= a_copyStr;
		}
	}
	public partial class Form3 : Form
	{
		public DataManger		m_rDataManager;
		public ImageManager		m_imgManager;
		private Bitmap			m_bitmapSurface;

		public List<HistoryData>	m_history = new List<HistoryData>();

		public Form1			m_rForm1;

		public int				m_HCount = 0;
		public int				m_VCount = 0;

		public int				m_HSize = 0;
		public int				m_VSize = 0;

		public string			m_optionString		= "";
        public string			m_optionString2		= "";
        public string           m_optionString3     = "";
        public string			m_optionString4		= "";
		public string			m_copyString		= "";

		private int				m_historyCount = 9;

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(String sClassName, String sWindowText);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool PostMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern Int32 SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32")]
		static extern short GetAsyncKeyState(Keys vKey);

		public Form3( Form1 parent, DataManger dataManager )
		{
			this.Owner		= parent;
			this.m_rForm1	= parent;
			m_rDataManager	= dataManager;

			InitializeComponent();
			comboBox1.SelectedIndex = 0;
		}

		private void Form3_Load(object sender, EventArgs e)
		{
			this.pictureBox1.MouseWheel     += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);

			ReCreateSurface();
			UpdateScrollBar();

			DoPaint();

			this.Left		= m_rForm1.m_dataManager.m_historyLeft;
			this.Top		= m_rForm1.m_dataManager.m_historyTop;
			this.Width		= m_rForm1.m_dataManager.m_historyWidth;
			this.Height		= m_rForm1.m_dataManager.m_historyHeight;

			comboBox1.SelectedIndex = m_rForm1.m_dataManager.m_historyPosType;

			this.MinimumSize = new Size( m_rDataManager.m_thumbnailWidth+150, m_rDataManager.m_thumbnailHeight+1);
		}

		public void InterlockMove()
		{
			if( comboBox1.SelectedIndex == 0 ) return;

			switch( comboBox1.SelectedIndex )
			{
				case 1:	this.Left	= m_rForm1.Bounds.Right;				this.Top	= m_rForm1.Bounds.Top;					break;
				case 2:	this.Left	= m_rForm1.Bounds.Left - this.Width;	this.Top	= m_rForm1.Bounds.Top ;					break;
				case 3:	this.Left	= m_rForm1.Bounds.Left;					this.Top	= m_rForm1.Bounds.Top - this.Height;	break;
				case 4:	this.Left	= m_rForm1.Bounds.Left;					this.Top	= m_rForm1.Bounds.Bottom;				break;	
			}
			
		}

		public void AddFav( string filename, string summary, string path, string genre, List<string> folder, string copyStr )
		{
			if( checkBox1.Checked == false ) return;

			//古いのでかぶりがあれば削除
			int alreadyID = indexOf_histroyFileName( filename );
			if( alreadyID != -1 && checkBox4.Checked == true )
			{
				m_history.RemoveAt( alreadyID );
			}

			folderStr tmpFolder = new folderStr();
			tmpFolder.m_folderName = folder;

			//追加
			m_history.Add( new HistoryData(filename, summary, path, genre,  tmpFolder, copyStr) );


			if( m_history.Count > m_historyCount )
			{
				m_history.RemoveAt( 0 );
			}

			DoPaint();
			UpdateScrollBar();
		}

		private void Form3_Click(object sender, EventArgs e)
		{

		}




		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
		private void DoPaint()
		{
			Graphics g = Graphics.FromImage(m_bitmapSurface);

			g.FillRectangle( Brushes.Black, 0, 0, pictureBox1.Width, pictureBox1.Height );

			string			fileNameNoExe;
			int				posX			= 0;
			int				posY			= -vScrollBar1.Value;
			int				summaryPosY		= 0;
			Font			tmpFont			= new Font("Meiryo", m_rDataManager.m_summaryFontSize );
			Font			tmpFontBig		= new Font("Meiryo", (float)(m_rDataManager.m_summaryFontSize*1.4) );
			StringFormat	sf				= new StringFormat();
			Brush			b				= new SolidBrush(Color.FromArgb(128, Color.Black));
			int				loopYCount		= 0;

			string			tmpStr;

			int				loopCount = m_history.Count-1;

			for( int i = loopCount; i >= 0; i-- )
			{
				fileNameNoExe = System.IO.Path.GetFileNameWithoutExtension( m_history[i].fileName );
				//g.DrawImage(m_imgManager.m_imageDictionary[fileNameNoExe].thmbnailImage, posX, posY, m_rDataManager.m_thumbnailWidth, m_rDataManager.m_thumbnailHeight);
				g.DrawImage(m_imgManager.m_imageDictionary[m_history[i].fileName ].thmbnailImage, posX, posY, m_rDataManager.m_thumbnailWidth, m_rDataManager.m_thumbnailHeight);

				//----サムネイル説明分の表示
				if (checkBox3.Checked == true)
				{	
					//説明文
					tmpStr				= m_history[i].summary;
					SizeF stringSize	= g.MeasureString( tmpStr, tmpFont, m_rForm1.m_thumbnailWidth, sf);
					summaryPosY			= posY + m_rForm1.m_thumbnailHeight - (int)stringSize.Height;

					g.FillRectangle(b, posX, summaryPosY, stringSize.Width, stringSize.Height);
					g.DrawString( tmpStr, tmpFont, Brushes.White, new Rectangle( posX, summaryPosY, m_rForm1.m_thumbnailWidth, m_rForm1.m_thumbnailHeight), sf);

					//ジヤンル名
					tmpStr				= m_history[i].genre;
					stringSize			= g.MeasureString( tmpStr, tmpFont, m_rForm1.m_thumbnailWidth, sf);
					int genrePosY		= summaryPosY - (int)stringSize.Height;

					g.FillRectangle(b, posX, genrePosY, stringSize.Width, stringSize.Height);
					g.DrawString( tmpStr, tmpFont, Brushes.Aqua, new Rectangle( posX, genrePosY, m_rForm1.m_thumbnailWidth, m_rForm1.m_thumbnailHeight), sf);

					//ショートカット番号
					tmpStr				= (m_history.Count - i).ToString();
					stringSize			= g.MeasureString( tmpStr, tmpFontBig, m_rForm1.m_thumbnailWidth, sf);
					
					g.FillRectangle(b, posX, posY, stringSize.Width, stringSize.Height);
					g.DrawString( tmpStr, tmpFontBig, Brushes.White, new Rectangle( posX, posY, m_rForm1.m_thumbnailWidth, m_rForm1.m_thumbnailHeight), sf);
				}

				posX = (posX + m_rForm1.m_thumbnailWidth);

				//サムネイル改行
				if ( posX + m_rForm1.m_thumbnailWidth > pictureBox1.Width )
				{
					loopYCount++;
					posX			= 0;
					posY			+= m_rForm1.m_thumbnailHeight;
				}
			}

			tmpFont.Dispose();
			b.Dispose();
			g.Dispose();
			
			pictureBox1.Image = m_bitmapSurface;
			pictureBox1.Invalidate();

		}

		//-----------------------------------------------------------------------------------
		//サーフェイスの再作成
		//-----------------------------------------------------------------------------------
		private void ReCreateSurface()
		{
			m_bitmapSurface = new Bitmap(pictureBox1.Width, pictureBox1.Height);	

			m_HCount = pictureBox1.Width  / m_rForm1.m_thumbnailWidth;
			m_VCount = pictureBox1.Height / m_rForm1.m_thumbnailHeight;

			m_HSize	= m_HCount * m_rForm1.m_thumbnailWidth;
			m_VSize = m_VCount * m_rForm1.m_thumbnailHeight;
		}

		private void Form3_ResizeEnd(object sender, EventArgs e)
		{
			
		}

		private void Form3_Resize(object sender, EventArgs e)
		{
			ReCreateSurface();
			UpdateScrollBar();
			DoPaint();
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			ClickAction(e);
		}

		public void ClickAction( MouseEventArgs e)
		{
			//if( m_HSize < e.X || m_VSize < e.Y  ) return;


			int panelNo = e.X /  m_rForm1.m_thumbnailWidth + (((e.Y+vScrollBar1.Value) /  m_rForm1.m_thumbnailHeight) * m_HCount);			

			panelNo = m_history.Count - panelNo-1;

			if( panelNo < 0 || m_history.Count <= panelNo ) return;

			if (e.Button == MouseButtons.Right  )
			{
				DoCopyString( panelNo );
			} 
			
			if( e.Button == MouseButtons.Left )
			{
				bool		isEdit		= false;
				Rectangle	rect		= new Rectangle(0,0,100,100);
				Form2		tmpForm2	= new Form2(m_history[panelNo].filePath, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, rect, isEdit);

				tmpForm2.ShowDialog();
				tmpForm2.Dispose();
			}

			if( e.Button == MouseButtons.Middle )
			{
				if (Control.ModifierKeys  == Keys.Control)
				{
					m_history.RemoveAt( panelNo );
				
					DoPaint();
				}else
				{
					DoCopyString( panelNo, false, true );
				}
			}
		}

		public void DoCopyString( int panelNo, bool flipNo = false, bool direct = false )
		{
			if( panelNo < 0 || m_history.Count <= panelNo ) return;

			if( flipNo == true ) panelNo = m_history.Count - panelNo - 1;

			string			fileName	= m_history[panelNo].fileName;
			string			copyStr		= m_history[panelNo].copyStr;
			folderStr		folderName	= m_history[panelNo].folderPath;

			string copyString = m_copyString;

			//フォルダ階層をテキスト置き換え
			copyString = copyString.Replace("(d0)", folderName.m_folderName[0]);
				
			if (folderName.m_folderName.Count >= 2) copyString = copyString.Replace("(d1)", folderName.m_folderName[1]);
			else copyString = copyString.Replace("(d1)", "");
				
			if (folderName.m_folderName.Count >= 3) copyString = copyString.Replace("(d2)", folderName.m_folderName[2]);
			else copyString = copyString.Replace("(d2)", "");
				
			if (folderName.m_folderName.Count >= 4) copyString = copyString.Replace("(d3)", folderName.m_folderName[3]);
			else copyString = copyString.Replace("(d3)", "");
				
			if (folderName.m_folderName.Count >= 5) copyString = copyString.Replace("(d4)", folderName.m_folderName[4]);
			else copyString = copyString.Replace("(d4)", "");
				
			if (folderName.m_folderName.Count >= 6) copyString = copyString.Replace("(d5)", folderName.m_folderName[5]);
			else copyString = copyString.Replace("(d5)", "");

			if( direct ) copyString = "(1)";

			//ワイルドカード的な指定の置き換え実行
			if( copyStr != "" )		copyString = copyString.Replace("(1)", copyStr);
			else					copyString = copyString.Replace("(1)", fileName);

			copyString = copyString.Replace("(2)", m_optionString);
			copyString = copyString.Replace("(3)", m_optionString2);
            copyString = copyString.Replace("(4)", m_optionString3);
            copyString = copyString.Replace("(5)", m_optionString4);

            if (copyString == "" ) return;

			Clipboard.SetText(copyString);
			this.Text = "【履歴ウインドウ】 - " + copyString;

			m_rForm1.SendHidemaru();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			this.TopMost = checkBox2.Checked;
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			DoPaint();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			InterlockMove();
		}

		private void Form3_KeyDown(object sender, KeyEventArgs e)
		{
			KeyShortCutProc( e.KeyCode );
		}

		private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			KeyShortCutProc( e.KeyCode );
		}

		private void KeyShortCutProc( System.Windows.Forms.Keys keyCode )
		{
			switch( keyCode )
			{
				case Keys.NumPad1:	
				case Keys.NumPad2:
				case Keys.NumPad3:
				case Keys.NumPad4:
				case Keys.NumPad5:
				case Keys.NumPad6:
				case Keys.NumPad7:
				case Keys.NumPad8:
				case Keys.NumPad9:	 
					DoCopyString(keyCode-Keys.NumPad1, true );
					break;
			}
		}

		private void Form3_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_rForm1.m_dataManager.m_historyLeft	    = this.Left;
			m_rForm1.m_dataManager.m_historyTop			= this.Top;
			m_rForm1.m_dataManager.m_historyWidth	    = this.Width;
			m_rForm1.m_dataManager.m_historyHeight	    = this.Height;
			m_rForm1.m_dataManager.m_historyPosType		= comboBox1.SelectedIndex;
		}

		private void UpdateScrollBar()
		{
			
			int		colCount	= pictureBox1.Width / m_rDataManager.m_thumbnailWidth;
			int		rowCount	= m_history.Count / colCount;
			
			if(	m_history.Count % colCount != 0 ) rowCount++;

			int		picHeight	= rowCount * m_rDataManager.m_thumbnailHeight;

			vScrollBar1.Minimum = 0;
			vScrollBar1.Value	= 0;

			if( picHeight >= pictureBox1.Height )
			{
				vScrollBar1.Enabled = true;
				vScrollBar1.Maximum = picHeight;
				vScrollBar1.LargeChange	= pictureBox1.Height;
				
			}else{
				vScrollBar1.Enabled = false;
				vScrollBar1.Maximum = pictureBox1.Height;
			}

		}

		private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			DoPaint();
		}

		//-----------------------------------------------------------------------------------
		// 画像上でのマウスホイールイベント  
		//-----------------------------------------------------------------------------------
		private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)  
		{
			int newScrollValue = vScrollBar1.Value - ((e.Delta * SystemInformation.MouseWheelScrollLines / 120) * 12);


			// スクロール量（方向）の表示    
			if (vScrollBar1.Enabled == false) return;

			if (newScrollValue >= vScrollBar1.Maximum-vScrollBar1.LargeChange)  newScrollValue = vScrollBar1.Maximum - vScrollBar1.LargeChange;
			if (newScrollValue <= vScrollBar1.Minimum)                          newScrollValue = vScrollBar1.Minimum;

			vScrollBar1.Value = newScrollValue;

			DoPaint();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			m_history.Clear();

			DoPaint();
		}


		public int indexOf_histroyFileName( string searchVal )
		{
			int ret = -1;
			int i = 0;
			foreach( var tmp in m_history )
			{
				if( tmp.fileName == searchVal )
				{
					ret = i;
					break;
				}
				i++;
			}

			return ret;
		}

	}
}
