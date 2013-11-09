using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
	public class DelegateDisposableToken : DisposableToken
	{
		private Action action;

		public DelegateDisposableToken(Action action)
		{
			Contract.Requires<ArgumentNullException>(action != null, "action");

			this.action = action;
		}

		public override void Dispose()
		{
			if (this.action != null)
			{
				this.action();
				this.action = null;
			}

			base.Dispose();
		}
	}
}