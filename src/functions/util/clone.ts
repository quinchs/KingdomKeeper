import { Guild } from "discord.js";
import { reportToDev } from "./errorReport"

/**
 * @description Copies the channels and roles from one guild to another.
 * @param guildOne The guild to copy.
 * @param guildTwo The guild to copy to.
 */
export async function clone(guildOne: Guild, guildTwo: Guild) {
    guildOne.roles.forEach(originalRole => {
        guildTwo.createRole({
            color: originalRole.color,
            hoist: originalRole.hoist,
            mentionable: originalRole.mentionable,
            name: originalRole.name,
            permissions: originalRole.permissions,
            position: originalRole.position
        }).catch(error => {
            console.error(error);
            reportToDev(guildOne, error);
        });
    })
}