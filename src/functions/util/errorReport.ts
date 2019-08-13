import { Message, RichEmbed, Guild, GuildMember, TextChannel } from "discord.js";

export function reportError(message: Message, error: Error) {
    let errEmbed = new RichEmbed();
    errEmbed.setColor('#E91E63')
        .setAuthor('It seems that an exception has occurred...', 'https://cdn2.iconfinder.com/data/icons/weby-interface-vol-1-1/512/s_warning-caution-exclamation-alert-attention-error-rounded-red-512.png')
        .setDescription('An exception went unhandled. You may not be able to do anything about this, but I\'ve notified the developer.')
        .addField('Error', `\`\`\`js\n${error.message}\`\`\``)
        .setFooter('This issue should be fixed in the near future. However, it may not even be broken!')
        .setTimestamp(Date.now());

    message.channel.send(errEmbed).catch(err => {
        console.error(err);
        message.channel.send("Well it looks like that there was a whoopsie in the actual error reporter method! Ooft")
    });

    let devEmbed = new RichEmbed();
    devEmbed.setColor('#E91E63')
        .setAuthor('An exception went unhandled when the bot was in operation!', 'https://cdn2.iconfinder.com/data/icons/weby-interface-vol-1-1/512/s_warning-caution-exclamation-alert-attention-error-rounded-red-512.png')
        .setDescription('The following error occurred with the following details.')
        .addField('Error', `\`\`\`js\n${error}\`\`\``)
        .addField(`Author`, `${message.author}`)
        .addField(`Command`, `\`\`\`puppet\n${message.content}\`\`\``)
        .addField(`Channel`, `${message.channel}`)

    let devUser = message.guild.members.get("310586056351154176")

    if (devUser instanceof GuildMember) {
        devUser.send(devEmbed).catch(err => {
            console.error(err);
            message.channel.send("Well it looks like that there was a whoopsie in the actual error reporter method! Ooft")
        });
    }

    message.guild.members.get("310586056351154176")
}

/**
 * @description Reports the error to the developer if it exists.
 * @param guild The guild that the error originates from.
 * @param error The error that was produced. 
 */
export function reportToDev(guild: Guild, error: Error) {
    let devEmbed = new RichEmbed();
    devEmbed.setColor('#E91E63')
        .setAuthor('An exception went unhandled when the bot was in operation!', 'https://cdn2.iconfinder.com/data/icons/weby-interface-vol-1-1/512/s_warning-caution-exclamation-alert-attention-error-rounded-red-512.png')
        .setDescription('The following error occurred with the following details.')
        .addField('Error', `\`\`\`js\n${error}\`\`\``)

    let devChannel = guild.channels.get("610306559402049566");

    if (devChannel instanceof TextChannel) {
        devChannel.send(devEmbed).catch(error => {
            console.error(error)
        });
    }

}