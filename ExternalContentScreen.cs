using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZHCN
{
	public class ExternalContentScreen : Game.ExternalContentScreen
	{
		public override void Update()
		{
			if (m_listDirty)
			{
				m_listDirty = false;
				UpdateList();
			}
			ExternalContentEntry externalContentEntry = null;
			if (m_directoryList.SelectedIndex.HasValue)
				externalContentEntry = m_directoryList.Items[m_directoryList.SelectedIndex.Value] as ExternalContentEntry;
			if (externalContentEntry != null)
			{
				m_actionButton.IsVisible = true;
				if (externalContentEntry.Type == ExternalContentType.Directory)
				{
					m_actionButton.Text = "进入";
					m_actionButton.IsEnabled = true;
					m_copyLinkButton.IsEnabled = false;
				}
				else
				{
					m_actionButton.Text = "下载";
					if (ExternalContentManager.IsEntryTypeDownloadSupported(ExternalContentManager.ExtensionToType(Storage.GetExtension(externalContentEntry.Path).ToLower())))
					{
						m_actionButton.IsEnabled = true;
						m_copyLinkButton.IsEnabled = true;
					}
					else
					{
						m_actionButton.IsEnabled = false;
						m_copyLinkButton.IsEnabled = false;
					}
				}
			}
			else
			{
				m_actionButton.IsVisible = false;
				m_copyLinkButton.IsVisible = false;
			}
			m_directoryLabel.Text = m_externalContentProvider.IsLoggedIn ? ("内容 " + m_path) : "未登陆";
			m_providerNameLabel.Text = m_externalContentProvider.DisplayName;
			m_upDirectoryButton.IsEnabled = m_externalContentProvider.IsLoggedIn && m_path != "/";
			m_loginLogoutButton.Text = m_externalContentProvider.IsLoggedIn ? "注销" : "登陆";
			m_loginLogoutButton.IsVisible = m_externalContentProvider.RequiresLogin;
			m_copyLinkButton.IsVisible = m_externalContentProvider.SupportsLinks;
			m_copyLinkButton.IsEnabled = externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type);
			if (m_changeProviderButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new SelectExternalContentProviderDialog("选择内容源", true, delegate (IExternalContentProvider provider)
				{
					m_externalContentProvider = provider;
					m_listDirty = true;
					SetPath(null);
				}));
			}
			if (m_upDirectoryButton.IsClicked)
			{
				string directoryName = Storage.GetDirectoryName(m_path);
				SetPath(directoryName);
			}
			if (m_actionButton.IsClicked && externalContentEntry != null)
				if (externalContentEntry.Type == ExternalContentType.Directory)
					SetPath(externalContentEntry.Path);
				else
					DownloadEntry(externalContentEntry);
			if (m_copyLinkButton.IsClicked && externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type))
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("创建链接", false);
				DialogsManager.ShowDialog(null, busyDialog);
				m_externalContentProvider.Link(externalContentEntry.Path, busyDialog.Progress, delegate (string link)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new ExternalContentLinkDialog(link));
				}, delegate (Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog("Error", error.Message, "OK", null, null));
				});
			}
			if (m_loginLogoutButton.IsClicked)
				if (m_externalContentProvider.IsLoggedIn)
				{
					m_externalContentProvider.Logout();
					SetPath(null);
					m_listDirty = true;
				}
				else
				{
					ExternalContentManager.ShowLoginUiIfNeeded(m_externalContentProvider, false, delegate
					{
						SetPath(null);
						m_listDirty = true;
					});
				}
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
				ScreensManager.SwitchScreen("Content");
		}

		public new void UpdateList()
		{
			m_directoryList.ClearItems();
			if (m_externalContentProvider != null && m_externalContentProvider.IsLoggedIn)
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("检索目录", false);
				DialogsManager.ShowDialog(null, busyDialog);
				m_externalContentProvider.List(m_path, busyDialog.Progress, delegate (ExternalContentEntry entry)
				{
					DialogsManager.HideDialog(busyDialog);
					var list = new List<ExternalContentEntry>((from e in entry.ChildEntries
															   where EntryFilter(e)
															   select e).Take(1000));
					m_directoryList.ClearItems();
					list.Sort(delegate (ExternalContentEntry e1, ExternalContentEntry e2)
					{
						if (e1.Type == ExternalContentType.Directory && e2.Type != ExternalContentType.Directory)
							return -1;
						if (e1.Type != ExternalContentType.Directory && e2.Type == ExternalContentType.Directory)
							return 1;
						return string.Compare(e1.Path, e2.Path);
					});
					foreach (ExternalContentEntry item in list)
					{
						m_directoryList.AddItem(item);
					}
				}, delegate (Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog("Error", error.Message, "OK", null, null));
				});
			}
		}

		public new void DownloadEntry(ExternalContentEntry entry)
		{
			CancellableBusyDialog busyDialog = new CancellableBusyDialog("下载中", false);
			DialogsManager.ShowDialog(null, busyDialog);
			m_externalContentProvider.Download(entry.Path, busyDialog.Progress, delegate (Stream stream)
			{
				busyDialog.LargeMessage = "导入中";
				ExternalContentManager.ImportExternalContent(stream, entry.Type, Storage.GetFileName(entry.Path), delegate
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
				}, delegate (Exception error)
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog("Error", error.Message, "OK", null, null));
				});
			}, delegate (Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog("Error", error.Message, "OK", null, null));
			});
		}
	}
}