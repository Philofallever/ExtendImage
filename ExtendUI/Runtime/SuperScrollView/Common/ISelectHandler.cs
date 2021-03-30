using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendUI.SuperScrollView
{
    public  interface ISelectHandler
    {
        bool Selected { get; }
        void SetSelectedStatus(bool selected, bool instant = false);
    }
}
