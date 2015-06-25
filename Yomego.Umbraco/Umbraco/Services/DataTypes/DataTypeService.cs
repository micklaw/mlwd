﻿using System.Collections.Generic;

namespace Yomego.Umbraco.Umbraco.Services.DataTypes
{
    public abstract class DataTypeService : BaseService
    {
        #region Abstract Methods

        public abstract Dictionary<string, string> GetPreValue(int id);

        #endregion
    }
}