//
// Tests for System.Web.UI.WebControls.MasterPageTest.cs
//
// Author:
//	Yoni Klein (yonik@mainsoft.com)
//
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


#if NET_2_0

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using MyWebControl = System.Web.UI.WebControls;
using System.Collections;
using MonoTests.SystemWeb.Framework;
using MonoTests.stand_alone.WebHarness;
using System.Threading;


namespace MonoTests.System.Web.UI.WebControls
{
	public class PokerMasterPage : MasterPage
	{
		public PokerMasterPage ()
		{
			TrackViewState ();
		}
		public StateBag StateBag
		{
			get { return base.ViewState; }
		}
		public new IDictionary ContentTemplates ()
		{
			return base.ContentTemplates;
		}
		public new void AddContentTemplate (string templateName, ITemplate template)
		{
			base.AddContentTemplate (templateName, template);
		}
		public string MasterMethod ()
		{
			return "FromMasterMethod";
		}
	}

	[TestFixture]
	public class MasterPageTest
	{
		[TestFixtureSetUp]
		public void CopyTestResources ()
		{
#if DOT_NET
			WebTest.CopyResource (GetType (), "MonoTests.System.Web.UI.WebControls.Resources.MasterTypeTest1.aspx", "MasterTypeTest1.aspx");
			WebTest.CopyResource (GetType (), "MonoTests.System.Web.UI.WebControls.Resources.MasterTypeTest2.aspx", "MasterTypeTest2.aspx");
			WebTest.CopyResource (GetType (), "MonoTests.System.Web.UI.WebControls.Resources.MyDerived.master", "MyDerived.master");
			WebTest.CopyResource (GetType (), "MonoTests.System.Web.UI.WebControls.Resources.MyPageWithDerivedMaster.aspx", "MyPageWithDerivedMaster.aspx");
#else
			WebTest.CopyResource (GetType (), "MasterTypeTest1.aspx", "MasterTypeTest1.aspx");
			WebTest.CopyResource (GetType (), "MasterTypeTest2.aspx", "MasterTypeTest2.aspx");
			WebTest.CopyResource (GetType (), "MyDerived.master", "MyDerived.master");
			WebTest.CopyResource (GetType (), "MyPageWithDerivedMaster.aspx", "MyPageWithDerivedMaster.aspx");
#endif
		}

		[SetUp]
		public void SetupTestCase ()
		{
			Thread.Sleep (100);
		}

		[Test]
		public void MasterPage_DefaultProperties ()
		{
			PokerMasterPage pmp = new PokerMasterPage ();
			Assert.AreEqual (null, pmp.Master, "Master Property");
			Assert.AreEqual (null, pmp.MasterPageFile, "MasterPageFile Property");
		}

		[Test]
		[Category ("NotWorking")]
		public void MasterPage_DefaultPropertiesNotWorking ()
		{
			PokerMasterPage pmp = new PokerMasterPage ();
			IDictionary i = pmp.ContentTemplates ();
			Assert.AreEqual (null, i, "ContentTemplates");
		}

		[Test]
		[Category ("NunitWeb")]
		public void MasterPage_Render ()
		{
			Render_Helper (StandardUrl.PAGE_WITH_MASTER);
		}


		[Test]
		[Category ("NunitWeb")]
		public void MasterPageDerived_Render ()
		{
			Render_Helper (StandardUrl.PAGE_WITH_DERIVED_MASTER);
		}

		// Bug #325114
		[Test]
		[Category ("NunitWeb")]
		[ExpectedException (typeof(HttpException))]
		public void MasterPage_ContentPlaceHolder_Not_Found ()
		{
			Render_Helper (StandardUrl.PAGE_WITH_MASTER_INVALID_PLACE_HOLDER);
		}
		
		public void Render_Helper(string url)
		{
			WebTest t = new WebTest (PageInvoker.CreateOnLoad (_RenderDefault));
			t.Request.Url = url;
			string PageRenderHtml = t.Run ();
			
			
			if (PageRenderHtml.IndexOf ("Page main text") < 0) {
			        Assert.Fail ("Master#2");
			}
			
			Assert.AreEqual (-1, PageRenderHtml.IndexOf ("Master main text"), "Master#3");
			

			if (PageRenderHtml.IndexOf ("Page dynamic text") < 0) {
				Assert.Fail ("Master#5");
			}

			if (PageRenderHtml.IndexOf ("My master page footer") < 0) {
			        Assert.Fail ("Master#6, result: "+PageRenderHtml);
			}

			if (PageRenderHtml.IndexOf ("Master page content text") < 0) {
				Assert.Fail ("Master#7");
			}

			if (url == StandardUrl.PAGE_WITH_DERIVED_MASTER) {
				if (PageRenderHtml.IndexOf ("Derived header text") < 0) {
					Assert.Fail ("Master#8");
				}

				if (PageRenderHtml.IndexOf ("Derived master page text ") < 0) {
					Assert.Fail ("Master#9");
				}

				if (PageRenderHtml.IndexOf ("Master header text") < 0) {
					Assert.Fail ("Master#10");
				}
			}
			else {
				Assert.AreEqual (-1, PageRenderHtml.IndexOf ("Master header text"), "Master#1");
				Assert.AreEqual (-1, PageRenderHtml.IndexOf ("Master dynamic text"), "Master#4");
			}
		}

		[Test]
		[Category ("NunitWeb")]
		[Category ("NotWorking")]
		public void MasterType_VirtualPath ()
		{
			WebTest t = new WebTest ("MasterTypeTest1.aspx");
			string PageRenderHtml = t.Run ();
			if (PageRenderHtml.IndexOf ("MasterTypeMethod") < 0)
				Assert.Fail ("MasterType VirtualPath test failed");
		}

		[Test]
		[Category ("NunitWeb")]
		public void MasterType_TypeName ()
		{
			WebTest t = new WebTest ("MasterTypeTest2.aspx");
			string PageRenderHtml = t.Run ();
			if (PageRenderHtml.IndexOf ("FromMasterMethod") < 0)
				Assert.Fail ("MasterType TypeName test failed");
		}

		public static void _RenderDefault (Page p)
		{
			p.Form.Controls.Add(new LiteralControl("Page dynamic text"));
		}

		[Test]
	 	[ExpectedException (typeof(HttpException))]
		public void MasterPage_AddContentTemplate ()
		{
			PokerMasterPage pmp = new PokerMasterPage();
			ITemplate it = null;
			pmp.AddContentTemplate ("myTemplate", it);
			pmp.AddContentTemplate ("myTemplate", it);
		}
		
		[TestFixtureTearDown]
		public void TearDown ()
		{
			WebTest.Unload ();
		}
	}
}
#endif
