﻿using Logging.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IClassLogger
	{
		ILogger Logger { get; }
	}
}
