using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

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

			string urlEncoded = OAuth.UrlEncode( source );

			Assert.AreEqual( expect, urlEncoded );
		}

		/// <summary>
		///GetAccessToken のテスト
		///</summary>
		[TestMethod()]
		public void GetAccessTokenTest()
		{
			string consumerKey = string.Empty; // TODO: 適切な値に初期化してください
			string consumerSecret = string.Empty; // TODO: 適切な値に初期化してください
			Tuple<string, string> requestToken = null; // TODO: 適切な値に初期化してください
			string verifier = string.Empty; // TODO: 適切な値に初期化してください
			Tuple<string, string> expected = null; // TODO: 適切な値に初期化してください
			Tuple<string, string> actual;
			actual = OAuth.GetAccessToken( consumerKey, consumerSecret, requestToken, verifier );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///GetAuthorizationUrl のテスト
		///</summary>
		public void GetAuthorizationUrlTestHelper<a>()
		{
			string requestToken_0 = string.Empty; // TODO: 適切な値に初期化してください
			a requestToken_1 = default( a ); // TODO: 適切な値に初期化してください
			string expected = string.Empty; // TODO: 適切な値に初期化してください
			string actual;
			actual = OAuth.GetAuthorizationUrl<a>( requestToken_0, requestToken_1 );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		[TestMethod()]
		public void GetAuthorizationUrlTest()
		{
			GetAuthorizationUrlTestHelper<GenericParameterHelper>();
		}

		/// <summary>
		///GetOAuthWebResponse のテスト
		///</summary>
		[TestMethod()]
		public void GetOAuthWebResponseTest()
		{
			string url = string.Empty; // TODO: 適切な値に初期化してください
			string headerString = string.Empty; // TODO: 適切な値に初期化してください
			string httpMethod = string.Empty; // TODO: 適切な値に初期化してください
			WebResponse expected = null; // TODO: 適切な値に初期化してください
			WebResponse actual;
			actual = OAuth.GetOAuthWebResponse( url, headerString, httpMethod );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///GetRequestToken のテスト
		///</summary>
		[TestMethod()]
		public void GetRequestTokenTest()
		{
			string consumerKey = string.Empty; // TODO: 適切な値に初期化してください
			string consumerSecret = string.Empty; // TODO: 適切な値に初期化してください
			Tuple<string, string> expected = null; // TODO: 適切な値に初期化してください
			Tuple<string, string> actual;
			actual = OAuth.GetRequestToken( consumerKey, consumerSecret );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///UrlEncode のテスト
		///</summary>
		[TestMethod()]
		public void UrlEncodeTest()
		{
			string url = string.Empty; // TODO: 適切な値に初期化してください
			string expected = string.Empty; // TODO: 適切な値に初期化してください
			string actual;
			actual = OAuth.UrlEncode( url );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///tokenFromString のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem( "TwitterApi.dll" )]
		public void tokenFromStringTest()
		{
			string token = string.Empty; // TODO: 適切な値に初期化してください
			Tuple<string, string> expected = null; // TODO: 適切な値に初期化してください
			Tuple<string, string> actual;
			actual = OAuth_Accessor.tokenFromString( token );
			Assert.AreEqual( expected, actual );
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///AccessTokenUrl のテスト
		///</summary>
		[TestMethod()]
		public void AccessTokenUrlTest()
		{
			string actual;
			actual = OAuth.AccessTokenUrl;
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///AuthorizeUrl のテスト
		///</summary>
		[TestMethod()]
		public void AuthorizeUrlTest()
		{
			string actual;
			actual = OAuth.AuthorizeUrl;
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///OAuthRoot のテスト
		///</summary>
		[TestMethod()]
		public void OAuthRootTest()
		{
			string actual;
			actual = OAuth.OAuthRoot;
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}

		/// <summary>
		///RequestTokenUrl のテスト
		///</summary>
		[TestMethod()]
		public void RequestTokenUrlTest()
		{
			string actual;
			actual = OAuth.RequestTokenUrl;
			Assert.Inconclusive( "このテストメソッドの正確性を確認します。" );
		}
	}
}
