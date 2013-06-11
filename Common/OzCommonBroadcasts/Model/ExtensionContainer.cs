using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPSSDK;
using OzCommon.Utils;

namespace OzCommonBroadcasts.Model
{
    public class ExtensionContainer : IExtensionContainer
    {
        private IAPIExtension _extension;

        public IAPIExtension GetExtension()
        {
            return _extension;
        }

        public void SetExtension(IAPIExtension extension)
        {
            _extension = extension;
        }
    }
}
