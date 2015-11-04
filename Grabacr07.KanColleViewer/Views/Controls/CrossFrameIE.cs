using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MSHTML;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class CrossFrameIE
	{
		// Returns null in case of failure.
		public static IHTMLDocument2 GetDocumentFromWindow(IHTMLWindow2 htmlWindow)
		{
			if (htmlWindow == null)
			{
				return null;
			}

			// First try the usual way to get the document.
			try
			{
				IHTMLDocument2 doc = htmlWindow.document;
				return doc;
			}
			catch (COMException comEx)
			{
				// I think COMException won't be ever fired but just to be sure ...
				if (comEx.ErrorCode != E_ACCESSDENIED)
				{
					return null;
				}
			}
			catch (System.UnauthorizedAccessException)
			{
			}
			catch
			{
				// Any other error.
				return null;
			}

			// At this point the error was E_ACCESSDENIED because the frame contains a document from another domain.
			// IE tries to prevent a cross frame scripting security issue.
			try
			{
				// Convert IHTMLWindow2 to IWebBrowser2 using IServiceProvider.
				IServiceProvider sp = (IServiceProvider)htmlWindow;

				// Use IServiceProvider.QueryService to get IWebBrowser2 object.
				Object brws = null;
				sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out brws);

				// Get the document from IWebBrowser2.
				SHDocVw.IWebBrowser2 browser = (SHDocVw.IWebBrowser2)(brws);

				return (IHTMLDocument2)browser.Document;
			}
			catch
			{
			}

			return null;
		}

		private const int E_ACCESSDENIED = unchecked((int)0x80070005L);
		private static Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
		private static Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E");
	}

	// This is the COM IServiceProvider interface, not System.IServiceProvider .Net interface!
	[ComImport(), ComVisible(true), Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IServiceProvider
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
	}
}
