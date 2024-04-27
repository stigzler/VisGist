using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisGist.Commands
{
    internal class AsyncRelayCommand : AsyncCommandBase
    {
        private readonly Func<Task> callback;

        public AsyncRelayCommand(Func<Task> callback, Action<Exception> onException) : base(onException)
        {
            this.callback = callback;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await callback();
        }
    }
}
 