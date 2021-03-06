﻿using GladNet.Common;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// <see cref="ResponseMessage"/>s are <see cref="NetworkMessage"/>s in response to <see cref="RequestMessage"/> from remote peers.
	/// It contains additional fields/properties compared to <see cref="NetworkMessage"/> that provide information on the response.
	/// </summary>
	[GladNetSerializationContract]
	public class ResponseMessage : NetworkMessage, IResponseMessage
	{
		/// <summary>
		/// Constructor for <see cref="ResponseMessage"/> that calls <see cref="NetworkMessage"/>.ctor
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> of the <see cref="NetworkMessage"/>.</param>
		public ResponseMessage(PacketPayload payload)
			: base(payload)
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload of " + this.GetType() + " cannot be null in construction.");
		}

		/// <summary>
		/// Protected instructor used for deep cloning the NetworkMessage.
		/// </summary>
		/// <param name="netSendablePayload">Shallow copy of the PacketPayload for copying.</param>
		protected ResponseMessage(NetSendable<PacketPayload> netSendablePayload)
			: base(netSendablePayload)
		{
			//Used for deep cloning
		}

		/// <summary>
		/// Dispatches the <see cref="ResponseMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameters are null.</exception>
		/// <param name="parameters">The <see cref="IMessageParameters"/> of the <see cref="ResponseMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters)
		{
			if(receiver == null)
				throw new ArgumentNullException("receiver", typeof(INetworkMessageReceiver).ToString() + " parameter is null in " + GetType().ToString());

			if(parameters == null)
				throw new ArgumentNullException("parameters", typeof(IMessageParameters).ToString() + " parameter is null in " + GetType().ToString());

			receiver.OnNetworkMessageReceive(this, parameters);
		}

		public override NetworkMessage DeepClone()
		{
			return new ResponseMessage(Payload.ShallowClone());
		}
	}
}
