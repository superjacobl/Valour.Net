﻿using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

/*  Valour - A free and secure chat client
 *  Copyright (C) 2021 Vooper Media LLC
 *  This program is subject to the GNU Affero General Public license
 *  A copy of the license should be included - if not, see <http://www.gnu.org/licenses/>
 */

namespace Valour.Shared.Items.Messages;

public class PlanetMessageBase : MessageBase
{
    [JsonPropertyName("Planet_Id")]
    public ulong Planet_Id { get; set; }

    [NotMapped]
    [JsonInclude]
    [JsonPropertyName("ItemType")]
    public override ItemType ItemType => ItemType.PlanetMessage;
}

