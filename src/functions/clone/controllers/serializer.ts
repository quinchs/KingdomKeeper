import { Client, Guild, Collection, Snowflake, GuildMember, BanInfo, User, Role, CategoryChannel, TextChannel } from "discord.js";
import { Url } from "url";

// interface guildData {
//     name: Guild["name"],
//     region: Guild["region"],
//     icon: Guild["iconURL"],
//     verificationLevel: Guild["verificationLevel"],
//     afkTimeout: Guild["afkTimeout"],
//     explicitContentFilter: Guild["explicitContentFilter"],
//     splash: Guild["splashURL"]
// }

// interface roleInfo {
//     idOld: Role["id"],
//     name: Role["name"],
//     hexColor: Role["hexColor"],
//     hoist: Role["hoist"],
//     mentionable: Role["mentionable"],
//     position: Role["position"],
//     defaultRole: Boolean,
//     permissions: Role["permissions"]
// }

export class Serializer {
    static serializeOldGuild(client: Client, originalGuild: Guild, banCollection: Collection<Snowflake, (User|BanInfo)>, guildData: any, backupFile: string) {
        // Check
        let guildToCopy = client.guilds.get("488448958436474890");
        if (guildToCopy instanceof Guild) {
            // General
            guildData.general = this.getGeneralData(originalGuild);

            // Roles
            guildData.roles = guildData.roles = this.serializeRoles(guildToCopy);

            // Categories
            guildData.categories = this.serializeCategories(guildToCopy);

            // Text Channels


        } else {
            throw Error("The guild you are trying to copy is not currently available!");
        }
    }

    /**
     * Serializes the existing guild's data.
     * @param guild Original guild Object.
     */
    static getGeneralData(guild: Guild) {
        return {
            name: guild.name,
            region: guild.region,
            icon: guild.iconURL,
            verificationLevel: guild.verificationLevel,
            afkTimeout: guild.afkTimeout,
            explicitContentFilter: guild.explicitContentFilter,
            splash: guild.splashURL
        };
    }

    /**
     * Role serialization.
     * Managed roles will also be saved.
     * The role collection is sorted by their position
     * to ensure correct order afterwards.
     * @param guildToCopy Original guild object
     * @returns Serialized guild roles
     */
    static serializeRoles(guildToCopy: Guild) {
        let roleCol = guildToCopy.roles.sort((a, b) => b.position - a.position);
        let roles = roleCol.map((role) => {
            return {
                idOld: role.id,
                name: role.name,
                hexColor: role.hexColor,
                hoist: role.hoist,
                mentionable: role.mentionable,
                position: role.position,
                defaultRole: guildToCopy.defaultRole.id === role.id,
                permissions: role.permissions
            }
        });

        return roles;
    }

    /**
     * Category serialization
     * Only role permission overwrites will be saved.
     * The category collection is sorted by their position
     * to ensure correct order afterwards.
     * @param guildToCopy Original guild object
     * @returns Serialized category channels
     */
    static serializeCategories(guildToCopy: Guild) {
        let categoryCollection = guildToCopy.channels.filter(c => c.type === 'category');

        categoryCollection = categoryCollection.sort((a, b) => a.position - b.position);

        let categories = categoryCollection.map((category) => {
            let permOverwritesCollection = category.permissionOverwrites.filter(pOver => pOver.type === 'role');
            permOverwritesCollection = permOverwritesCollection.filter(pOver => guildToCopy.roles.has(pOver.id));
            let permOverwrites = permOverwritesCollection.map(pOver => {
                return {
                    id: pOver.id,
                    allowed: pOver.allow,
                    denied: pOver.deny,
                };
            });

            return {
                idOld: category.id,
                name: category.name,
                position: category.position,
                permOverwrites: permOverwrites,
            };
        });

        return categories;
    }

    static serializeTextChannels(guildToCopy: any) {
        let textChannelCollection: Collection<string, TextChannel> = guildToCopy.channels.filter(c => c instanceof TextChannel);
        textChannelCollection = textChannelCollection.sort((a, b) => a.position - b.position);
        let textChannel = textChannelCollection.map(tCh => {
            let permOverwritesCollection = tCh.permissionOverwrites.filter(pOver => pOver.type === 'role');
            permOverwritesCollection = permOverwritesCollection.filter(pOver => guildToCopy.roles.has(pOver.id));
            let permOverwrites = permOverwritesCollection.map(pOver => {
                return {
                    id: pOver.id,
                    allowed: pOver.allow,
                    denied: pOver.deny,
                };
            });

            return {
                id: tCh.id,
                name: tCh.name,
                topic: tCh.topic,
                nsfw: tCh.nsfw,
                isSystemChannel: guildToCopy.systemChannelID === tCh.id,
                position: tCh.position,
                parentCat: tCh.parentID
            };
        });

        return textChannel;
    }
}