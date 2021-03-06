﻿using GladNet.Common;
using Logging.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common.Tests
{
	[TestFixture(TestOf = typeof(ServerPeer))]
	public static class ServerPeerTests
	{
		[Test(Author = "Andrew Blakely", Description = "Should only be able to send requests.", TestOf = typeof(ServerPeer))]
		[TestCase(OperationType.Request, true)]
		[TestCase(OperationType.Event, false)]
		[TestCase(OperationType.Response, false)]
		public static void Test_CanSend_IsRequest(OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<INetworkMessageSender> sender = new Mock<INetworkMessageSender>();
			sender.Setup(x => x.CanSend(opType)).Returns(expectedResult); //set this up so it doesn't affect results

			Mock<ServerPeer> peer = new Mock<ServerPeer>(Mock.Of<ILogger>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}

		[Test(Author = "Andrew Blakely", TestOf = typeof(ServerPeer))]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Dependencies()
		{
			//Assert
			Assert.DoesNotThrow(() => { var r = new Mock<ServerPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>()).Object; } );
		}

		[Test(Author = "Andrew Blakely", Description = nameof(ServerPeer) + " should be listening for events and responses.", TestOf = typeof(ServerPeer))]
		public static void Test_Registered_EventMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ServerPeer> peer = new Mock<ServerPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), subService.Object);
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToEvents(It.IsAny<OnNetworkEventMessage>()), Times.Once());
		}


		[Test(Author = "Andrew Blakely", Description = nameof(ServerPeer) + " should be listening for events and responses.", TestOf = typeof(ServerPeer))]
		public static void Test_Registered_ResponseMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ServerPeer> peer = new Mock<ServerPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), subService.Object);
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToResponses(It.IsAny<OnNetworkResponseMessage>()), Times.Once());
		}

		//don't test that peer isn't subbed to request messages. They may listen for invalids

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ServerPeer))]
		public static void Test_SendResposne_Calls_Send_Request_On_NetSend_Service_With_Generic_Static_Params()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			TestPayload payload = new TestPayload();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ServerPeer> peer = new Mock<ServerPeer>(Mock.Of<ILogger>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
			peer.CallBase = true;

			//act
			peer.Object.SendRequest(payload);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Request, payload), Times.Once());
		}

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ServerPeer))]
		public static void Test_SendResposne_Calls_Send_Request_On_NetSend_Service()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			PacketPayload payload = Mock.Of<PacketPayload>();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ServerPeer> peer = new Mock<ServerPeer>(Mock.Of<ILogger>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
			peer.CallBase = true;

			//act
			peer.Object.SendRequest(payload, DeliveryMethod.ReliableOrdered, true, 5);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Request, payload, DeliveryMethod.ReliableOrdered, true, 5), Times.Once());
		}
	}
}
