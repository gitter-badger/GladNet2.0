﻿using GladNet.Common;
using GladNet.Server.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common.UnitTests
{
	[TestFixture]
	public static class ClientPeerTests
	{
		[Test]
		[TestCase(NetworkMessage.OperationType.Request, false)]
		[TestCase(NetworkMessage.OperationType.Event, true)]
		[TestCase(NetworkMessage.OperationType.Response, true)]
		public static void Test_CanSend_IsRequest(NetworkMessage.OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<INetEngine>());
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}
	}
}
