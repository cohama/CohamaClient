using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using TwitterApi;

namespace SlowTest
{
	/// <summary>
	/// UnitTest1 の概要の説明
	/// </summary>
	[TestClass]
	public class SlowOAuthTest
	{
		public SlowOAuthTest()
		{
			//
			// TODO: コンストラクター ロジックをここに追加します
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///現在のテストの実行についての情報および機能を
		///提供するテスト コンテキストを取得または設定します。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		//
		// テストを作成する際には、次の追加属性を使用できます:
		//
		// クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// 各テストを実行する前に、TestInitialize を使用してコードを実行してください
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// 各テストを実行した後に、TestCleanup を使用してコードを実行してください
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void HomeTimeLineTest()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load( @"D:\Documents\Visual Studio 2010\F#\TwitterClient\oauthtoken.xml" );
			XmlNode root = doc.SelectSingleNode( "/OAuthTokens" );

			string ckey = root["ConsumerKey"].InnerText;
			string csec = root["ConsumerSecret"].InnerText;
			string akey = root["AccessKey"].InnerText;
			string asec = root["AccessSecret"].InnerText;

			OAuthHandler oauth = new OAuthHandler( ckey, csec, akey, asec );
			Api api = new Api( oauth );

			XmlNodeList result = api.HomeTimeLine();
		}
	}
}
