// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PermissionAnalyzer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kuick
{
	internal class PermissionAnalyzer
	{
		private PermissionAnalyzer(List<IAccessControl> acl) { }

		private List<IAccessControl> Acl { get; set; }

		public static PermissionAnalyzer Build(List<IAccessControl> acl)
		{
			return new PermissionAnalyzer(acl);
		}

		// Verify
		public bool Verify(IResource resource, PermissionAction action)
		{
			// negative
			bool negativePermitted = false;
			Parallel.ForEach<IAccessControl>(
				Acl,
				delegate(IAccessControl ac, ParallelLoopState state) {
					if(ac.Permit) { return; }

					if(InScope(resource, ac)) {
						if(ac.Permission > 0) {
							if(!Permission.Set(ac.Permission).Verify(action)) {
								negativePermitted = true;
								state.Stop();
							}
						}
					}
				}
			);
			if(negativePermitted) { return false; }

			// positive
			bool positivePermitted = false;
			Parallel.ForEach<IAccessControl>(
				Acl,
				delegate(IAccessControl ac, ParallelLoopState state) {
					if(!ac.Permit) { return; }

					if(InScope(resource, ac)) {
						if(ac.Permission > 0) {
							if(Permission.Set(ac.Permission).Verify(action)) {
								positivePermitted = true;
								state.Stop();
							}
						}
					}
				}
			);
			if(positivePermitted) { return true; }

			// default
			return false;
		}

		private bool InScope(IResource resource, IAccessControl ac)
		{
			return resource.Equals(ac.Resource)
				? ac.Permission > 0
				: false;
		}
	}
}
