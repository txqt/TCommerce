using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IPictureServiceCommon
    {
        Task<Picture?> GetPictureByIdAsync(int pictureId);
    }
}
