﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageFactory
	{
		EventMessage CreateEventMessage(PacketPayload payload);

		ResponseMessage CreateResponseMessage(PacketPayload payload);

		RequestMessage CreateRequestMessage(PacketPayload payload);

		StatusMessage CreateStatusMessage(StatusChangePayload payload);
	}
}
