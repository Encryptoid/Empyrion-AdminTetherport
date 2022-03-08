# Empyrion-AdminTetherport

This is a Server based mod for Empyrion designed for use on the Anvil server, but could be used on other servers.

The purpose is to allow an Admin to teleport to a player to complete a ticket, and then return to where they were, easily.

The "entry point" for mod code is AdminTetherport.cs

The following commands are made available to people with Admin(Rank 9) permissions:

# !attp
This command will pop up a UI window that will list all online players. You can click on one of these players to create a tether at your location. You will then be teleported to the player.

Example with my player repeated for testing purposes:
![image](https://user-images.githubusercontent.com/89423557/154297229-f54eabca-4e35-4847-9865-38c6da53cd73.png)

# !uattp
This command will untether you and teleport you back to the saved location(tether location).

# !retrieve
This will launch a UI displaying active players simialr to the image for !attp. Clicking on one of these names will teleport the player to you. This will not create a tether, this is mostly just for retrieving players who are stuck. If the player is in a ship, it should also teleport the ship, allowing you to free people who's ships are stuck.

# Initial Setup
Please read the Initial Setup section here before running on your server: https://github.com/Encryptoid/zucchini-empyrion#initial-setup-notes
