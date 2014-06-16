// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EventHandler.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public delegate void EntityEventHandler(IEntity sender, EntityEventArgs e);
	public delegate void InstanceEventHandler(IEntity sender);
	public delegate void InstancesEventHandler(ref List<IEntity> senders);
}
