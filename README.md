# kiosk-server

Touch screen kiosk with remote control web server, using blazor and net6.

UNDER DEVELOPMENT, NOT RELEASED YET

TODO
- installation instructions
- user/password authentication for setup screen
- home assistant integration (??)

Display when no Kiosk URL is defined yet:

![touch screen](https://i.imgur.com/bTQtqSe.png)

When using the external setup URL (http://x.x.x.x:5000/setup, no password authentication YET!) you can enter the Kiosk URL and then either reboot or shutdown the raspberry pi.
The Kiosk URL is only shown after a reboot.

![touch screen](https://i.imgur.com/2aVKkbq.png)

![touch screen](https://i.imgur.com/s4vLMP6.png)

Test Equipment :

Touch Display 1920x515 (12.6 inch, IPS panel):

https://www.aliexpress.com/item/1005001966967133.html

Raspberry Pi Compute Module 4 (I only have the 8GB RAM / 16GB EMMC version)

The Waveshare CM4-NANO-B expansion board (also available on Amazon)

https://www.waveshare.com/wiki/CM4-NANO-B

The USB-C port is connected to the 5V power. (Also used to put the OS image onto the EMMC flash.)

The USB-A port is connected to the display. (To power it and also for the touch screen)

The HDMI port is connected to the display HDMI port.


