﻿using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class MiningGuildSurveyor : ModifiedTIELnFighter
        {
            public MiningGuildSurveyor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Mining Guild Surveyor",
                    "",
                    Faction.Scum,
                    2,
                    3,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bf/da/bfda499f-603a-41c7-b2ee-50ffeeddb384/swz23_mining-guild-surveyor.png";
            }
        }
    }
}