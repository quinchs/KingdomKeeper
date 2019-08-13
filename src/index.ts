require('dotenv').config()
import * as Discord from "discord.js"
const client = new Discord.Client()

import * as handler from "./functions/handlers/messageHandler"

import * as chalk from "chalk";

client.login(process.env.TOKEN)

client.on("ready", ()=> {
    console.log(chalk.default.green("Succesfully connected!"));
    client.user.setPresence({
        afk: true,
        status: "dnd",
        game: {
            name: "TypeScript Testing Build",
            type: "WATCHING"
        }
    })
    let devServer = client.guilds.get("608080803728982036")

    if (devServer instanceof Discord.Guild) {
        let devServerLogChannel = devServer.channels.get("610306559402049566")
        if (devServerLogChannel instanceof Discord.TextChannel) {
            devServerLogChannel.send("[TYPESCRIPT] Successfully Connected").catch(err => {
                if (err) {
                    console.error(err);
                } else {
                    console.log("Message succesfully sent.")
                }
            })
        }
    }
})

client.on("message", message => {
    handler.messageHandler(message)
})