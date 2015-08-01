﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Extensions
{
	/// <summary>
	/// Provides extension methods for the <see cref="GladNet.Common.NetworkMessage"/> Type as well as its various enums.
	/// </summary>
	public static class NetworkMessageExt
	{
		/// <summary>
		/// This translates <see cref="NetDeliveryMethod"/> to GladNet <see cref="PacketPayload.DeliveryMethod"/>
		/// Information related to this translation can be found here https://code.google.com/p/lidgren-network-gen3/wiki/Basics
		/// </summary>
		/// <param name="lidgrenDeliveryMethod">The instance to be used for translation.</param>
		/// <exception cref="ArgumentException">Throws an exception if the <see cref="NetDeliveryMethod"/> is undefined.</exception>
		/// <returns>The equivalent <see cref="NetworkMessageExt.DeliveryMethod"/> for the given <see cref="NetDeliveryMethod"/></returns>
		public static NetworkMessage.DeliveryMethod LidgrenDeliveryTypeToGladNetType(this NetDeliveryMethod lidgrenDeliveryMethod)
		{
			switch (lidgrenDeliveryMethod)
			{
				//Following comments are based on https://code.google.com/p/lidgren-network-gen3/wiki/Basics and https://github.com/lidgren/lidgren-network-gen3/blob/master/Lidgren.Network/NetDeliveryMethod.cs

				//This delivery method guarantees that messages will always be received in the exact order they were sent.
				case NetDeliveryMethod.ReliableOrdered:
					return NetworkMessage.DeliveryMethod.ReliableOrdered;

				//This delivery method is similar to UnreliableSequenced; except that it guarantees that SOME messages will be received - if you only send one message - it will be received. If you sent two messages quickly, and they get reordered in transit, only the newest message will be received 
				//- but at least ONE of them will be received.
				case NetDeliveryMethod.ReliableSequenced:
					return NetworkMessage.DeliveryMethod.ReliableDiscardStale;

				//This delivery method ensures that every message sent will be eventually received. 
				//It does not however guarantee what order they will be received in; late messages may be delivered before newer ones.
				case NetDeliveryMethod.ReliableUnordered:
					return NetworkMessage.DeliveryMethod.ReliableUnordered;

				//his is just UDP. Messages can be lost or received more than once. Messages may not be received in the same order as they were sent.
				case NetDeliveryMethod.Unreliable:
					return NetworkMessage.DeliveryMethod.UnreliableAcceptDuplicate;

				//Using this delivery method messages can still be lost; but you're protected against duplicated messages. 
				//If a message arrives late; that is, if a message sent after this one has already been received - it will be dropped. 
				//This means you will never receive "older" data than what you already have received.
				case NetDeliveryMethod.UnreliableSequenced:
					return NetworkMessage.DeliveryMethod.UnreliableDiscardStale;
				
				//This could occur if for some reason a value is passed that is an invalid delivery method.
				//It is unlikely to occur.
				default:
					throw new ArgumentException("LidgrenDeliveryMethod invalid in " + typeof(NetworkMessageExt) + " unable to translate to GladNet type. Invalid code was: "
						+ lidgrenDeliveryMethod.ToString(), "lidgrenDeliveryMethod");
			}
		}
	}
}