using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using TwitterApi;

namespace UnitTest
{
	/// <summary>
	/// UnitTest1 の概要の説明
	/// </summary>
	[TestClass]
	public class OAuthTest
	{
		public OAuthTest()
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
		public void UrlEncodingTest()
		{
			string source = "https://auth.login.yahoo.co.jp/oauth/v2/get_request_token";
			string expect = "https%3A%2F%2Fauth.login.yahoo.co.jp%2Foauth%2Fv2%2Fget_request_token";

			string urlEncoded = OAuthUtility.UrlEncode( source );

			Assert.AreEqual( expect, urlEncoded );
		}

		[TestMethod]
		public void OAuthSerializationTest()
		{
			string ckey = "this";
			string csec = "is";
			string akey = "a";
			string asec = "test";
			OAuthHandler oauth = new OAuthHandler( ckey, csec, akey, asec );
			oauth.SaveAs( @"serialized.txt" );

			OAuthHandler loaded = OAuthHandler.LoadFrom( @"serialized.txt" );

			Assert.AreEqual( ckey, loaded.ConsumerKey );
			Assert.AreEqual( csec, loaded.ConsumerSecret );
			Assert.AreEqual( akey, loaded.AccessKey );
			Assert.AreEqual( asec, loaded.AccessSecret );
		}
	}
}
