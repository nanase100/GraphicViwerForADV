using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using System.Windows.Forms;

using garu.Util;


namespace GraphicViewer
{
    public class ImageSet
    {
        public Image mainImage			{ get; set; }
        public Image thmbnailImage		{ get; set; }

		public ImageSet()
		{
			mainImage		= null;
			thmbnailImage	= null;
		}
    }

	//-----------------------------------------------------------------------------------
	//
	//-----------------------------------------------------------------------------------
    public class ImageManager
    {

		public Susie m_susie;

        public Dictionary<string, ImageSet> m_imageDictionary{get;set;}

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        public ImageManager()
        {
            m_imageDictionary	= new Dictionary<string, ImageSet>();
			m_susie				= new Susie();
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        ~ImageManager()
        {
            foreach ( KeyValuePair<string,ImageSet> imgSet in m_imageDictionary )
            {
                imgSet.Value.mainImage.Dispose();
                imgSet.Value.thmbnailImage.Dispose();
            }
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        static bool dummy()
        {
            return false; // このメソッドの内容は何でもよい
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        public void SaveThumbnail(string path, string prefix = "")
        {
            foreach ( KeyValuePair<string, ImageSet> tmpImg in m_imageDictionary )
            {
                tmpImg.Value.thmbnailImage.Save(path + tmpImg.Key + ".png" );
            }
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
        public void LoadImage(HashSet<DataSet> listData, int thumbWidth = 80, int thumbHeight = 60, Dictionary<string, Rectangle> faceRectDictionary = null )
        {
            ImageSet tmpImg;
            FileStream fs;
            string fileNameOnly;

            foreach (DataSet tmpData in listData)
            {
                tmpImg = new ImageSet();

                using (fs = File.OpenRead(tmpData.m_fileName))
                {

					//ベース画像読み込み
                    //tmpImg.mainImage	= Image.FromStream(fs);
					tmpImg.mainImage	=	(Image)m_susie.GetPicture(tmpData.m_fileName);
                    fileNameOnly		= System.IO.Path.GetFileNameWithoutExtension(tmpData.m_fileName);

                    //サムネイル作成
                    try
                    {
                        //専用の個別顔グラ座標がある場合は先に処理
                        if (tmpData.m_cutRect.Size.Height == 0)
                        {
                            //顔グラ情報が存在する場合は少し違う処理でサムネイルをつくる
                            if (faceRectDictionary != null && faceRectDictionary.ContainsKey(tmpData.m_genre) == true)
                            {
                                Rectangle srcRect		= faceRectDictionary[tmpData.m_genre];
                                Rectangle destRect		= new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                                tmpImg.thmbnailImage	= new Bitmap(thumbWidth, thumbHeight);
                                Graphics.FromImage(tmpImg.thmbnailImage).DrawImage(tmpImg.mainImage, destRect, srcRect, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                // tmpImg.thmbnailImage = tmpImg.mainImage.GetThumbnailImage(thumbWidth, thumbHeight, new Image.GetThumbnailImageAbort(dummy), IntPtr.Zero);
                                Rectangle srcRect		= new System.Drawing.Rectangle(0, 0, tmpImg.mainImage.Width, tmpImg.mainImage.Height);
                                Rectangle destRect		= new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                                tmpImg.thmbnailImage	= new Bitmap(thumbWidth, thumbHeight);

                                Graphics.FromImage(tmpImg.thmbnailImage).DrawImage(tmpImg.mainImage, destRect, srcRect, GraphicsUnit.Pixel);
                            }
                        }
                        else
                        {
                            Rectangle srcRect		= tmpData.m_cutRect;
                            Rectangle destRect		= new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                            tmpImg.thmbnailImage	= new Bitmap(thumbWidth, thumbHeight);
                            Graphics.FromImage(tmpImg.thmbnailImage).DrawImage(tmpImg.mainImage, destRect, srcRect, GraphicsUnit.Pixel);
                        }

                        tmpImg.mainImage.Dispose();
                        m_imageDictionary.Add(fileNameOnly, tmpImg);
                    }
                    catch
                    {
                    }
                }
                fs.Dispose();
            }
        }

		//-----------------------------------------------------------------------------------
		//
		//-----------------------------------------------------------------------------------
		public void LoadImage(DataSet refData, int thumbWidth = 80, int thumbHeight = 60, Dictionary<string, Rectangle> faceRectDictionary = null)
		{
			ImageSet	tmpImg = new ImageSet();
			FileStream	fs;
			string		fileNameOnly;

            try
            {
                using (fs = File.OpenRead(refData.m_fileName))
                {

                    //ベース画像読み込み
                    //tmpImg.mainImage	= Image.FromStream(fs);
                    tmpImg.mainImage	= (Image)m_susie.GetPicture(refData.m_fileName);
                    fileNameOnly		= System.IO.Path.GetFileNameWithoutExtension(refData.m_fileName);

                    //サムネイル作成
                    try
                    {
                        Rectangle srcRect;
                        Rectangle destRect;

                        //専用の個別顔グラ座標がある場合は先に処理
                        if (refData.m_cutRect.Size.Height == 0)
                        {
                            //顔グラ情報が存在する場合は少し違う処理でサムネイルをつくる
                            if (faceRectDictionary != null && faceRectDictionary.ContainsKey(refData.m_genre) == true && faceRectDictionary[refData.m_genre].Width > 0)
                            {
                                srcRect = faceRectDictionary[refData.m_genre];
                                destRect = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                            }
                            else
                            {
                                srcRect = new System.Drawing.Rectangle(0, 0, tmpImg.mainImage.Width, tmpImg.mainImage.Height);
                                destRect = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                            }
                        }
                        else
                        {
                            srcRect = refData.m_cutRect;
                            destRect = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);


                            tmpImg.thmbnailImage = new Bitmap(thumbWidth, thumbHeight);
                            Graphics.FromImage(tmpImg.thmbnailImage).DrawImage(tmpImg.mainImage, destRect, srcRect, GraphicsUnit.Pixel);
                        }

                        tmpImg.thmbnailImage = new Bitmap(thumbWidth, thumbHeight);
                        Graphics.FromImage(tmpImg.thmbnailImage).DrawImage(tmpImg.mainImage, destRect, srcRect, GraphicsUnit.Pixel);
                        tmpImg.mainImage.Dispose();
                        m_imageDictionary.Add(fileNameOnly, tmpImg);

                    }
                    catch( System.Exception ex)
                    {
                    }

                }
            }
            catch (System.Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
		}


        


    }

}
