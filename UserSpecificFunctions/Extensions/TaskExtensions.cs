using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSpecificFunctions.Extensions
{
    /// <summary>Provides extensions methods for the<see cref="Task"/> type.</summary>
    public static class TaskExtensions
    {
        public static Task LogExceptions(this Task task)
        {
            return task.ContinueWith(action => { var e = action.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
