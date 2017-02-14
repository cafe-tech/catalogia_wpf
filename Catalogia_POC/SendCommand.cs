/*
    Catalog WPF 0.7
    Copyright © 2016-2017, Michael Francisco / FCS. All rights reserved.
  
    Catalog WPF is licensed under the terms of the GPLv2
    <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>.

    This program is free software; you can redistribute it and/or modify 
    it under the terms of the GNU General Public License as published 
    by the Free Software Foundation; version 2 of the License.

    This script is distributed in the hope that it will be useful, but 
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
    or FITNESS FOR A PARTICULAR BUSINESS MODEL. See the GNU General Public License 
    for more details. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catalogia_POC
{
    class SendCommand : ICommand
    {
        private Action<object> _action;
        public SendCommand(Action<object> action)
        {
            _action = action;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                // _action("Test action");
            }
        }

        #endregion
    }

}
