﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
    public enum TranslationType
    {
        /// <summary>
        /// Application Translation... Not really. Used for updates.
        /// </summary>
        App = 0,

        /// <summary>
        /// Equipment translation list
        /// </summary>
        Equipment = 1,

        /// <summary>
        /// Map and enemy fleet translation lists
        /// </summary>
        Operations = 2,

        /// <summary>
        /// Quest translation list
        /// </summary>
        Quests = 3,

        /// <summary>
        /// Ship name translation list
        /// </summary>
        Ships = 4,

        /// <summary>
        /// Ship type translation list
        /// </summary>
        ShipTypes = 5,

        /// <summary>
        /// Operation map translations only
        /// </summary>
        OperationMaps = 6,

        /// <summary>
        /// Operation enemy fleet translations only
        /// </summary>
        OperationSortie = 7,

        /// <summary>
        /// Quest detail translations only
        /// </summary>
        QuestDetail = 8,

        /// <summary>
        /// Quest title translations only
        /// </summary>
        QuestTitle = 9
    }
}