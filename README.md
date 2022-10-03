# kiosk-server

Touch screen kiosk with remote control web server, using blazor and net6.

UNDER DEVELOPMENT

TODO
- user/password authentication for setup screen

Display when no Kiosk URL is defined yet:

![touch screen](https://i.imgur.com/bTQtqSe.png)

When using the external setup URL (http://x.x.x.x:5000/setup, no password authentication YET!) you can enter one of more Kiosk URLs and then either reboot or shutdown the raspberry pi.

The Kiosk URL is only shown after a reboot.

If there is only one Kiosk URL, then the software will redirect to that URL at startup.

If there is more than one Kiosk URL defined, then the software will redirect to an internal page (http://x.x.x.x:5000/kiosk) that has a tab bar at the top and an iframe filling the rest of the screen. 

The contents of the iframe is changed to the Kiosk URL, after pressing the tab button with the name of the Kiosk URL.

If you see a blank iframe with an error like 'www.msn.com refused to connect.' :
That means that the remote web server does not allow rendering inside an iframe. (via the X-Frame-Options http response header)

You won't have this problem, if you define just one Kiosk URL.

![touch screen](https://i.imgur.com/Wzp5kqm.png)

![touch screen](https://i.imgur.com/cXrHx23.png)

## Test Environment

Touch Display 1920x480 (8.8 inch, IPS panel, default orientation is portrait):

https://www.aliexpress.com/item/1005003014364673.html

![touch screen](https://i.imgur.com/QWs2S9S.jpg)


Touch Display 1920x515 (12.6 inch, IPS panel):

https://www.aliexpress.com/item/1005001966967133.html

![touch screen](https://i.imgur.com/s4vLMP6.png)


Touch display 3840x1100 (14 inch, IPS panel, uses usb-c connector for power and touch screen):

https://www.aliexpress.com/item/1005003332731770.html

![touch screen](https://i.imgur.com/MjmCNvf.jpg)



Raspberry Pi Compute Module 4 (I only have the 8GB RAM / 16GB EMMC version)

The Waveshare CM4-NANO-B expansion board (also available on Amazon)

https://www.waveshare.com/wiki/CM4-NANO-B

The USB-C port is connected to the 5V power. (Also used to put the OS image onto the EMMC flash.)

The USB-A port is connected to the display. (To power it and also for the touch screen)

The HDMI port is connected to the display HDMI port.

Pressing the pushbutton (GPIO21) shuts down the CM4

# Installation Instructions

I mainly used this document as a guideline: (note that for the CM4 it's not exactly the same)

https://gist.github.com/fjctp/210f4e870f913416b8d0e17fd36153c2


## Install bootloader on CM4

https://www.raspberrypi.com/documentation/computers/compute-module.html

Set boot switch to on, plug in usb-c 

Download rpiboot_setup.exe, run rpiboot.exe 

Also see waveshare CM4-NANO-B wiki page.

https://github.com/raspberrypi/usbboot/raw/master/win32/rpiboot_setup.exe

- Install raspberry pi os lite 32 bit 
- Set up wifi
- Set up ssh
- Set up an account (The instructions and various configuration files assume pi/raspberry Adjust as required.)

After connecting via ssh :
```
sudo apt-get update

sudo apt-get upgrade

sudo raspi-config

Select desktop/cli console auto login

sudo apt-get install -y --no-install-recommends xserver-xorg x11-xserver-utils xinit openbox

sudo apt-get install -y --no-install-recommends chromium-browser
```

## Edit /boot/config.txt

For Touch Display 1920x480 (portrait orientation, default) :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

max_framebuffer_height=1920
hdmi_timings=480 1 48 32 80 1920 0 3 10 56 0 0 0 60 0 75840000 3
hdmi_group=2
hdmi_mode=87

[cm4]
#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21
```

For Touch Display 1920x480 (landscape orientation, rotate 90&deg;) also add:
```
display_hdmi_rotate=1
```

For landscape orientation: the touchscreen also needs to be rotated 90&deg; :

Edit /usr/share/X11/xorg.conf.d/40-libinput.conf

Add the TransformationMatrix option to the existing touchscreen InputClass:
```
Section "InputClass"
        Identifier "libinput touchscreen catchall"
        MatchIsTouchscreen "on"
        Option "TransformationMatrix" "0 1 0 -1 0 1 0 0 1"
        MatchDevicePath "/dev/input/event*"
        Driver "libinput"
EndSection
```

For Touch Display 1920x515 :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

hdmi_group=2
hdmi_mode=87
hdmi_cvt=1920 515 60 6 0 0 0

[cm4]
#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21
```

For Touch Display 3840x1100 :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

hdmi_enable_4kp60=1
hdmi_group=2
hdmi_mode=87
hdmi_cvt=3840 1100 60

[cm4]
#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21
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

For the 3840x1100 screen, you can increase the zoom level of chromium using --force-device-scale-factor on the command line :
```
chromium-browser --noerrdialogs --disable-infobars --force-device-scale-factor=1.5 --kiosk 'http://127.0.0.1:5000'
```

## Edit ~/.profile

```
[[ -z $DISPLAY && $XDG_VTNR -eq 1 ]] && startx -- -nocursor
```

## Web Server

Copy all the web server application files and subdirectories to ~/kiosk-server

Make application runnable using :
```
sudo chmod +x ~/kiosk-server/kiosk-server
```

Install the application as a service (Adjust kiosk-server.service if user or directory is different) :
```
sudo systemctl stop kiosk-server

sudo cp ~/kiosk-server/kiosk-server.service /etc/systemd/system/kiosk-server.service

sudo systemctl daemon-reload

sudo systemctl enable kiosk-server

sudo systemctl start kiosk-server
```

Check if service is running ok :
```
sudo systemctl status kiosk-server

or

sudo journalctl -u kiosk-server 
```

## Visual Studio Publish Action

When using 'Publish' -> Visual Studio automatically synchronises all files to ~/kiosk-server using WinSCP.

Adjust paths, ip address, user and password in .csproj file as required :
```
<Target Name="PiCopy" AfterTargets="AfterPublish">
   <Exec Command="&quot;C:\Program Files (x86)\WinSCP\WinSCP.com&quot; /command &quot;open sftp://pi:raspberry@192.168.2.36/&quot; &quot;synchronize remote C:\dotnet\projects\kiosk-server\kiosk-server\bin\Release\net6.0\publish /home/pi/kiosk-server/&quot; &quot;exit&quot;" />
</Target>
```

Alternatively, you could also copy all the files using pscp, that comes with putty:

```
Target Name="PiCopy" AfterTargets="AfterPublish">
   <Exec Command="pscp -r -pw raspberry C:\dotnet\projects\kiosk-server\kiosk-server\bin\Release\net6.0\publish\ pi@192.168.2.36:/home/pi/kiosk-server/" />
</Target>
```

First stop the web server, before updating the files :
```
sudo systemctl stop kiosk-server
```

## Home Assistant Dashboards

There is no on-screen keyboard, so an auto login mechanism is required:

I added a new user 'Kiosk'

The user id can be found on the details pop-up:
![home assistant](https://i.imgur.com/MzeJlGT.png)

Then, I added the IP address of the kiosk as trusted network and the Kiosk user as a trusted user to configuration.yaml:
```
# Allow login without password from local network
homeassistant:
  auth_providers:
    - type: trusted_networks
      trusted_networks:
        - 192.168.2.36/32
      trusted_users:
        192.168.2.36:
          - dacfc03879144b31b57104cc00f6a1a2 ## specific user for kiosk
      allow_bypass_login: true
    - type: homeassistant
```

You can use the normal home assistant navigation, to switch between dashboards. 

Or you can also add each dashboard URL separately to the Kiosk URL List:

![touch screen](https://i.imgur.com/cXrHx23.png)

![home assistant](https://i.imgur.com/xDXkFYL.jpg)

I installed the 'Kiosk Mode' HACS frontend repository. See https://github.com/maykar/kiosk-mode

Now, I can use the kiosk query parameter, to hide the header and sidebar on the dashboard:

For example : http://192.168.2.34:8123/lovelace/home?kiosk

Note, that this only works correctly, if you hide the sidebar by default, for the Kiosk user:

![home assistant](https://i.imgur.com/pKVELn4.png)
