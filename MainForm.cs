﻿using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ImageViewer
{
	/// <summary>
	/// Главное окно программы
	/// </summary>
	public partial class MainForm : Form
	{
		private Loader Loader;

		public MainForm()
		{
			InitializeComponent();

			Loader = CreateAsyncLoader();
			//Loader = CreateSyncLoader();

			// загрузка и инициализация дерева папок
			DirectoriesTree.Load();
			DirectoriesTree.AfterDirectorySelect += DirectoriesTree_AfterDirectorySelect;

			// добавление картинок в дереве
			ImageList treeImageList = new ImageList();
			treeImageList.Images.Add( "folder-opened", LoadEmbeddedImage( "ImageViewer.res.folder-opened.png" ) );
			treeImageList.Images.Add( "folder-closed", LoadEmbeddedImage( "ImageViewer.res.folder-closed.png" ) );
			DirectoriesTree.ImageList = treeImageList;
			DirectoriesTree.ImageKey = "folder-closed";
			DirectoriesTree.SelectedImageKey = "folder-opened";

			// инициализация окна просмотра содержимого папки
			Image defaultImageThumbnail = LoadEmbeddedImage( "ImageViewer.res.image.png" );
			DirectoryView.InitializeComponent( Loader, defaultImageThumbnail );
			DirectoryView.SelectedIndexChanged += DirectoryView_SelectedIndexChanged;
		}

		/// <summary>
		/// Создать экземпляр синхронного "загрузчика"
		/// </summary>
		/// <returns></returns>
		private Loader CreateSyncLoader()
		{
			return new SyncLoader( PictureBox );
		}

		/// <summary>
		/// Создать экезмпляр асинхронного "загрузчика"
		/// </summary>
		/// <returns></returns>
		private Loader CreateAsyncLoader()
		{
			AsyncLoader loader = new AsyncLoader( this );
			loader.ImageLoaded += Loader_ImageLoaded;
			return loader;
		}

		/// <summary>
		/// Получить изображение, внедрённое в программу как ресурс
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private Image LoadEmbeddedImage( string name )
		{
			return Image.FromStream( Assembly.GetExecutingAssembly().GetManifestResourceStream( name ) );
		}

		/// <summary>
		/// Обработчик пункта "Выход" в главном меню.
		/// Вызывает закрытие главного окна и завершение приложения.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitMenuItem_Click( object sender, System.EventArgs e )
		{
			Close();
		}

		/// <summary>
		/// Обработчик выбора папки в дереве папок.
		/// Отображает содержимое папки в соответствующем окне
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DirectoriesTree_AfterDirectorySelect( object sender, DirectoryTreeViewEventArgs args )
		{
			DirectoryView.LoadDirectory( args.DirectoryPath );

			// руками вызываем обработчик изменения выбранного элемента списка
			DirectoryView_SelectedIndexChanged( null, null );
		}

		/// <summary>
		/// Обработчик выбора файла в списке файлов.
		/// Использует загрузчик для загрузки изображения из файла.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DirectoryView_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( DirectoryView.SelectedItems.Count == 1 )
			{
				string filepath = ( string )DirectoryView.SelectedItems[0].Tag;
				Loader.LoadImage( filepath );
			}
			else
			{
				PictureBox.Image = null;
				Loader.CancelLoadImage();
			}
		}

		/// <summary>
		/// Обработчик загрузки полноразмерного изображения
		/// </summary>
		/// <param name="image"></param>
		private void Loader_ImageLoaded( Image image )
		{
			PictureBox.Image = image;
		}
	}
}
