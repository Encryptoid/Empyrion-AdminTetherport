# Empyrion-AdminTetherport

This is a Server based mod for Empyrion designed for use on the Anvil server, but could be used on other servers.

The purpose is to allow an Admin to teleport to a player to complete a ticket, and then return to where they were, easily.

The "entry point" for mod code is AdminTetherport.cs

The following commands are made available to people with Admin(Rank 9) permissions:

# !attp {Target Player Entity Id}
This command will create a tetherport record and then teleport the admin to the player who's entity Id is given.

# !uattp
This command will untether the admin and teleport them back to the saved location

# Initial Setup
Please read the Initial Setup section here: https://github.com/Encryptoid/zucchini-empyrion#initial-setup-notes
