using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPSSDK;

namespace OzCommonBroadcasts.Model
{
    public interface IExtensionContainer
    {
        IAPIExtension GetExtension();

        void SetExtension(IAPIExtension extension);
    }
}
