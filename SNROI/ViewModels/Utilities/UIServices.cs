﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SNROI.ViewModels.Utilities
{
    //https://stackoverflow.com/questions/10118788/mvvm-wait-cursor-how-to-set-the-wait-cursor-during-invocation-of-a-command

    /// <summary>
    ///   Contains helper methods for UI, so far just one for showing a waitcursor
    /// </summary>
    public static class UIServices
    {
        /// <summary>
        ///   A value indicating whether the UI is currently busy
        /// </summary>
        private static bool IsBusy;

        /// <summary>
        /// Sets the busystate as busy.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        /// Sets the busystate to busy or not busy.
        /// </summary>
        /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
        private static void SetBusyState(bool busy)
        {
            if (busy == IsBusy)
                return;
            IsBusy = busy;
            Mouse.OverrideCursor = busy ? Cursors.Wait : null;

            if (IsBusy)
            {
                new DispatcherTimer(TimeSpan.FromSeconds(0), 
                    DispatcherPriority.ApplicationIdle, 
                    dispatcherTimer_Tick, 
                    System.Windows.Application.Current.Dispatcher);
            }
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!(sender is DispatcherTimer dispatcherTimer))
                return;
            SetBusyState(false);
            dispatcherTimer.Stop();
        }
    }
}