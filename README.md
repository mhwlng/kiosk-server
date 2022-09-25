# kiosk-server

touch screen kiosk remote control web server using blazor and net6

UNDER DEVELOPMENT, NOT RELEASED YET

Testing on a raspberry pi compute module 4 (8GB RAM/16GB EMMC) connected to a 1920 x 515 wide 'bar-style' touch screen


TODO
- documentation
- user/password authentication for setup screen
- home assistant integration (??)



Display when no url is defined yet:

![touch screen](https://i.imgur.com/bTQtqSe.png)

when using the external setup URL (http://x.x.x.x:5000/setup, no password authentication YET!) you can enter the Kiosk URL and then either reboot or shutdown the raspberry pi.
The kiosk url is only used after a reboot.

![touch screen](https://i.imgur.com/2aVKkbq.png)
