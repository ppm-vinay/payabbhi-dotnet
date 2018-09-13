using System.Collections.Generic;
using NUnit.Framework;
using Payabbhi;
using Payabbhi.Error;

namespace UnitTesting.Payabbhi.Tests
{
	[TestFixture]
	public class TestUtility
	{
		const string ACCESSID = "access_id";
		const string SECRETKEY = "secret_key";
		Client client;

		public void Init(string accessID, string secretKey)
		{
			client = new Client(accessID, secretKey);
		}

		[Test]
		public void TestVerifyPaymentSignature()
		{
			Init(ACCESSID, SECRETKEY);
			IDictionary<string, string> options = new Dictionary<string, string>();
			options.Add("payment_signature", "e70360e32919311d31cbc9b558ea9024715b816ce64293ffc992459a94daac42");
			options.Add("order_id", "dummy_order_id");
			options.Add("payment_id", "dummy_payment_id");
			Assert.IsTrue(client.Utility.VerifyPaymentSignature(options));
		}

		[Test]
		public void TestVerifyWrongPaymentSignature()
		{
			Init(ACCESSID, SECRETKEY);
			IDictionary<string, string> options = new Dictionary<string, string>();
			options.Add("payment_signature", "wrong_signature");
			options.Add("order_id", "dummy_order_id");
			options.Add("payment_id", "dummy_payment_id");
			var ex = Assert.Throws<SignatureVerificationError>(() => client.Utility.VerifyPaymentSignature(options));
			Assert.That(ex.Message, Is.EqualTo("message: Invalid signature\n"));
			Assert.That(ex.Description, Is.EqualTo(Constants.Messages.InvalidSignatureError));
			Assert.That(ex.Field, Is.EqualTo(null));
		}

		[Test]
		public void TestVerifyWebhookSignature()
		{
			Init(ACCESSID, SECRETKEY);
			string filepath = "dummy_event.json";
			string payload = Helper.GetJsonString(filepath).Replace(System.Environment.NewLine, string.Empty);
			string secret = "skw_live_jHNxKsDqJusco5hA";
			string actualSignature = "t=1536577756, v1=d973a60a35f503bcac0844d34f5ec9ead8f2db3873ddd9b874da8fec799fa462";
			Assert.IsTrue(client.Utility.VerifyWebhookSignature(payload,actualSignature,secret,999999999));
		}

		[Test]
		public void TestVerifyWrongWebhookSignature()
		{
			Init(ACCESSID, SECRETKEY);
			string filepath = "dummy_event.json";
			string payload = Helper.GetJsonString(filepath).Replace(System.Environment.NewLine, string.Empty);
			string secret = "skw_live_jHNxKsDqJusco5hA";
			string actualSignature = "t=1536577756, v1=random";
			var ex = Assert.Throws<SignatureVerificationError>(() => client.Utility.VerifyWebhookSignature(payload,actualSignature,secret));
			Assert.That(ex.Message, Is.EqualTo("message: Invalid signature\n"));
			Assert.That(ex.Description, Is.EqualTo(Constants.Messages.InvalidSignatureError));
			Assert.That(ex.Field, Is.EqualTo(null));
		}

		[Test]
		public void TestInvalidArgumentError()
		{
			Init(ACCESSID, SECRETKEY);
			IDictionary<string, string> options = new Dictionary<string, string>();
			options.Add("order_id", "dummy_order_id");
			options.Add("payment_id", "dummy_payment_id");
			var ex = Assert.Throws<InvalidRequestError>(() => client.Utility.VerifyPaymentSignature(options));
			Assert.That(ex.Message, Is.EqualTo("message: The arguments provided are invalid.\n"));
			Assert.That(ex.Description, Is.EqualTo(Constants.Messages.InvalidArgumentError));
			Assert.That(ex.Field, Is.EqualTo(null));
		}
	}
}
