using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using garu.Util;

namespace GraphicViewer
{
    public partial class Form2 : Form
    {
        private string		m_fileName;
        private int			m_x;
        private int			m_y;
		private Susie		m_susie;
		private Rectangle	m_thumbnailRect;
		private bool		m_isEditThumbnailRect;
		private	Image		m_img;

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        public Form2( string fileName, int x, int y, Rectangle thumbnailRect, bool isEditThumbnailRect  )
        {
            InitializeComponent();

            m_fileName				= fileName;
            m_x						= x;
            m_y						= y;

            this.Text				= fileName;
			m_thumbnailRect			= thumbnailRect;
			m_isEditThumbnailRect	= isEditThumbnailRect;
			m_susie					= new Susie();
			m_img = null;

			
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Left)
			{
				this.Close();
				m_img.Dispose();
			}
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        private void Form2_Load(object sender, EventArgs e)
        {

            //画像ファイルを読み込んで、Imageオブジェクトとして取得する
            //Image img		= Image.FromFile(m_fileName);
			m_img		= m_susie.GetPicture(m_fileName);


            //描画先とするImageオブジェクトを作成する
            Bitmap canvas	= new Bitmap(m_img.Width, m_img.Height);
            pictureBox1.Size= new Size(m_img.Width, m_img.Height);
            this.ClientSize = new Size(m_img.Width, m_img.Height);

            int x = m_x - m_img.Width  / 2;
            int y = m_y - m_img.Height / 2;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x + m_img.Width  > System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width ) x = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width  - m_img.Width;
            if (y + m_img.Height > System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) y = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - m_img.Height;

			//画像が大きく画面外にウインドウが出てしまう場合の対処
			if (y < 0) y = 0; 

            this.SetDesktopLocation(x,y);

           // this.SetDesktopLocation(0, 0);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);

            //画像をcanvasの座標(20, 10)の位置に描画する
            g.DrawImage(m_img, 0, 0, m_img.Width, m_img.Height);

			if (m_isEditThumbnailRect)
			{
				g.DrawRectangle(System.Drawing.Pens.Aqua, 0, 0, 100, 100);
			}

            

            //Graphicsオブジェクトのリソースを解放する
            g.Dispose();

            //PictureBox1に表示する
            pictureBox1.Image = canvas;

            this.TopMost = true;
        }

		private void Form2_Paint(object sender, PaintEventArgs e)
		{
			if (m_isEditThumbnailRect && m_img != null )
			{
				Bitmap canvas = new Bitmap(m_img.Width, m_img.Height);
				Graphics g = Graphics.FromImage(canvas);

				//画像をcanvasの座標(20, 10)の位置に描画する
				g.DrawImage(m_img, 0, 0, m_img.Width, m_img.Height);

				if (m_isEditThumbnailRect)
				{
					g.DrawRectangle(System.Drawing.Pens.Aqua, 0, 0, 100, 100);
				}
			}
		}

		private void pictureBox1_Click( object sender, EventArgs e )
		{

		}
	}
}
