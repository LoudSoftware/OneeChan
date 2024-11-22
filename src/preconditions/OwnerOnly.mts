import { Precondition } from "@sapphire/framework";
import { Message } from "discord.js";
import { CommandInteraction } from "discord.js";

const OWNER = "147410761021390850";

export class OwnerOnlyPrecondition extends Precondition {
    #message = "This command can only be used by the owner.";

    public override messageRun(message: Message) {
        return this.checkOwner(message.author.id);
    }

    public override chatInputRun(interaction: CommandInteraction) {
        return this.checkOwner(interaction.user.id);
    }

    private checkOwner(userId: string) {
        return OWNER === userId
            ? this.ok()
            : this.error({ message: this.#message });
    }
}

declare module "@sapphire/framework" {
    interface Preconditions {
        OwnerOnly: never;
    }
}
