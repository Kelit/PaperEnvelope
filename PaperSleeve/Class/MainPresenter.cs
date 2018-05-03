using EnvelopePaper.Class;
using PaperSleeve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvelopePaper.Class
{

    class MainPresenter
    {
       private readonly IMainForm _view;

       private readonly IFIleManager _manager;

       public MainPresenter(IMainForm view, IFIleManager managere)
        {
            _view = view;
            _manager = managere;

            _view.SetSembolCount(0);

            _view.ContentChanged += _view_ContentChanged;
        }

        private void _view_ContentChanged(object sender, EventArgs e)
        {
            string content = _view.Content;

            int count = _manager.GetSymbolCount(content);
            _view.SetSembolCount(count);
        }
    }
}
