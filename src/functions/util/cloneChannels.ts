import { Guild, ChannelData, ChannelCreationOverwrites } from "discord.js";
import { reportToDev } from "./errorReport";

/**
 * @description Clones all of the channels from one guild to another. 
 * @param guildOne The guild to copy the channels from.
 * @param guildTwo The guild to copy the channels to.
 */
export async function cloneChannels(guildOne: Guild, guildTwo: Guild) {
    guildOne.channels.forEach(chan => {
        let chanOptions: ChannelData = {
            name: chan.name,
            position: chan.position
        };

        if (chan.type === "voice") {
            chanOptions.type = "voice"
        } else if (chan.type === "text") {
            chanOptions.type = "text"
        } else if (chan.type === "category") {
            chanOptions.type = "category";
        } else {
            throw TypeError(`Type of ${chan.type} is not a valid type!`);
        };

        console.log(chanOptions)

        let chanPermissions: ChannelCreationOverwrites;
        
        guildTwo.createChannel(chan.name, chanOptions).catch(error => {
            console.error(error);
            reportToDev(guildTwo, error)
        });

        return "Sucessfully completed!"
    })
}