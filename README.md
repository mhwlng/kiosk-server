# kiosk-server

Touch screen kiosk with remote control web server, using blazor and net6.

UNDER DEVELOPMENT

TODO
- user/password authentication for setup screen
- home assistant integration (??)

Display when no Kiosk URL is defined yet:

![touch screen](https://i.imgur.com/bTQtqSe.png)

When using the external setup URL (http://x.x.x.x:5000/setup, no password authentication YET!) you can enter the Kiosk URL and then either reboot or shutdown the raspberry pi.
The Kiosk URL is only shown after a reboot.

![touch screen](https://i.imgur.com/2aVKkbq.png)

![touch screen](https://i.imgur.com/s4vLMP6.png)

## Test Environment

Touch Display 1920x515 (12.6 inch, IPS panel):

https://www.aliexpress.com/item/1005001966967133.html

Raspberry Pi Compute Module 4 (I only have the 8GB RAM / 16GB EMMC version)

The Waveshare CM4-NANO-B expansion board (also available on Amazon)

https://www.waveshare.com/wiki/CM4-NANO-B

The USB-C port is connected to the 5V power. (Also used to put the OS image onto the EMMC flash.)

The USB-A port is connected to the display. (To power it and also for the touch screen)

The HDMI port is connected to the display HDMI port.


# Installation Instructions

I mainly used this document as a guideline: (note that for the CM4 it's not exactly the same)

https://gist.github.com/fjctp/210f4e870f913416b8d0e17fd36153c2


## Install bootloader on CM4

https://www.raspberrypi.com/documentation/computers/compute-module.html

Set boot switch to on, plug in usb-c Download rpiboot_setup.exe, run rpiboot.exe Also see waveshare CM4-NANO-B wiki page.

https://github.com/raspberrypi/usbboot/raw/master/win32/rpiboot_setup.exe

- Install raspberry pi os lite 32 bit 
- Set up wifi
- Set up ssh
- Set up an account (The instructions and various configuration files assume pi/raspberry Adjust as required.)

After connecting via ssh :

```
sudo raspi-config

Select desktop/cli console auto login

sudo apt-get install -y --no-install-recommends xserver-xorg x11-xserver-utils xinit openbox

sudo apt-get install -y --no-install-recommends chromium-browser
```

## Edit /boot/config.txt

```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3 before !!!!!
hdmi_group=2
hdmi_mode=87
hdmi_cvt=1920 515 60 6 0 0 0

[cm4]
#otg_mode=1
dtoverlay=dwc2,dr_mode=host
```

## Edit /etc/xdg/openbox/autostart

```
xset s off
xset s noblank
xset -dpms

setxkbmap -option terminate:ctrl_alt_bksp

sed -i 's/"exited_cleanly":false/"exited_cleanly":true/' ~/.config/chromium/'Local State'
sed -i 's/"exited_cleanly":false/"exited_cleanly":true/; s/"exit_type":"[^"]\+"/"exit_type":"Normal"/' ~/.config/chromium/Default/Preferences

chromium-browser --noerrdialogs --disable-infobars --kiosk 'http://127.0.0.1:5000'
```

## Edit ~/.profile

```
[[ -z $DISPLAY && $XDG_VTNR -eq 1 ]] && startx -- -nocursor
```

## Copy all the web server application files to /home/pi/kiosk-server

Make application runnable using 
```
sudo chmod +x /home/pi/kiosk-server/kiosk-server
```

Install the application as a service (Adjust kiosk-server.service if user or directory is different) :

```
sudo systemctl stop kiosk-server

sudo cp /home/pi/kiosk-server/kiosk-server.service /etc/systemd/system/kiosk-server.service

sudo systemctl daemon-reload

sudo systemctl enable kiosk-server

sudo systemctl start kiosk-server

check if service is running ok :

sudo systemctl status kiosk-server

sudo journalctl -u kiosk-server 
```

## Visual Studio Publish Action

When using 'Publish' -> copies all files to /home/pi/kiosk-server using pscp, that comes with putty

Adjust paths, ip address and password in .csproj file as required :

```
Target Name="PiCopy" AfterTargets="AfterPublish">
   <Exec Command="pscp -r -pw raspberry C:\dotnet\projects\kiosk-server\kiosk-server\bin\Release\net6.0\publish\ pi@192.168.2.36:/home/pi/kiosk-server/" />
</Target>
```
