﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class StatusMessage : NetworkMessage, IStatusMessage
	{
		public NetStatus Status
		{
			get
			{
				//Constructor enforces the Type. Casting is safe.
				return (Payload.Data as StatusChangePayload).Status;
			}
		}

		public StatusMessage(StatusChangePayload payload)
			: base(payload)
		{
			//This ensures a non-malicious sender sends a StatusChangePayload but it must still be checked.
		}

		/// <summary>
		/// Protected instructor used for deep cloning the NetworkMessage.
		/// </summary>
		/// <param name="netSendablePayload">Shallow copy of the PacketPayload for copying.</param>
		protected StatusMessage(NetSendable<PacketPayload> netSendablePayload)
			: base(netSendablePayload)
		{
			//Used for deep cloning
		}

		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters = null)
		{
			//We don't need IMessageParameters for this type of message.
			if (receiver == null)
				throw new ArgumentNullException("receiver", "INetworkMessageReciever must not be null.");

			receiver.OnStatusChanged(Status);
		}

		public override NetworkMessage DeepClone()
		{
			//Shallow clone of the payload is valid because internally it's represented as a non-instance specific mutable PacketPayload and/or
			//a non-mutating byte[] that is used for serialization/encryption and new instances of the byte[] are created if they're mutated.
			//This helps us out in preformance when we want to serialize once and encrypt for many.
			return new StatusMessage(Payload.ShallowClone());
		}
	}
}
