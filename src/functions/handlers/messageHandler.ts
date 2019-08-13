import { Message, Guild } from "discord.js";
import { clone } from "../util/clone";
import * as chalk from "chalk";
import * as prompter from "discordjs-prompter";

let prefix = "$.";

export function messageHandler(message: Message) {
    if (message.content.startsWith(prefix)) {
        let command = message.content.slice(prefix.length);

        if (command === "clone") {
            let devServer = message.client.guilds.get("608080803728982036");

            prompter.default
                .reaction(message.channel, {
                    question: "Are you sure that you want to continue?",
                    userId: message.author.id,
                    timeout: 5000
                })
                .then(response => {
                    if (!response) message.reply("You took too long!");
                    if (response == "yes") {
                        message.reply("Starting clone...");
                        if (devServer instanceof Guild) {
                            clone(message.guild, devServer);
                        }
                    }
                    if (response == "no") {
                        message.reply("Alright, cancelling...");
                    }
                });
        }
    }
}
